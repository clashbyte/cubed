using Cubed.Components;
using Cubed.Components.Rendering;
using Cubed.Core;
using Cubed.Data.Shaders;
using Cubed.Data.Types;
using Cubed.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Cubed.World {

	/// <summary>
	/// Door
	/// </summary>
	public class Door : ObstructEntity {
		
		/// <summary>
		/// Map
		/// </summary>
		public Map ParentMap {
			get {
				return parent;
			}
			set {
				if (parent != value) {
					parent = value;
				}
			}
		}

		/// <summary>
		/// Door width
		/// </summary>
		public int Width {
			get {
				return component.Width;
			}
			set {
				component.Width = value;
			}
		}

		/// <summary>
		/// Door height
		/// </summary>
		public int Height {
			get {
				return component.Height;
			}
			set {
				component.Height = value;
			}
		}

		/// <summary>
		/// Door thickness
		/// </summary>
		public float Thickness {
			get {
				return component.Thickness;
			}
			set {
				component.Thickness = value;
			}
		}

		/// <summary>
		/// Door opening mode
		/// </summary>
		public DoorType OpenType {
			get {
				return component.OpenType;
			}
			set {
				component.OpenType = value;
			}
		}

		/// <summary>
		/// Front side texture
		/// </summary>
		public Texture FrontTexture {
			get {
				return component.FrontTexture;
			}
			set {
				component.FrontTexture = value;
			}
		}

		/// <summary>
		/// Back side texture
		/// </summary>
		public Texture RearTexture {
			get {
				return component.RearTexture;
			}
			set {
				component.RearTexture = value;
			}
		}

		/// <summary>
		/// Trim side texture
		/// </summary>
		public Texture SideTexture {
			get {
				return component.SideTexture;
			}
			set {
				component.SideTexture = value;
			}
		}

		/// <summary>
		/// Updating object
		/// </summary>
		public float Value {
			get {
				return component.Value;
			}
			set {
				component.Value = value;
			}
		}

		/// <summary>
		/// Parental map
		/// </summary>
		Map parent;

		/// <summary>
		/// Hidden door component
		/// </summary>
		DoorComponent component;

		/// <summary>
		/// Entities to store colliders
		/// </summary>
		Entity[] colliderEntities;

		/// <summary>
		/// Hidden colliders
		/// </summary>
		Collider[] colliders;

		/// <summary>
		/// Door constructor
		/// </summary>
		public Door() {
			AddComponent(component = new DoorComponent() {
				Width = 1,
				Height = 1,
				Thickness = 1f / 10f
			});
			InvalidateMesh();
		}

		/// <summary>
		/// Check if this door touches light
		/// </summary>
		/// <param name="light">Light</param>
		/// <returns>True if door touches light</returns>
		public override bool TouchesLight(Light light) {
			float dist = (float)Math.Pow((new Vector3(Width, Height, Thickness)).LengthFast + light.Range, 2);
			return (Position - light.Position).LengthSquared < dist;
		}

		/// <summary>
		/// Rebuild lights
		/// </summary>
		protected override void RebuildMatrix() {
			base.RebuildMatrix();
			UpdateLights();
			InvalidateStatic();
			InvalidateDynamic();
			UpdateColliders();
		}

		/// <summary>
		/// Removing data and destroying lights
		/// </summary>
		public override void Destroy() {
			base.Destroy();
			UpdateLights();
		}

		/// <summary>
		/// Rebuilding mesh
		/// </summary>
		public override void RebuildMesh() {
			component.RebuildMesh();
			UpdateColliders();
			UpdateLights();
		}

		/// <summary>
		/// Rebuilding static lightmap
		/// </summary>
		public override void RebuildStaticLightmap() {
			component.UpdateLightLayer(true, AffectedLights);
		}

		/// <summary>
		/// Rebuilding dynamic lightmap
		/// </summary>
		public override void RebuildDynamicLightmap() {
			component.UpdateLightLayer(false, AffectedLights);
		}

		/// <summary>
		/// Rendering object at shadow pass
		/// </summary>
		public override void RenderShadowPass() {
			component.RenderShadow();
		}

		/// <summary>
		/// Updating colliders
		/// </summary>
		void UpdateColliders() {
			Scene scene = Scene.Current;
			if (scene == null) {
				return;
			}

			// Calculating sizes
			Vector2 rotSize = RotateVector(new Vector2(Width, Thickness), Angles.Y);
			Vector3 size = new Vector3(rotSize.X, Height, rotSize.Y);
			size.X = Math.Abs(size.X);
			size.Y = Math.Abs(size.Y);
			size.Z = Math.Abs(size.Z);

			// Creating colliders
			List<Entity> cols = new List<Entity>();
			switch (OpenType) {

				// Sliding doors
				case DoorType.SlideLeft:
				case DoorType.SlideUp:
				case DoorType.SlideRight:
				case DoorType.SlideDown:
					float val = Math.Abs(Value);
					Vector3 shift = Vector3.Zero;
					if (OpenType == DoorType.SlideUp || OpenType == DoorType.SlideDown) {
						shift.Y = val * ((float)Height - 0.1f) * (OpenType == DoorType.SlideDown ? -1 : 1);
					} else {
						shift.X = val * ((float)Width - 0.1f) * (OpenType == DoorType.SlideLeft ? -1 : 1);
					}
					Vector2 rotShift = RotateVector(shift.Xz, -Angles.Y);
					shift.X = rotShift.X;
					shift.Z = rotShift.Y;

					Entity ent = new Entity();
					ent.Parent = this;
					ent.Position = shift + Position;
					ent.BoxCollider = new Collider() {
						Size = size
					};
					cols.Add(ent);
					break;

				// Horizontal double
				case DoorType.DoubleSlideHorizontal:
					val = Math.Abs(Value);
					shift = new Vector3((float)Width * 0.25f + ((float)Width * 0.5f - 0.1f) * val, 0, 0);
					rotShift = RotateVector(shift.Xz, -Angles.Y);
					shift.X = rotShift.X;
					shift.Z = rotShift.Y;
					Vector2 rotSizeSub = RotateVector(new Vector2((float)Width / 2f, Thickness), Angles.Y);
					Vector3 sizeSub = new Vector3(rotSizeSub.X, Height, rotSizeSub.Y);
					sizeSub.X = Math.Abs(sizeSub.X);
					sizeSub.Y = Math.Abs(sizeSub.Y);
					sizeSub.Z = Math.Abs(sizeSub.Z);

					cols.Add(new Entity() {
						Parent = this,
						Position = shift + Position,
						BoxCollider = new Collider() {
							Size = sizeSub
						}
					});
					cols.Add(new Entity() {
						Parent = this,
						Position = -shift + Position,
						BoxCollider = new Collider() {
							Size = sizeSub
						}
					});
					break;

				// Vertical sliding double
				case DoorType.DoubleSlideVertical:
					val = Math.Abs(Value);
					shift = new Vector3(0, (float)Height * 0.25f + ((float)Height * 0.5f - 0.1f) * val, 0);
					rotShift = RotateVector(shift.Xz, Angles.Y);
					shift.X = rotShift.X;
					shift.Z = rotShift.Y;
					rotSizeSub = RotateVector(new Vector2((float)Width, Thickness), -Angles.Y);
					sizeSub = new Vector3(rotSizeSub.X, (float)Height * 0.5f, rotSizeSub.Y);
					sizeSub.X = Math.Abs(sizeSub.X);
					sizeSub.Y = Math.Abs(sizeSub.Y);
					sizeSub.Z = Math.Abs(sizeSub.Z);

					cols.Add(new Entity() {
						Parent = this,
						Position = shift + Position,
						BoxCollider = new Collider() {
							Size = sizeSub
						}
					});
					cols.Add(new Entity() {
						Parent = this,
						Position = -shift + Position,
						BoxCollider = new Collider() {
							Size = sizeSub
						}
					});
					break;

				// Hinged left-right
				case DoorType.HingeLeft:
				case DoorType.HingeRight:
					val = Math.Abs(Value);
					shift = new Vector3(
						-(float)Width / 2f,
						0,
						Math.Sign(Value) * (float)Width / 2f
					);
					if (OpenType == DoorType.HingeRight) {
						shift.X *= -1f;
					}
					shift = Vector3.Lerp(Vector3.Zero, shift, val);
					rotShift = RotateVector(shift.Xz, -Angles.Y);
					shift.X = rotShift.X;
					shift.Z = rotShift.Y;

					rotSizeSub = RotateVector(new Vector2((float)Width, Thickness), -Angles.Y - 90f * val);
					sizeSub = new Vector3(rotSizeSub.X, Height, rotSizeSub.Y);
					sizeSub.X = Math.Abs(sizeSub.X);
					sizeSub.Y = Math.Abs(sizeSub.Y);
					sizeSub.Z = Math.Abs(sizeSub.Z);

					cols.Add(new Entity() {
						Parent = this,
						Position = shift + Position,
						BoxCollider = new Collider() {
							Size = sizeSub
						}
					});
					break;

				// Hinged up-down
				case DoorType.HingeUp:
				case DoorType.HingeDown:
					val = Math.Abs(Value);
					shift = new Vector3(
						0,
						(float)Height / 2f,
						Math.Sign(Value) * (float)Height / 2f
					);
					if (OpenType == DoorType.HingeDown) {
						shift.Y *= -1f;
					}
					shift = Vector3.Lerp(Vector3.Zero, shift, val);
					rotShift = RotateVector(shift.Xz, -Angles.Y);
					shift.X = rotShift.X;
					shift.Z = rotShift.Y;

					Vector2 turnSizeSub = RotateVector(new Vector2(Thickness, Height), -90f * val);
					rotSizeSub = RotateVector(new Vector2(Width, turnSizeSub.X), -Angles.Y);
					sizeSub = new Vector3(rotSizeSub.X, turnSizeSub.Y, rotSizeSub.Y);
					sizeSub.X = Math.Abs(sizeSub.X);
					sizeSub.Y = Math.Abs(sizeSub.Y);
					sizeSub.Z = Math.Abs(sizeSub.Z);

					cols.Add(new Entity() {
						Parent = this,
						Position = shift + Position,
						BoxCollider = new Collider() {
							Size = sizeSub
						}
					});
					break;

				case DoorType.DoubleHingeHorizontal:
					val = Math.Abs(Value);
					for (int i = 0; i < 2; i++) {
						shift = new Vector3(
							-(float)Width / 2f,
							0,
							Math.Sign(Value) * (float)Width / 4f
						);
						Vector3 subShift = -Vector3.UnitX * ((float)Width / 4f);
						if (i == 1) {
							shift.X *= -1f;
							subShift.X *= -1f;
						}
						shift = Vector3.Lerp(subShift, shift, val);
						rotShift = RotateVector(shift.Xz, -Angles.Y);
						shift.X = rotShift.X;
						shift.Z = rotShift.Y;

						rotSizeSub = RotateVector(new Vector2((float)Width / 2f, Thickness), -Angles.Y - 90f * val);
						sizeSub = new Vector3(rotSizeSub.X, Height, rotSizeSub.Y);
						sizeSub.X = Math.Abs(sizeSub.X);
						sizeSub.Y = Math.Abs(sizeSub.Y);
						sizeSub.Z = Math.Abs(sizeSub.Z);

						cols.Add(new Entity() {
							Parent = this,
							Position = shift + Position,
							BoxCollider = new Collider() {
								Size = sizeSub
							}
						});
					}
					break;

				case DoorType.DoubleHingeVertical:
					for (int i = 0; i < 2; i++) {
						val = Math.Abs(Value);
						shift = new Vector3(
							0,
							(float)Height / 2f,
							Math.Sign(Value) * (float)Height / 4f
						);
						Vector3 subShift = Vector3.UnitY * ((float)Height / 4f);
						if (i == 1) {
							shift.Y *= -1f;
							subShift.Y *= -1f;
						}
						shift = Vector3.Lerp(subShift, shift, val);
						rotShift = RotateVector(shift.Xz, -Angles.Y);
						shift.X = rotShift.X;
						shift.Z = rotShift.Y;

						turnSizeSub = RotateVector(new Vector2(Thickness, Height / 2f), -90f * val);
						rotSizeSub = RotateVector(new Vector2(Width, turnSizeSub.X), -Angles.Y);
						sizeSub = new Vector3(rotSizeSub.X, turnSizeSub.Y, rotSizeSub.Y);
						sizeSub.X = Math.Abs(sizeSub.X);
						sizeSub.Y = Math.Abs(sizeSub.Y);
						sizeSub.Z = Math.Abs(sizeSub.Z);

						cols.Add(new Entity() {
							Parent = this,
							Position = shift + Position,
							BoxCollider = new Collider() {
								Size = sizeSub
							}
						});
					}
					break;
			}

			// Removing all entities
			if (colliderEntities != null) {
				foreach (Entity oldCol in colliderEntities) {
					scene.Entities.Remove(oldCol);
					oldCol.Destroy();
				}
			}

			// Saving new entities
			foreach (Entity newCol in cols) {
				newCol.Parent = this;
				scene.Entities.Add(newCol);
			}
			colliderEntities = cols.ToArray();
		}

		/// <summary>
		/// Rotating vector by angle
		/// </summary>
		/// <param name="source">Source vector</param>
		/// <param name="angle">Angle to rotate</param>
		/// <returns>Rotated vector</returns>
		static Vector2 RotateVector(Vector2 source, float angle) {
			float rad = MathHelper.DegreesToRadians(angle);
			float sin = (float)Math.Sin(rad);
			float cos = (float)Math.Cos(rad);
			return new Vector2(
				source.X * cos - source.Y * sin,
				source.X * sin + source.Y * cos
			);
		}

		/// <summary>
		/// Hidden renderable item
		/// </summary>
		class DoorComponent : EntityComponent, IRenderable {

			/// <summary>
			/// Door width
			/// </summary>
			public int Width {
				get {
					return (int)size.X;
				}
				set {
					if (size.X != value) {
						size.X = value;
						if (Parent != null) {
							(Parent as ObstructEntity).InvalidateMesh();
						}
					}
				}
			}

			/// <summary>
			/// Door height
			/// </summary>
			public int Height {
				get {
					return (int)size.Y;
				}
				set {
					if (size.Y != value) {
						size.Y = value;
						if (Parent != null) {
							(Parent as ObstructEntity).InvalidateMesh();
						}
					}
				}
			}

			/// <summary>
			/// Door thickness
			/// </summary>
			public float Thickness {
				get {
					return thickness;
				}
				set {
					if (thickness != value) {
						thickness = value;
						if (Parent != null) {
							(Parent as ObstructEntity).InvalidateMesh();
						}
					}
				}
			}

			/// <summary>
			/// Door opening mode
			/// </summary>
			public DoorType OpenType {
				get {
					return type;
				}
				set {
					if (type != value) {
						type = value;
						if (Parent != null) {
							(Parent as ObstructEntity).InvalidateMesh();
						}
					}
				}
			}

			/// <summary>
			/// Front side texture
			/// </summary>
			public Texture FrontTexture {
				get {
					return frontGroup.tex;
				}
				set {
					frontGroup.tex = value;
				}
			}

			/// <summary>
			/// Back side texture
			/// </summary>
			public Texture RearTexture {
				get {
					return rearGroup.tex;
				}
				set {
					rearGroup.tex = value;
				}
			}

			/// <summary>
			/// Trim side texture
			/// </summary>
			public Texture SideTexture {
				get {
					return sideGroup.tex;
				}
				set {
					sideGroup.tex = value;
				}
			}

			/// <summary>
			/// Updating object
			/// </summary>
			public float Value {
				get {
					return openValue;
				}
				set {
					if (value != openValue) {
						openValue = value;
						if (Parent != null) {
							(Parent as ObstructEntity).InvalidateMesh();
						}
					}
				}
			}

			/// <summary>
			/// Internal door size
			/// </summary>
			Vector2 size;

			/// <summary>
			/// Thickness of the door
			/// </summary>
			float thickness;

			/// <summary>
			/// Opening value
			/// </summary>
			float openValue;

			/// <summary>
			/// Internal type
			/// </summary>
			DoorType type;
			
			/// <summary>
			/// Lightmap textures
			/// </summary>
			int staticLightmap, dynamicLightmap;

			/// <summary>
			/// Framebuffer
			/// </summary>
			static int frameBuffer;

			/// <summary>
			/// Lightmap size
			/// </summary>
			Vector2 lightmapSize;

			/// <summary>
			/// All the groups
			/// </summary>
			RenderGroup frontGroup, rearGroup, sideGroup, shadowGroup;

			/// <summary>
			/// Constructor for door
			/// </summary>
			public DoorComponent() {
				frontGroup = new RenderGroup();
				rearGroup = new RenderGroup();
				sideGroup = new RenderGroup();
				shadowGroup = new RenderGroup();

			}

			/// <summary>
			/// Rendering door
			/// </summary>
			void IRenderable.Render() {

				// Binding texture
				Caps.CheckErrors();
				GL.ActiveTexture(TextureUnit.Texture1);
				GL.Enable(EnableCap.Texture2D);
				GL.BindTexture(TextureTarget.Texture2D, staticLightmap);
				GL.ActiveTexture(TextureUnit.Texture2);
				GL.Enable(EnableCap.Texture2D);
				GL.BindTexture(TextureTarget.Texture2D, dynamicLightmap);
				GL.ActiveTexture(TextureUnit.Texture3);
				GL.Enable(EnableCap.Texture2D);
				Caps.CheckErrors();
				if ((Parent as Door).ParentMap.Sunlight != null) {
					GL.BindTexture(TextureTarget.Texture2D, (Parent as Door).ParentMap.Sunlight.Texture);
					MapShader.Shader.SunColor = (Parent as Door).ParentMap.Sunlight.Color;
					MapShader.Shader.SunMatrix = (Parent as Door).ParentMap.Sunlight.FullMatrix;
					Caps.CheckErrors();
				} else {
					MapShader.Shader.SunColor = Color.Black;
					MapShader.Shader.SunMatrix = Matrix4.Identity;
					Texture.BindEmpty();
					Caps.CheckErrors();
				}
				GL.ActiveTexture(TextureUnit.Texture0);
				Caps.CheckErrors();

				// Drawing surfaces
				MapShader.Shader.AmbientColor = (Parent as Door).ParentMap.Ambient;

				// Setting up fog
				Fog fog = Scene.Current.Fog;
				MapShader.Shader.FogEnabled = fog != null;
				if (fog != null) {
					MapShader.Shader.FogNear = fog.Near;
					MapShader.Shader.FogFar = fog.Far;
					MapShader.Shader.FogColor = fog.Color;
				}

				// Rendering surfaces
				Caps.CheckErrors();
				MapShader.Shader.Bind();
				frontGroup.Render();
				rearGroup.Render();
				sideGroup.Render();

				// Unbinding
				MapShader.Shader.Unbind();
				Caps.CheckErrors();
				GL.ActiveTexture(TextureUnit.Texture1);
				GL.BindTexture(TextureTarget.Texture2D, 0);
				GL.Disable(EnableCap.Texture2D);
				GL.ActiveTexture(TextureUnit.Texture2);
				GL.BindTexture(TextureTarget.Texture2D, 0);
				GL.Disable(EnableCap.Texture2D);
				GL.ActiveTexture(TextureUnit.Texture3);
				Texture.BindEmpty();
				GL.Disable(EnableCap.Texture2D);
				GL.ActiveTexture(TextureUnit.Texture0);
				Caps.CheckErrors();

			}

			/// <summary>
			/// Rendering shadow pass
			/// </summary>
			internal void RenderShadow() {
				shadowGroup.RenderShadow();
			}

			/// <summary>
			/// Rebuild light data
			/// </summary>
			/// <param name="isStatic">Static lights</param>
			/// <param name="lights">Lights</param>
			internal void UpdateLightLayer(bool isStatic, IEnumerable<Light> lights) {
				if (isStatic) {
					RebuildLightLayer(lights, true, ref staticLightmap, (int)lightmapSize.X, (int)lightmapSize.Y);
				} else {
					RebuildLightLayer(lights, false, ref dynamicLightmap, (int)lightmapSize.X, (int)lightmapSize.Y);
				}
			}

			/// <summary>
			/// Calculating culling box
			/// </summary>
			/// <returns>Culling box</returns>
			internal override CullBox GetCullingBox() {
				return new CullBox() {
					Min = new Vector3(-size.X, -size.Y, -size.X),
					Max = new Vector3(size.X, size.Y, size.X)
				};
			}

			/// <summary>
			/// Rebuilding mesh
			/// </summary>
			internal void RebuildMesh() {

				// Some build vars
				float sx = size.X / 2f - 0.0001f;
				float sy = size.Y / 2f - 0.0001f;
				float sz = thickness / 2f - 0.0001f;
				float txw = size.X * MapTriangulator.TEXELS_PER_BLOCK;
				float txh = size.Y * MapTriangulator.TEXELS_PER_BLOCK;
				float txd = (float)Math.Ceiling(thickness * MapTriangulator.TEXELS_PER_BLOCK);
				float val = openValue;

				// Planes to render
				RenderPlane[] frontPlanes = null, rearPlanes = null, sidePlanes = null;
				switch (type) {

					// Sliding doors
					case DoorType.SlideLeft:
					case DoorType.SlideUp:
					case DoorType.SlideRight:
					case DoorType.SlideDown:
						float offx = 0;
						float offy = 0;
						val = Math.Abs(val);
						if (type == DoorType.SlideUp || type == DoorType.SlideDown) {
							offy = val * (size.Y - 0.1f) * (type == DoorType.SlideDown ? -1f : 1f);
						} else {
							offx = val * (size.X - 0.1f) * (type == DoorType.SlideLeft ? -1f : 1f);
						}
						frontPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx + offx, sy + offy, -sz),
									new Vector3(sx + offx, sy + offy, -sz),
									new Vector3(-sx + offx, -sy + offy, -sz),
									new Vector3(sx + offx, -sy + offy, -sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, 0),
									new Vector2(txw, 0),
									new Vector2(0, txh),
									new Vector2(txw, txh),
								},
							}
						};
						rearPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx + offx, sy + offy, sz),
									new Vector3(-sx + offx, sy + offy, sz),
									new Vector3(sx + offx, -sy + offy, sz),
									new Vector3(-sx + offx, -sy + offy, sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw, 0),
									new Vector2(txw * 2, 0),
									new Vector2(txw, txh),
									new Vector2(txw * 2, txh),
								},
							}
						};
						sidePlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx + offx, sy + offy, -sz),
									new Vector3(sx + offx, sy + offy, sz),
									new Vector3(sx + offx, -sy + offy, -sz),
									new Vector3(sx + offx, -sy + offy, sz),

								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2, 0),
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2, txh),
									new Vector2(txw * 2 + txd, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx + offx, sy + offy, sz),
									new Vector3(-sx + offx, sy + offy, -sz),
									new Vector3(-sx + offx, -sy + offy, sz),
									new Vector3(-sx + offx, -sy + offy, -sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2 + txd * 2, 0),
									new Vector2(txw * 2 + txd, txh),
									new Vector2(txw * 2 + txd * 2, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx + offx, sy + offy, -sz),
									new Vector3(-sx + offx, sy + offy, sz),
									new Vector3(sx + offx, sy + offy, -sz),
									new Vector3(sx + offx, sy + offy, sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd),
									new Vector2(0, txh),
									new Vector2(txw, txh + txd),
									new Vector2(txw, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx + offx, -sy + offy, sz),
									new Vector3(-sx + offx, -sy + offy, -sz),
									new Vector3(sx + offx, -sy + offy, sz),
									new Vector3(sx + offx, -sy + offy, -sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 2),
									new Vector2(0, txh + txd),
									new Vector2(txw, txh + txd * 2),
									new Vector2(txw, txh + txd),
								},
							},
						};
						break;

					// Double door with horizontal sliding
					case DoorType.DoubleSlideHorizontal:
						float off = Math.Abs(val) * -(size.X / 2f - 0.1f);
						frontPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx + off, sy, -sz),
									new Vector3(off, sy, -sz),
									new Vector3(-sx + off, -sy, -sz),
									new Vector3(off, -sy, -sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(0.5f, 0f),
									new Vector2(0f, 1f),
									new Vector2(0.5f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, 0),
									new Vector2(txw / 2f, 0),
									new Vector2(0, txh),
									new Vector2(txw / 2f, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-off, sy, -sz),
									new Vector3(sx - off, sy, -sz),
									new Vector3(-off, -sy, -sz),
									new Vector3(sx - off, -sy, -sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0.5f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0.5f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw / 2f, 0),
									new Vector2(txw, 0),
									new Vector2(txw / 2f, txh),
									new Vector2(txw, txh),
								},
							}
						};
						rearPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx - off, sy, sz),
									new Vector3(-off, sy, sz),
									new Vector3(sx - off, -sy, sz),
									new Vector3(-off, -sy, sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(0.5f, 0f),
									new Vector2(0f, 1f),
									new Vector2(0.5f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw, 0),
									new Vector2(txw * 1.5f, 0),
									new Vector2(txw, txh),
									new Vector2(txw * 1.5f, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(off, sy, sz),
									new Vector3(-sx + off, sy, sz),
									new Vector3(off, -sy, sz),
									new Vector3(-sx + off, -sy, sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0.5f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0.5f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 1.5f, 0),
									new Vector2(txw * 2f, 0),
									new Vector2(txw * 1.5f, txh),
									new Vector2(txw * 2f, txh),
								},
							}
						};
						sidePlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx - off, sy, -sz),
									new Vector3(sx - off, sy, sz),
									new Vector3(sx - off, -sy, -sz),
									new Vector3(sx - off, -sy, sz),

								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2, 0),
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2, txh),
									new Vector2(txw * 2 + txd, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx + off, sy, sz),
									new Vector3(-sx + off, sy, -sz),
									new Vector3(-sx + off, -sy, sz),
									new Vector3(-sx + off, -sy, -sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2 + txd * 2, 0),
									new Vector2(txw * 2 + txd, txh),
									new Vector2(txw * 2 + txd * 2, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(off, sy, -sz),
									new Vector3(off, sy, sz),
									new Vector3(off, -sy, -sz),
									new Vector3(off, -sy, sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd * 2, 0),
									new Vector2(txw * 2 + txd * 3, 0),
									new Vector2(txw * 2 + txd * 2, txh),
									new Vector2(txw * 2 + txd * 3, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-off, sy, sz),
									new Vector3(-off, sy, -sz),
									new Vector3(-off, -sy, sz),
									new Vector3(-off, -sy, -sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd * 3, 0),
									new Vector2(txw * 2 + txd * 4, 0),
									new Vector2(txw * 2 + txd * 3, txh),
									new Vector2(txw * 2 + txd * 4, txh),
								},
							},
							
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx + off, sy, -sz),
									new Vector3(-sx + off, sy, sz),
									new Vector3(off, sy, -sz),
									new Vector3(off, sy, sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY * 0.5f,
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd),
									new Vector2(0, txh),
									new Vector2(txw * 0.5f, txh + txd),
									new Vector2(txw * 0.5f, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-off, sy, -sz),
									new Vector3(-off, sy, sz),
									new Vector3(sx - off, sy, -sz),
									new Vector3(sx - off, sy, sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 0.5f, txh + txd),
									new Vector2(txw * 0.5f, txh),
									new Vector2(txw, txh + txd),
									new Vector2(txw, txh),
								},
							},

							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx + off, -sy, sz),
									new Vector3(-sx + off, -sy, -sz),
									new Vector3(off, -sy, sz),
									new Vector3(off, -sy, -sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY * 0.5f,
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 3),
									new Vector2(0, txh + txd * 2),
									new Vector2(txw * 0.5f, txh + txd * 3),
									new Vector2(txw * 0.5f, txh + txd * 2),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-off, -sy, sz),
									new Vector3(-off, -sy, -sz),
									new Vector3(sx - off, -sy, sz),
									new Vector3(sx - off, -sy, -sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 0.5f, txh + txd * 3),
									new Vector2(txw * 0.5f, txh + txd * 2),
									new Vector2(txw, txh + txd * 3),
									new Vector2(txw, txh + txd * 2),
								},
							},
						};

						break;

					// Double door with vertical slide
					case DoorType.DoubleSlideVertical:
						off = Math.Abs(val) * -(size.Y / 2f - 0.1f);
						frontPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, sy - off, -sz),
									new Vector3(sx, sy - off, -sz),
									new Vector3(-sx, -off, -sz),
									new Vector3(sx, -off, -sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, 0),
									new Vector2(txw, 0),
									new Vector2(0, txh / 2f),
									new Vector2(txw, txh / 2f),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, off, -sz),
									new Vector3(sx, off, -sz),
									new Vector3(-sx, -sy + off, -sz),
									new Vector3(sx, -sy + off, -sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh / 2f),
									new Vector2(txw, txh / 2f),
									new Vector2(0, txh),
									new Vector2(txw, txh),
								},
							}
						};
						rearPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, sy - off, sz),
									new Vector3(-sx, sy - off, sz),
									new Vector3(sx, -off, sz),
									new Vector3(-sx, -off, sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw, 0),
									new Vector2(txw * 2f, 0),
									new Vector2(txw, txh / 2f),
									new Vector2(txw * 2, txh / 2f),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, off, sz),
									new Vector3(-sx, off, sz),
									new Vector3(sx, -sy + off, sz),
									new Vector3(-sx, -sy + off, sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw, txh / 2f),
									new Vector2(txw * 2f, txh / 2f),
									new Vector2(txw, txh),
									new Vector2(txw * 2f, txh),
								},
							}
						};
						sidePlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, sy - off, -sz),
									new Vector3(sx, sy - off, sz),
									new Vector3(sx, -off, -sz),
									new Vector3(sx, -off, sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2, 0),
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2, txh / 2f),
									new Vector2(txw * 2 + txd, txh / 2f),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, off, -sz),
									new Vector3(sx, off, sz),
									new Vector3(sx, -sy + off, -sz),
									new Vector3(sx, -sy + off, sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2, txh / 2f),
									new Vector2(txw * 2 + txd, txh / 2f),
									new Vector2(txw * 2, txh),
									new Vector2(txw * 2 + txd, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, sy - off, sz),
									new Vector3(-sx, sy - off, -sz),
									new Vector3(-sx, -off, sz),
									new Vector3(-sx, -off, -sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2 + txd * 2f, 0),
									new Vector2(txw * 2 + txd, txh / 2f),
									new Vector2(txw * 2 + txd * 2f, txh / 2f),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, off, sz),
									new Vector3(-sx, off, -sz),
									new Vector3(-sx, -sy + off, sz),
									new Vector3(-sx, -sy + off, -sz),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd, txh / 2f),
									new Vector2(txw * 2 + txd * 2f, txh / 2f),
									new Vector2(txw * 2 + txd, txh),
									new Vector2(txw * 2 + txd * 2f, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, sy - off, -sz),
									new Vector3(-sx, sy - off, sz),
									new Vector3(sx, sy - off, -sz),
									new Vector3(sx, sy - off, sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd),
									new Vector2(0, txh),
									new Vector2(txw, txh + txd),
									new Vector2(txw, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -off, sz),
									new Vector3(-sx, -off, -sz),
									new Vector3(sx, -off, sz),
									new Vector3(sx, -off, -sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 2f),
									new Vector2(0, txh + txd),
									new Vector2(txw, txh + txd * 2f),
									new Vector2(txw, txh + txd),
								},
							},

							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -sy + off, sz),
									new Vector3(-sx, -sy + off, -sz),
									new Vector3(sx, -sy + off, sz),
									new Vector3(sx, -sy + off, -sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 3),
									new Vector2(0, txh + txd * 2),
									new Vector2(txw, txh + txd * 3),
									new Vector2(txw, txh + txd * 2),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, off, -sz),
									new Vector3(-sx, off, sz),
									new Vector3(sx, off, -sz),
									new Vector3(sx, off, sz),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 4f),
									new Vector2(0, txh + txd * 3f),
									new Vector2(txw, txh + txd * 4f),
									new Vector2(txw, txh + txd * 3f),
								},
							},
						};

						break;

					// Hinged left-right
					case DoorType.HingeLeft:
					case DoorType.HingeRight:
						float angle = openValue * 90f;
						Vector2[] baseVerts = new Vector2[] {
							RotateVector(new Vector2(0, sz), angle),
							RotateVector(new Vector2(sx * 2f, sz), angle),
							RotateVector(new Vector2(0, -sz), angle),
							RotateVector(new Vector2(sx * 2f, -sz), angle),
						};
						if (type == DoorType.HingeLeft) {
							for (int i = 0; i < 4; i++) {
								baseVerts[i] -= Vector2.UnitX * sx;
							}
						} else {
							Vector2 neg = new Vector2(-1f, 1f);
							baseVerts = new Vector2[] {
								baseVerts[1] * neg + Vector2.UnitX * sx,
								baseVerts[0] * neg + Vector2.UnitX * sx,
								baseVerts[3] * neg + Vector2.UnitX * sx,
								baseVerts[2] * neg + Vector2.UnitX * sx,
							};
						}

						frontPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[2].X, sy, baseVerts[2].Y),
									new Vector3(baseVerts[3].X, sy, baseVerts[3].Y),
									new Vector3(baseVerts[2].X, -sy, baseVerts[2].Y),
									new Vector3(baseVerts[3].X, -sy, baseVerts[3].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, 0),
									new Vector2(txw, 0),
									new Vector2(0, txh),
									new Vector2(txw, txh),
								},
							}
						};
						rearPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[1].X, sy, baseVerts[1].Y),
									new Vector3(baseVerts[0].X, sy, baseVerts[0].Y),
									new Vector3(baseVerts[1].X, -sy, baseVerts[1].Y),
									new Vector3(baseVerts[0].X, -sy, baseVerts[0].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw, 0),
									new Vector2(txw * 2, 0),
									new Vector2(txw, txh),
									new Vector2(txw * 2, txh),
								},
							}
						};
						sidePlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[3].X, sy, baseVerts[3].Y),
									new Vector3(baseVerts[1].X, sy, baseVerts[1].Y),
									new Vector3(baseVerts[3].X, -sy, baseVerts[3].Y),
									new Vector3(baseVerts[1].X, -sy, baseVerts[1].Y),

								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2, 0),
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2, txh),
									new Vector2(txw * 2 + txd, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[0].X, sy, baseVerts[0].Y),
									new Vector3(baseVerts[2].X, sy, baseVerts[2].Y),
									new Vector3(baseVerts[0].X, -sy, baseVerts[0].Y),
									new Vector3(baseVerts[2].X, -sy, baseVerts[2].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2 + txd * 2, 0),
									new Vector2(txw * 2 + txd, txh),
									new Vector2(txw * 2 + txd * 2, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[2].X, sy, baseVerts[2].Y),
									new Vector3(baseVerts[0].X, sy, baseVerts[0].Y),
									new Vector3(baseVerts[3].X, sy, baseVerts[3].Y),
									new Vector3(baseVerts[1].X, sy, baseVerts[1].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd),
									new Vector2(0, txh),
									new Vector2(txw, txh + txd),
									new Vector2(txw, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[0].X, -sy, baseVerts[0].Y),
									new Vector3(baseVerts[2].X, -sy, baseVerts[2].Y),
									new Vector3(baseVerts[1].X, -sy, baseVerts[1].Y),
									new Vector3(baseVerts[3].X, -sy, baseVerts[3].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 2),
									new Vector2(0, txh + txd),
									new Vector2(txw, txh + txd * 2),
									new Vector2(txw, txh + txd),
								},
							},
						};
						break;

					// Hinged up-down
					case DoorType.HingeUp:
					case DoorType.HingeDown:
						angle = openValue * 90f;
						baseVerts = new Vector2[] {
							RotateVector(new Vector2(0, sz), angle),
							RotateVector(new Vector2(sy * 2f, sz), angle),
							RotateVector(new Vector2(0, -sz), angle),
							RotateVector(new Vector2(sy * 2f, -sz), angle),
						};
						if (type == DoorType.HingeDown) {
							for (int i = 0; i < 4; i++) {
								baseVerts[i] -= Vector2.UnitX * sy;
							}
						} else {
							Vector2 neg = new Vector2(-1f, 1f);
							baseVerts = new Vector2[] {
								baseVerts[1] * neg + Vector2.UnitX * sy,
								baseVerts[0] * neg + Vector2.UnitX * sy,
								baseVerts[3] * neg + Vector2.UnitX * sy,
								baseVerts[2] * neg + Vector2.UnitX * sy,
							};
						}

						frontPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, baseVerts[3].X, baseVerts[3].Y),
									new Vector3(sx, baseVerts[3].X, baseVerts[3].Y),
									new Vector3(-sx, baseVerts[2].X, baseVerts[2].Y),
									new Vector3(sx, baseVerts[2].X, baseVerts[2].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, 0),
									new Vector2(txw, 0),
									new Vector2(0, txh),
									new Vector2(txw, txh),
								},
							}
						};
						rearPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, baseVerts[1].X, baseVerts[1].Y),
									new Vector3(-sx, baseVerts[1].X, baseVerts[1].Y),
									new Vector3(sx, baseVerts[0].X, baseVerts[0].Y),
									new Vector3(-sx, baseVerts[0].X, baseVerts[0].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw, 0),
									new Vector2(txw * 2, 0),
									new Vector2(txw, txh),
									new Vector2(txw * 2, txh),
								},
							}
						};
						sidePlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, baseVerts[3].X, baseVerts[3].Y),
									new Vector3(sx, baseVerts[1].X, baseVerts[1].Y),
									new Vector3(sx, baseVerts[2].X, baseVerts[2].Y),
									new Vector3(sx, baseVerts[0].X, baseVerts[0].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2, 0),
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2, txh),
									new Vector2(txw * 2 + txd, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, baseVerts[1].X, baseVerts[1].Y),
									new Vector3(-sx, baseVerts[3].X, baseVerts[3].Y),
									new Vector3(-sx, baseVerts[0].X, baseVerts[0].Y),
									new Vector3(-sx, baseVerts[2].X, baseVerts[2].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2 + txd * 2, 0),
									new Vector2(txw * 2 + txd, txh),
									new Vector2(txw * 2 + txd * 2, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, baseVerts[3].X, baseVerts[3].Y),
									new Vector3(-sx, baseVerts[1].X, baseVerts[1].Y),
									new Vector3(sx, baseVerts[3].X, baseVerts[3].Y),
									new Vector3(sx, baseVerts[1].X, baseVerts[1].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd),
									new Vector2(0, txh),
									new Vector2(txw, txh + txd),
									new Vector2(txw, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, baseVerts[0].X, baseVerts[0].Y),
									new Vector3(-sx, baseVerts[2].X, baseVerts[2].Y),
									new Vector3(sx, baseVerts[0].X, baseVerts[0].Y),
									new Vector3(sx, baseVerts[2].X, baseVerts[2].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 2),
									new Vector2(0, txh + txd),
									new Vector2(txw, txh + txd * 2),
									new Vector2(txw, txh + txd),
								},
							},
						};
						break;

					// Double hinged horizontal door
					case DoorType.DoubleHingeHorizontal:
						angle = openValue * 90f;
						baseVerts = new Vector2[] {
							RotateVector(new Vector2(0, sz), angle),
							RotateVector(new Vector2(sx, sz), angle),
							RotateVector(new Vector2(0, -sz), angle),
							RotateVector(new Vector2(sx, -sz), angle),
							RotateVector(new Vector2(-sx, sz), -angle),
							RotateVector(new Vector2(0, sz), -angle),
							RotateVector(new Vector2(-sx, -sz), -angle),
							RotateVector(new Vector2(0, -sz), -angle),
						};
						for (int i = 0; i < 4; i++) {
							baseVerts[i] -= Vector2.UnitX * sx;
						}
						for (int i = 4; i < 8; i++) {
							baseVerts[i] += Vector2.UnitX * sx;
						}

						frontPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[2].X, sy, baseVerts[2].Y),
									new Vector3(baseVerts[3].X, sy, baseVerts[3].Y),
									new Vector3(baseVerts[2].X, -sy, baseVerts[2].Y),
									new Vector3(baseVerts[3].X, -sy, baseVerts[3].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(0.5f, 0f),
									new Vector2(0f, 1f),
									new Vector2(0.5f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, 0),
									new Vector2(txw / 2f, 0),
									new Vector2(0, txh),
									new Vector2(txw / 2f, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[6].X, sy, baseVerts[6].Y),
									new Vector3(baseVerts[7].X, sy, baseVerts[7].Y),
									new Vector3(baseVerts[6].X, -sy, baseVerts[6].Y),
									new Vector3(baseVerts[7].X, -sy, baseVerts[7].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0.5f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0.5f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw / 2f, 0),
									new Vector2(txw, 0),
									new Vector2(txw / 2f, txh),
									new Vector2(txw, txh),
								},
							}
						};
						rearPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[5].X, sy, baseVerts[5].Y),
									new Vector3(baseVerts[4].X, sy, baseVerts[4].Y),
									new Vector3(baseVerts[5].X, -sy, baseVerts[5].Y),
									new Vector3(baseVerts[4].X, -sy, baseVerts[4].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(0.5f, 0f),
									new Vector2(0f, 1f),
									new Vector2(0.5f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw, 0),
									new Vector2(txw * 1.5f, 0),
									new Vector2(txw, txh),
									new Vector2(txw * 1.5f, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[1].X, sy, baseVerts[1].Y),
									new Vector3(baseVerts[0].X, sy, baseVerts[0].Y),
									new Vector3(baseVerts[1].X, -sy, baseVerts[1].Y),
									new Vector3(baseVerts[0].X, -sy, baseVerts[0].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0.5f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0.5f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 1.5f, 0),
									new Vector2(txw * 2f, 0),
									new Vector2(txw * 1.5f, txh),
									new Vector2(txw * 2f, txh),
								},
							}
						};
						sidePlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[7].X, sy, baseVerts[7].Y),
									new Vector3(baseVerts[5].X, sy, baseVerts[5].Y),
									new Vector3(baseVerts[7].X, -sy, baseVerts[7].Y),
									new Vector3(baseVerts[5].X, -sy, baseVerts[5].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2, 0),
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2, txh),
									new Vector2(txw * 2 + txd, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[0].X, sy, baseVerts[0].Y),
									new Vector3(baseVerts[2].X, sy, baseVerts[2].Y),
									new Vector3(baseVerts[0].X, -sy, baseVerts[0].Y),
									new Vector3(baseVerts[2].X, -sy, baseVerts[2].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2 + txd * 2, 0),
									new Vector2(txw * 2 + txd, txh),
									new Vector2(txw * 2 + txd * 2, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[3].X, sy, baseVerts[3].Y),
									new Vector3(baseVerts[1].X, sy, baseVerts[1].Y),
									new Vector3(baseVerts[3].X, -sy, baseVerts[3].Y),
									new Vector3(baseVerts[1].X, -sy, baseVerts[1].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd * 2, 0),
									new Vector2(txw * 2 + txd * 3, 0),
									new Vector2(txw * 2 + txd * 2, txh),
									new Vector2(txw * 2 + txd * 3, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[4].X, sy, baseVerts[4].Y),
									new Vector3(baseVerts[6].X, sy, baseVerts[6].Y),
									new Vector3(baseVerts[4].X, -sy, baseVerts[4].Y),
									new Vector3(baseVerts[6].X, -sy, baseVerts[6].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd * 3, 0),
									new Vector2(txw * 2 + txd * 4, 0),
									new Vector2(txw * 2 + txd * 3, txh),
									new Vector2(txw * 2 + txd * 4, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[2].X, sy, baseVerts[2].Y),
									new Vector3(baseVerts[0].X, sy, baseVerts[0].Y),
									new Vector3(baseVerts[3].X, sy, baseVerts[3].Y),
									new Vector3(baseVerts[1].X, sy, baseVerts[1].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY * 0.5f,
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd),
									new Vector2(0, txh),
									new Vector2(txw * 0.5f, txh + txd),
									new Vector2(txw * 0.5f, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[6].X, sy, baseVerts[6].Y),
									new Vector3(baseVerts[4].X, sy, baseVerts[4].Y),
									new Vector3(baseVerts[7].X, sy, baseVerts[7].Y),
									new Vector3(baseVerts[5].X, sy, baseVerts[5].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 0.5f, txh + txd),
									new Vector2(txw * 0.5f, txh),
									new Vector2(txw, txh + txd),
									new Vector2(txw, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[0].X, -sy, baseVerts[0].Y),
									new Vector3(baseVerts[2].X, -sy, baseVerts[2].Y),
									new Vector3(baseVerts[1].X, -sy, baseVerts[1].Y),
									new Vector3(baseVerts[3].X, -sy, baseVerts[3].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY * 0.5f,
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 3),
									new Vector2(0, txh + txd * 2),
									new Vector2(txw * 0.5f, txh + txd * 3),
									new Vector2(txw * 0.5f, txh + txd * 2),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(baseVerts[4].X, -sy, baseVerts[4].Y),
									new Vector3(baseVerts[6].X, -sy, baseVerts[6].Y),
									new Vector3(baseVerts[5].X, -sy, baseVerts[5].Y),
									new Vector3(baseVerts[7].X, -sy, baseVerts[7].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 0.5f, txh + txd * 3),
									new Vector2(txw * 0.5f, txh + txd * 2),
									new Vector2(txw, txh + txd * 3),
									new Vector2(txw, txh + txd * 2),
								},
							},
						};
						break;

					case DoorType.DoubleHingeVertical:
						angle = openValue * 90f;
						baseVerts = new Vector2[] {
							RotateVector(new Vector2(0, sz), angle),
							RotateVector(new Vector2(sy, sz), angle),
							RotateVector(new Vector2(0, -sz), angle),
							RotateVector(new Vector2(sy, -sz), angle),
							RotateVector(new Vector2(-sy, sz), -angle),
							RotateVector(new Vector2(0, sz), -angle),
							RotateVector(new Vector2(-sy, -sz), -angle),
							RotateVector(new Vector2(0, -sz), -angle),
						};
						for (int i = 0; i < 4; i++) {
							baseVerts[i] -= Vector2.UnitX * sy;
						}
						for (int i = 4; i < 8; i++) {
							baseVerts[i] += Vector2.UnitX * sy;
						}

						frontPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -baseVerts[2].X, baseVerts[2].Y),
									new Vector3(sx, -baseVerts[2].X, baseVerts[2].Y),
									new Vector3(-sx, -baseVerts[3].X, baseVerts[3].Y),
									new Vector3(sx, -baseVerts[3].X, baseVerts[3].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, 0),
									new Vector2(txw, 0),
									new Vector2(0, txh / 2f),
									new Vector2(txw, txh / 2f),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -baseVerts[6].X, baseVerts[6].Y),
									new Vector3(sx, -baseVerts[6].X, baseVerts[6].Y),
									new Vector3(-sx, -baseVerts[7].X, baseVerts[7].Y),
									new Vector3(sx, -baseVerts[7].X, baseVerts[7].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh / 2f),
									new Vector2(txw, txh / 2f),
									new Vector2(0, txh),
									new Vector2(txw, txh),
								},
							}
						};
						rearPlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, -baseVerts[0].X, baseVerts[0].Y),
									new Vector3(-sx, -baseVerts[0].X, baseVerts[0].Y),
									new Vector3(sx, -baseVerts[1].X, baseVerts[1].Y),
									new Vector3(-sx, -baseVerts[1].X, baseVerts[1].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw, 0),
									new Vector2(txw * 2f, 0),
									new Vector2(txw, txh / 2f),
									new Vector2(txw * 2, txh / 2f),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, -baseVerts[4].X, baseVerts[4].Y),
									new Vector3(-sx, -baseVerts[4].X, baseVerts[4].Y),
									new Vector3(sx, -baseVerts[5].X, baseVerts[5].Y),
									new Vector3(-sx, -baseVerts[5].X, baseVerts[5].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw, txh / 2f),
									new Vector2(txw * 2f, txh / 2f),
									new Vector2(txw, txh),
									new Vector2(txw * 2f, txh),
								},
							}
						};
						sidePlanes = new RenderPlane[] {
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, -baseVerts[2].X, baseVerts[2].Y),
									new Vector3(sx, -baseVerts[0].X, baseVerts[0].Y),
									new Vector3(sx, -baseVerts[3].X, baseVerts[3].Y),
									new Vector3(sx, -baseVerts[1].X, baseVerts[1].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2, 0),
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2, txh / 2f),
									new Vector2(txw * 2 + txd, txh / 2f),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(sx, -baseVerts[6].X, baseVerts[6].Y),
									new Vector3(sx, -baseVerts[4].X, baseVerts[4].Y),
									new Vector3(sx, -baseVerts[7].X, baseVerts[7].Y),
									new Vector3(sx, -baseVerts[5].X, baseVerts[5].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2, txh / 2f),
									new Vector2(txw * 2 + txd, txh / 2f),
									new Vector2(txw * 2, txh),
									new Vector2(txw * 2 + txd, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -baseVerts[0].X, baseVerts[0].Y),
									new Vector3(-sx, -baseVerts[2].X, baseVerts[2].Y),
									new Vector3(-sx, -baseVerts[1].X, baseVerts[1].Y),
									new Vector3(-sx, -baseVerts[3].X, baseVerts[3].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0f),
									new Vector2(1f, 0f),
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd, 0),
									new Vector2(txw * 2 + txd * 2f, 0),
									new Vector2(txw * 2 + txd, txh / 2f),
									new Vector2(txw * 2 + txd * 2f, txh / 2f),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -baseVerts[4].X, baseVerts[4].Y),
									new Vector3(-sx, -baseVerts[6].X, baseVerts[6].Y),
									new Vector3(-sx, -baseVerts[5].X, baseVerts[5].Y),
									new Vector3(-sx, -baseVerts[7].X, baseVerts[7].Y),
								},
								TexCoords = new Vector2[] {
									new Vector2(0f, 0.5f),
									new Vector2(1f, 0.5f),
									new Vector2(0f, 1f),
									new Vector2(1f, 1f),
								},
								LightCoords = new Vector2[] {
									new Vector2(txw * 2 + txd, txh / 2f),
									new Vector2(txw * 2 + txd * 2f, txh / 2f),
									new Vector2(txw * 2 + txd, txh),
									new Vector2(txw * 2 + txd * 2f, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -baseVerts[2].X, baseVerts[2].Y),
									new Vector3(-sx, -baseVerts[0].X, baseVerts[0].Y),
									new Vector3(sx, -baseVerts[2].X, baseVerts[2].Y),
									new Vector3(sx, -baseVerts[0].X, baseVerts[0].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd),
									new Vector2(0, txh),
									new Vector2(txw, txh + txd),
									new Vector2(txw, txh),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -baseVerts[1].X, baseVerts[1].Y),
									new Vector3(-sx, -baseVerts[3].X, baseVerts[3].Y),
									new Vector3(sx, -baseVerts[1].X, baseVerts[1].Y),
									new Vector3(sx, -baseVerts[3].X, baseVerts[3].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 2f),
									new Vector2(0, txh + txd),
									new Vector2(txw, txh + txd * 2f),
									new Vector2(txw, txh + txd),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -baseVerts[5].X, baseVerts[5].Y),
									new Vector3(-sx, -baseVerts[7].X, baseVerts[7].Y),
									new Vector3(sx, -baseVerts[5].X, baseVerts[5].Y),
									new Vector3(sx, -baseVerts[7].X, baseVerts[7].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 3),
									new Vector2(0, txh + txd * 2),
									new Vector2(txw, txh + txd * 3),
									new Vector2(txw, txh + txd * 2),
								},
							},
							new RenderPlane() {
								Coords = new Vector3[] {
									new Vector3(-sx, -baseVerts[6].X, baseVerts[6].Y),
									new Vector3(-sx, -baseVerts[4].X, baseVerts[4].Y),
									new Vector3(sx, -baseVerts[6].X, baseVerts[6].Y),
									new Vector3(sx, -baseVerts[4].X, baseVerts[4].Y),
								},
								TexCoords = new Vector2[] {
									Vector2.Zero,
									Vector2.UnitX,
									Vector2.UnitY,
									Vector2.One,
								},
								LightCoords = new Vector2[] {
									new Vector2(0, txh + txd * 4f),
									new Vector2(0, txh + txd * 3f),
									new Vector2(txw, txh + txd * 4f),
									new Vector2(txw, txh + txd * 3f),
								},
							},
						};
						break;
				}
				
				// Building groups
				RenderPlane[] mergedPlanes = new RenderPlane[frontPlanes.Length + rearPlanes.Length + sidePlanes.Length];
				for (int i = 0; i < frontPlanes.Length; i++) {
					mergedPlanes[i] = frontPlanes[i];
				}
				for (int i = 0; i < rearPlanes.Length; i++) {
					mergedPlanes[i + frontPlanes.Length] = rearPlanes[i];
				}
				for (int i = 0; i < sidePlanes.Length; i++) {
					mergedPlanes[i + frontPlanes.Length + rearPlanes.Length] = sidePlanes[i];
				}

				// Calculating lightmap size
				Vector2 dim = Vector2.Zero;
				foreach (RenderPlane rg in mergedPlanes) {
					foreach (Vector2 coord in rg.LightCoords) {
						dim.X = Math.Max(dim.X, coord.X);
						dim.Y = Math.Max(dim.Y, coord.Y);
					}
				}
				dim.X = Caps.NonPowerOfTwoTextures ? dim.X : MathHelper.NextPowerOfTwo(dim.X);
				dim.Y = Caps.NonPowerOfTwoTextures ? dim.Y : MathHelper.NextPowerOfTwo(dim.Y);
				foreach (RenderPlane rg in mergedPlanes) {
					for (int i = 0; i < rg.LightCoords.Length; i++) {
						rg.LightCoords[i].X /= dim.X;
						rg.LightCoords[i].Y /= dim.Y;
					}
				}
				lightmapSize = dim;

				// Convertng planes to groups
				frontGroup.SetData(frontPlanes);
				rearGroup.SetData(rearPlanes);
				sideGroup.SetData(sidePlanes);
				shadowGroup.SetData(mergedPlanes);
			}

			/// <summary>
			/// Rebuild single light layer
			/// </summary>
			/// <param name="lights">Array of lights</param>
			/// <param name="isStatic">Flag for static lights</param>
			/// <param name="texWidth">Lightmap width</param>
			/// <param name="texHeight">Lightmap height</param>
			void RebuildLightLayer(IEnumerable<Light> lights, bool isStatic, ref int glTex, int texWidth, int texHeight) {

				// Saving matrices
				Matrix4 camMat = ShaderSystem.CameraMatrix;
				Matrix4 entMat = ShaderSystem.EntityMatrix;
				Matrix4 prjMat = ShaderSystem.ProjectionMatrix;
				Matrix4 texMat = ShaderSystem.TextureMatrix;

				// Checking framebuffer
				if (frameBuffer == 0 || !GL.IsFramebuffer(frameBuffer)) {
					frameBuffer = GL.GenFramebuffer();
				}
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
				Caps.CheckErrors();

				// Checking texture
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.Enable(EnableCap.Texture2D);
				if (glTex == 0 || !GL.IsTexture(glTex)) {
					glTex = GL.GenTexture();
				}
				GL.BindTexture(TextureTarget.Texture2D, glTex);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Three, texWidth, texHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
				GL.BindTexture(TextureTarget.Texture2D, 0);
				GL.Disable(EnableCap.Texture2D);

				// Adding attachments
				GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, glTex, 0);

				Caps.CheckErrors();

				GL.ActiveTexture(TextureUnit.Texture0);
				GL.Disable(EnableCap.DepthTest);

				// Rendering scene
				GL.Viewport(0, 0, texWidth, texHeight);
				GL.Enable(EnableCap.Blend);
				GL.ClearColor(0, 0, 0, 0);
				GL.Clear(ClearBufferMask.ColorBufferBit);
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
				Caps.CheckErrors();

				// Binding shader
				ShaderSystem.CameraMatrix = Matrix4.Identity;
				ShaderSystem.EntityMatrix = Parent.RenditionMatrix;
				ShaderSystem.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -1, 2);
				MapLightPassShader shader = MapLightPassShader.Shader;
				Caps.CheckErrors();

				shader.Bind();
				GL.BindBuffer(BufferTarget.ArrayBuffer, shadowGroup.vertexBuffer);
				GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, shadowGroup.normalBuffer);
				GL.VertexAttribPointer(shader.NormalBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, shadowGroup.lightmapBuffer);
				GL.VertexAttribPointer(shader.TexBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, shadowGroup.indexBuffer);
				shader.Unbind();

				// Enabling textures
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.Enable(EnableCap.TextureCubeMap);
				GL.ActiveTexture(TextureUnit.Texture1);
				GL.Enable(EnableCap.Texture2D);
				Caps.CheckErrors();

				// Rendering lights
				foreach (Light light in lights) {
					if (light.Static == isStatic) {
						Vector3 pos = light.LastUpdatePoint;
						pos.Z = -pos.Z;

						GL.ActiveTexture(TextureUnit.Texture0);
						GL.BindTexture(TextureTarget.TextureCubeMap, light.DepthTextureBuffer);
						GL.ActiveTexture(TextureUnit.Texture1);
						if (light.Texture != null && light.Texture.State == Texture.LoadingState.Complete) {
							light.Texture.Bind();
						} else {
							Texture.BindEmpty();
						}
						Caps.CheckErrors();

						shader.LightColor = light.Color;
						shader.LightRange = light.Range;
						shader.LightPos = pos;
						shader.LightRotation = light.TextureAngle;

						shader.Bind();
						Caps.CheckErrors();
						GL.DrawElements(PrimitiveType.Triangles, shadowGroup.indexCount, DrawElementsType.UnsignedShort, 0);
						Caps.CheckErrors();
						shader.Unbind();
					}
				}

				// Unbinding data
				GL.Disable(EnableCap.Blend);
				GL.Enable(EnableCap.DepthTest);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
				Caps.CheckErrors();

				// Unbinding textures
				GL.ActiveTexture(TextureUnit.Texture1);
				GL.BindTexture(TextureTarget.Texture2D, 0);
				GL.Disable(EnableCap.Texture2D);
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.TextureCubeMap, 0);
				GL.Disable(EnableCap.TextureCubeMap);

				// Restoring matrices
				ShaderSystem.CameraMatrix = camMat;
				ShaderSystem.EntityMatrix = entMat;
				ShaderSystem.ProjectionMatrix = prjMat;
				ShaderSystem.TextureMatrix = texMat;
			}

			/// <summary>
			/// Single rendering group
			/// </summary>
			class RenderGroup {

				/// <summary>
				/// Drawing buffers
				/// </summary>
				public int vertexBuffer, normalBuffer, texCoordBuffer, lightmapBuffer, indexBuffer;

				/// <summary>
				/// Index count
				/// </summary>
				public int indexCount;

				/// <summary>
				/// Texture to render
				/// </summary>
				public Texture tex;

				/// <summary>
				/// Setting data
				/// </summary>
				public void SetData(RenderPlane[] planes) {

					// Creating arrays
					float[] coords = new float[planes.Length * 4 * 3];
					float[] normals = new float[coords.Length];
					float[] texCoords = new float[planes.Length * 4 * 2];
					float[] lightCoords = new float[texCoords.Length];
					ushort[] indices = new ushort[planes.Length * 6];

					// Feeding data
					int coordPointer = 0, texPointer = 0, indexPointer = 0;
					for (int pn = 0; pn < planes.Length; pn++) {
						RenderPlane plane = planes[pn];
						Vector3 normal = -Vector3.Cross(plane.Coords[1] - plane.Coords[0], plane.Coords[1] - plane.Coords[2]).Normalized();
						for (int i = 0; i < 4; i++) {

							// Verts
							coords[coordPointer + 0] = plane.Coords[i].X;
							coords[coordPointer + 1] = plane.Coords[i].Y;
							coords[coordPointer + 2] = -plane.Coords[i].Z;

							// Normals
							normals[coordPointer + 0] = normal.X;
							normals[coordPointer + 1] = normal.Y;
							normals[coordPointer + 2] = -normal.Z;

							// Texture coords
							texCoords[texPointer + 0] = plane.TexCoords[i].X;
							texCoords[texPointer + 1] = plane.TexCoords[i].Y;
							lightCoords[texPointer + 0] = plane.LightCoords[i].X;
							lightCoords[texPointer + 1] = plane.LightCoords[i].Y;

							// Incrementing
							coordPointer += 3;
							texPointer += 2;
						}

						// Adding indices
						int sdx = pn * 4;
						indices[indexPointer + 0] = (ushort)(sdx + 0);
						indices[indexPointer + 1] = (ushort)(sdx + 2);
						indices[indexPointer + 2] = (ushort)(sdx + 1);
						indices[indexPointer + 3] = (ushort)(sdx + 1);
						indices[indexPointer + 4] = (ushort)(sdx + 2);
						indices[indexPointer + 5] = (ushort)(sdx + 3);
						indexPointer += 6;
					}
					indexCount = indices.Length;

					// Preparing buffers
					if (vertexBuffer == 0 || !GL.IsBuffer(vertexBuffer)) {
						vertexBuffer = GL.GenBuffer();
					}
					if (normalBuffer == 0 || !GL.IsBuffer(normalBuffer)) {
						normalBuffer = GL.GenBuffer();
					}
					if (texCoordBuffer == 0 || !GL.IsBuffer(texCoordBuffer)) {
						texCoordBuffer = GL.GenBuffer();
					}
					if (lightmapBuffer == 0 || !GL.IsBuffer(lightmapBuffer)) {
						lightmapBuffer = GL.GenBuffer();
					}
					if (indexBuffer == 0 || !GL.IsBuffer(indexBuffer)) {
						indexBuffer = GL.GenBuffer();
					}

					// Sending data
					GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
					GL.BufferData(BufferTarget.ArrayBuffer, coords.Length * 4, coords, BufferUsageHint.DynamicDraw);
					GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
					GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * 4, normals, BufferUsageHint.DynamicDraw);
					GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordBuffer);
					GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * 4, texCoords, BufferUsageHint.DynamicDraw);
					GL.BindBuffer(BufferTarget.ArrayBuffer, lightmapBuffer);
					GL.BufferData(BufferTarget.ArrayBuffer, lightCoords.Length * 4, lightCoords, BufferUsageHint.DynamicDraw);
					GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
					GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * 2, indices, BufferUsageHint.DynamicDraw);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

				}

				/// <summary>
				/// Rendering 
				/// </summary>
				public void Render() {
					if (tex != null) {
						tex.Bind();
					} else {
						Texture.BindEmpty();
					}
					if (Caps.ShaderPipeline) {

						Caps.CheckErrors();
						MapShader shader = MapShader.Shader;
						GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
						GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
						if (shader.NormalsBufferLocation > -1) {
							GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
							GL.VertexAttribPointer(shader.NormalsBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
						}
						GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordBuffer);
						GL.VertexAttribPointer(shader.TexCoordBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
						GL.BindBuffer(BufferTarget.ArrayBuffer, lightmapBuffer);
						GL.VertexAttribPointer(shader.LightTexCoordBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
						GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
						Caps.CheckErrors();
						GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedShort, 0);
						Caps.CheckErrors();
						GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
						GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
						Caps.CheckErrors();

					} else {
						/*
						GL.MatrixMode(MatrixMode.Modelview);
						GL.PushMatrix();
						GL.MultMatrix(ref matrix); int num = 0;

						GL.BindBuffer(BufferTarget.ArrayBuffer, rg.VertexBuffer);
						GL.EnableClientState(ArrayCap.VertexArray);
						GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

						GL.ClientActiveTexture(TextureUnit.Texture0);
						GL.DisableClientState(ArrayCap.TextureCoordArray);
						GL.ClientActiveTexture(TextureUnit.Texture1);
						GL.DisableClientState(ArrayCap.TextureCoordArray);
						GL.ClientActiveTexture(TextureUnit.Texture2);
						GL.DisableClientState(ArrayCap.TextureCoordArray);


						GL.BindBuffer(BufferTarget.ElementArrayBuffer, rg.IndexBuffer);
						GL.DrawElements(PrimitiveType.Points, rg.IndexCount, DrawElementsType.UnsignedShort, 0);


						GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
						GL.DisableClientState(ArrayCap.VertexArray);

						GL.PopMatrix();
						 */
					}
					Engine.Current.drawCalls++;
					Caps.CheckErrors();
				}

				/// <summary>
				/// Rendering shadow pass
				/// </summary>
				public void RenderShadow() {
					MapShadowShader shader = MapShadowShader.Shader;
					GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
					GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
					GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedShort, 0);
					GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				}

			}

			/// <summary>
			/// Rendering plane
			/// </summary>
			class RenderPlane {

				/// <summary>
				/// Vertex coords
				/// </summary>
				public Vector3[] Coords;

				/// <summary>
				/// Texture coords
				/// </summary>
				public Vector2[] TexCoords;

				/// <summary>
				/// Lightmap coords
				/// </summary>
				public Vector2[] LightCoords;

			}
		}

		/// <summary>
		/// Door opening type
		/// </summary>
		public enum DoorType : int {

			// Single-segment sliding door
			SlideLeft		= 0,
			SlideUp			= 1,
			SlideRight		= 2,
			SlideDown		= 3,

			// Two-segment sliding doors
			DoubleSlideHorizontal	= 10,
			DoubleSlideVertical		= 11,

			// Hinge
			HingeLeft		= 20,
			HingeUp			= 21,
			HingeRight		= 22,
			HingeDown		= 23,
			
			// Two-segment hinge
			DoubleHingeHorizontal	= 30,
			DoubleHingeVertical		= 31
		}
	}
}
