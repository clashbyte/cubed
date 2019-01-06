using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Components;
using Cubed.Graphics;
using Cubed.Input;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.World {

	/// <summary>
	/// Scene
	/// </summary>
	public class Scene {

		/// <summary>
		/// Millis per frame
		/// </summary>
		const int FRAME_TICKS = 16;

		/// <summary>
		/// Collision resolve bounces
		/// </summary>
		const int PHYSICAL_LOOPS = 16;

		/// <summary>
		/// Scene entities list
		/// </summary>
		public List<Entity> Entities {
			get;
			private set;
		}

		/// <summary>
		/// Render skybox
		/// </summary>
		public Skybox Sky { get; set; }

		/// <summary>
		/// Scene current camera
		/// </summary>
		public Camera Camera { get; set; }

		/// <summary>
		/// Scene fog
		/// </summary>
		public Fog Fog { get; set; }

		/// <summary>
		/// Current static level
		/// </summary>
		public Map Map {
			get {
				return map;
			}
			set {
				if (map != value) {
					map = value;
					foreach (Entity ent in Entities) {
						if (ent is Light) {
							(ent as Light).MakeDirty();
						}
					}
				}
			}
		}

		/// <summary>
		/// Background scene color
		/// </summary>
		public Color BackColor { get; set; }

		/// <summary>
		/// Is update paused
		/// </summary>
		public bool Paused { get; set; }

		/// <summary>
		/// Scene clearing mode
		/// </summary>
		public ClearMode Mode { get; set; }

		/// <summary>
		/// Current scene
		/// </summary>
		public static Scene Current {
			get;
			private set;
		}

		/// <summary>
		/// Last frame update
		/// </summary>
		protected int LastUpdateTime;

		/// <summary>
		/// Current map
		/// </summary>
		protected Map map;

		/// <summary>
		/// Scene constructor
		/// </summary>
		public Scene() {
			Entities = new List<Entity>();
			BackColor = Color.FromArgb(40, 40, 40);
			Mode = ClearMode.ClearAll;
		}

		/// <summary>
		/// Обновление логики
		/// </summary>
		internal void Update() {

			// Making this scene current
			Current = this;

			// Количество тиков для обновления
			int times = 1;
			if (LastUpdateTime == 0) {
				LastUpdateTime = Environment.TickCount - FRAME_TICKS;
			} else {
				times = (Environment.TickCount - LastUpdateTime) / FRAME_TICKS;
			}

			// Сборка списка объектов
			List<EntityComponent> updateable = new List<EntityComponent>();
			List<EntityComponent> lateUpdateable = new List<EntityComponent>();
			foreach (Entity e in Entities) {
				updateable.AddRange(e.GetLogicalComponents());
				lateUpdateable.AddRange(e.GetLateLogicalComponents());
			}

			LastUpdateTime += FRAME_TICKS * times;

			// Updating map geometry
			if (Map != null) {
				List<Light> lightList = new List<Light>();
				List<ObstructEntity> obstructorList = new List<ObstructEntity>();
				foreach (Entity en in Entities) {
					if (en is Light) {
						lightList.Add(en as Light);
					} else if(en is ObstructEntity) {
						obstructorList.Add(en as ObstructEntity);
					}
				}
				Map.Update(lightList, obstructorList, this);
				Caps.CheckErrors();
			}

			// Updating objects
			for (int i = 0; i < times; i++) {
				if (!Paused) {
					foreach (Entity e in Entities) {
						e.Update();
						Caps.CheckErrors();
					}
					foreach (EntityComponent e in updateable) {
						(e as IUpdatable).Update();
						Caps.CheckErrors();
					}
				}
			}

			// Colliding
			List<Collider> cols = new List<Collider>();
			foreach (Entity ent in Entities) {
				if (ent.BoxCollider != null) {
					ent.BoxCollider.Reset();
					cols.Add(ent.BoxCollider);
				}
			}
			Collider[] ocols = cols.ToArray();
			for (int i = 0; i < PHYSICAL_LOOPS; i++) {
				bool resolved = true;
				foreach (Entity ent in Entities) {
					if (ent.BoxCollider != null) {
						if (ent.BoxCollider.Collide(map, ocols)) {
							resolved = false;
						}
					}
				}
				if (resolved) {
					break;
				}
			}

			// Late entity update
			for (int i = 0; i < times; i++) {
				if (!Paused) {
					foreach (EntityComponent e in lateUpdateable) {
						(e as ILateUpdatable).LateUpdate();
						Caps.CheckErrors();
					}
				}
			}

			// Releasing current
			Current = null;
		}

		/// <summary>
		/// Отрисовка всех объектов для каждой камеры
		/// </summary>
		internal void Render() {

			// Making this scene current
			Current = this;

			// Очистка сцены
			GL.ClearColor(BackColor);
			switch (Mode) {
				case ClearMode.ClearDepth:
					GL.Clear(ClearBufferMask.DepthBufferBit);
					break;
				case ClearMode.ClearAll:
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
					break;
			}

			// Параметры
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
			GL.PolygonOffset(-1f, -1f);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);

			// Отрисовка камеры
			Vector3 cameraPos = Vector3.Zero;
			if (Camera != null) {
				Camera.Setup();
				Caps.CheckErrors();
				if (Sky != null && Mode == ClearMode.ClearAll) {
					Camera.LoadSkyMatrix();
					Sky.Render();
					Caps.CheckErrors();
				}
				Camera.LoadMatrix();
				Caps.CheckErrors();
				cameraPos = Camera.Position;
				GL.Enable(EnableCap.DepthTest);
				GL.DepthFunc(DepthFunction.Lequal);
			}

			// Сборка объектов
			bool needAlphaPass = false, needBlendPass = false;
			List<RenderableGroup> opaqueGroups = new List<RenderableGroup>();
			List<RenderableGroup> alphaTestGroups = new List<RenderableGroup>();
			List<RangedRenderableGroup> alphaBlendGroups = new List<RangedRenderableGroup>();

			foreach (Entity e in Entities) {
				if (e.Visible) {
					Matrix4 entityMatrix = e.RenditionMatrix;
					RenderableGroup opaque = new RenderableGroup();
					RenderableGroup alphaTest = new RenderableGroup();
					RangedRenderableGroup alphaBlend = new RangedRenderableGroup();

					IEnumerable<EntityComponent> components = e.GetVisualComponents();
					foreach (EntityComponent c in components) {
						switch (c.RenditionPass) {
							case EntityComponent.TransparencyPass.AlphaTest:
								alphaTest.Components.Add(c);
								break;
							case EntityComponent.TransparencyPass.Blend:
								alphaBlend.Components.Add(c);
								break;
							default:
								opaque.Components.Add(c);
								break;
						}
					}

					if (opaque.Components.Count > 0) {
						opaque.Matrix = entityMatrix;
						opaqueGroups.Add(opaque);
					}
					if (alphaTest.Components.Count > 0) {
						alphaTest.Matrix = entityMatrix;
						alphaTestGroups.Add(alphaTest);
						needAlphaPass = true;
					}
					if (alphaBlend.Components.Count > 0) {
						alphaBlend.Matrix = entityMatrix;
						alphaBlend.Distance = (e.Position - cameraPos).LengthSquared;
						alphaBlendGroups.Add(alphaBlend);
						needBlendPass = true;
					}
				}
			}

			// Rendering map geometry
			if (Map != null) {
				Map.Render(this);
				Caps.CheckErrors();
			}

			// Opaque pass
			foreach (RenderableGroup g in opaqueGroups) {
				g.Render();
				Caps.CheckErrors();
			}

			// Alpha pass
			if (needAlphaPass || needBlendPass) {
				// Enabling blending
				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

				// Alphatest pass
				if (needAlphaPass) {

					// Enabling alphatest
					if (Caps.ShaderPipeline) {
						ShaderSystem.IsAlphaTest = true;
					} else {
						GL.Enable(EnableCap.AlphaTest);
						GL.AlphaFunc(AlphaFunction.Gequal, 0.9f);
					}

					// Rendering surfaces
					foreach (RenderableGroup g in alphaTestGroups) {
						g.Render();
						Caps.CheckErrors();
					}

					// Disabling alphatest
					if (Caps.ShaderPipeline) {
						ShaderSystem.IsAlphaTest = false;
					} else {
						GL.Disable(EnableCap.AlphaTest);
					}
				}

				// Handle semi-transparent
				if (needBlendPass) {

					// Sorting surfaces from far to near
					alphaBlendGroups.Sort((a, b) => {
						return b.Distance.CompareTo(a.Distance);
					});

					// Disabling depth writing
					GL.DepthMask(false);

					// Rendering surfaces
					foreach (RangedRenderableGroup rg in alphaBlendGroups) {
						rg.Render();
						Caps.CheckErrors();
					}

					// Enabling depth back
					GL.DepthMask(true);
				}

				// Disabling blend
				GL.Disable(EnableCap.Blend);
			}

			// Disabling states
			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.DepthTest);

			// Releasing current
			Current = null;
		}

		/// <summary>
		/// Resetting update logics
		/// </summary>
		internal void ResetUpdateCounter() {
			LastUpdateTime = 0;
		}

		/// <summary>
		/// Group of surfaces to render
		/// </summary>
		class RenderableGroup {
			/// <summary>
			/// Matrix
			/// </summary>
			public Matrix4 Matrix;
			/// <summary>
			/// Visible components
			/// </summary>
			public List<EntityComponent> Components;

			/// <summary>
			/// Group constructor
			/// </summary>
			public RenderableGroup() {
				Components = new List<EntityComponent>();
			}

			/// <summary>
			/// Rendering group
			/// </summary>
			public void Render() {

				// Uploading matrix
				if (Caps.ShaderPipeline) {
					ShaderSystem.EntityMatrix = Matrix;
				} else {
					GL.PushMatrix();
					GL.MultMatrix(ref Matrix);
				}

				// Rendering surfaces
				foreach (EntityComponent c in Components) {
					RenderSingle(c);
				}

				// Releasing matrix
				if (!Caps.ShaderPipeline) {
					GL.PopMatrix();
				}
			}

			/// <summary>
			/// Rendering single object
			/// </summary>
			/// <param name="c">Object</param>
			protected virtual void RenderSingle(EntityComponent c) {
				if (c is IRenderable) {
					(c as IRenderable).Render();
				}
			}
		}

		/// <summary>
		/// Group of semi-transparent surfaces
		/// </summary>
		class RangedRenderableGroup : RenderableGroup {
			/// <summary>
			/// Distance to camera
			/// </summary>
			public float Distance;

			/// <summary>
			/// Drawing single object
			/// </summary>
			/// <param name="c">Object</param>
			protected override void RenderSingle(EntityComponent c) {
				switch (c.RenditionBlending) {
					case EntityComponent.BlendingMode.AlphaChannel:
						GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
						break;
					case EntityComponent.BlendingMode.Brightness:
						GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.Zero);
						break;
					case EntityComponent.BlendingMode.Add:
						GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
						break;
					case EntityComponent.BlendingMode.Multiply:
						GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.Zero);
						break;
					case EntityComponent.BlendingMode.ForceOpaque:
						GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
						break;

				}
				base.RenderSingle(c);
			}
		}

		/// <summary>
		/// Getting light level at point
		/// </summary>
		/// <returns>Color</returns>
		public Color GetLightAtPoint(float x, float y, float z) {
			if (map != null) {
				List<Light> lightList = new List<Light>();
				foreach (Entity en in Entities) {
					if (en is Light) {
						lightList.Add(en as Light);
					}
				}
				return map.GetLightLevel(x, y, z);
			}
			return Color.White;
		}

		/// <summary>
		/// Collecting all colliders
		/// </summary>
		/// <returns>Array of colliders</returns>
		internal Collider[] GetAllColliders() {
			List<Collider> cols = new List<Collider>();
			foreach (Entity e in Entities) {
				if (e.BoxCollider != null) {
					cols.Add(e.BoxCollider);
				}
			}
			return cols.ToArray();
		}

		/// <summary>
		/// Scene background clearing mode
		/// </summary>
		public enum ClearMode {
			/// <summary>
			/// No clearing
			/// </summary>
			None,
			/// <summary>
			/// Depth clearing
			/// </summary>
			ClearDepth,
			/// <summary>
			/// Full clearing
			/// </summary>
			ClearAll
		}
	}
}
