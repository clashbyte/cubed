using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using Cubed.Data.Editor.Attributes;
using Cubed.Graphics;
using Cubed.World;
using OpenTK;

namespace Cubed.Editing {

	/// <summary>
	/// Map light entity
	/// </summary>
	[InspectorIcon("Light")]
	[InspectorName("Light")]
	[InspectorDescription("LightDesc")]
	[TargetPrefab(typeof(Prefabs.MapLight))]
	public class MapLight : EditableObject {

		/// <summary>
		/// Light color
		/// </summary>
		public Color Color {
			get {
				return (Prefab as Prefabs.MapLight).Color;
			}
			set {
				(Prefab as Prefabs.MapLight).Color = value;
				System.Drawing.Color clr = (Prefab as Prefabs.MapLight).Color;
				if (Gizmo != null) {
					
				}
				if (SelectedGizmo != null) {
					
				}
			}
		}

		/// <summary>
		/// Light range
		/// </summary>
		[InspectorRange(0.1f, 32f)]
		public float Range {
			get {
				return (Prefab as Prefabs.MapLight).Range;
			}
			set {
				(Prefab as Prefabs.MapLight).Range = value;
				RebuildRing((Prefab as Prefabs.MapLight).Range);
			}
		}

		/// <summary>
		/// Texture
		/// </summary>
		public Texture Texture {
			get {
				return (Prefab as Prefabs.MapLight).Texture;
			}
			set {
				(Prefab as Prefabs.MapLight).Texture = value;
			}
		}

		/// <summary>
		/// Texture angle
		/// </summary>
		[InspectorRange(0, 360)]
		public float TextureAngle {
			get {
				return (Prefab as Prefabs.MapLight).TextureAngle;
			}
			set {
				(Prefab as Prefabs.MapLight).TextureAngle = value;
			}
		}

		/// <summary>
		/// Current texture
		/// </summary>
		static Texture gizmoIcon;

		/// <summary>
		/// Is light selected
		/// </summary>
		bool selected;

		/// <summary>
		/// Creating light
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Create(Scene scene) {
			if (Gizmo == null) {
				if (gizmoIcon != null && gizmoIcon.IsReleased) {
					gizmoIcon = null;
				}
				if (gizmoIcon == null) {
					gizmoIcon = new Texture(Forms.Resources.InspectorIcons.Light) {
						Filtering = Texture.FilterMode.Enabled
					};
				}
				Gizmo = new Entity();
				Gizmo.Parent = Prefab;
				Gizmo.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.3f,
					WireColor = Color.FromArgb(150, Color.Yellow),
					WireWidth = 1f
				});
				Gizmo.AddComponent(new SpriteComponent() {
					AffectedByLight = false,
					Facing = SpriteComponent.FacingMode.XY,
					Offset = Vector2.Zero,
					Scale = Vector2.One * 0.3f,
					Texture = gizmoIcon
				});
				Gizmo.LocalPosition = Vector3.Zero;
				scene.Entities.Add(Gizmo);
			}
			if (SelectedGizmo == null) {
				SelectedGizmo = new Entity();
				SelectedGizmo.Visible = false;
				SelectedGizmo.Parent = Prefab;
				SelectedGizmo.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.3f,
					WireColor = Color.Yellow,
					WireWidth = 1.5f
				});
				SelectedGizmo.AddComponent(new LineComponent() {
					Mode = LineComponent.LineType.Loop,
					WireColor = Color.Yellow,
					WireWidth = 1f
				});
				SelectedGizmo.LocalPosition = Vector3.Zero;
				scene.Entities.Add(SelectedGizmo);
			}
			BoundPosition = Vector3.Zero;
			BoundSize = Vector3.One * 0.3f;
			RebuildRing(((Cubed.Prefabs.MapLight)Prefab).Range);
			Prefab.Assign(scene);
		}

		/// <summary>
		/// Destroying
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Destroy(Scene scene) {
			if (Prefab != null) {
				Prefab.Unassign(scene);
			}
			if (Gizmo != null) {
				scene.Entities.Remove(Gizmo);
			}
			if (SelectedGizmo != null) {
				scene.Entities.Remove(SelectedGizmo);
			}
		}

		/// <summary>
		/// Selecting object
		/// </summary>
		/// <param name="scene"></param>
		public override void Select(Scene scene) {
			selected = true;
			SelectedGizmo.Visible = true;
		}

		/// <summary>
		/// Deselecting object
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Deselect(Scene scene) {
			selected = false;
			SelectedGizmo.Visible = false;
		}

		/// <summary>
		/// Entering play mode
		/// </summary>
		public override void StartPlayMode(Scene scene) {
			Gizmo.Visible = SelectedGizmo.Visible = false;
		}

		/// <summary>
		/// Exiting play mode
		/// </summary>
		public override void StopPlayMode(Scene scene) {
			Gizmo.Visible = true;
			SelectedGizmo.Visible = selected;
		}

		/// <summary>
		/// Rebuilding range ring
		/// </summary>
		/// <param name="range">Light range</param>
		void RebuildRing(float range) {
			int count = (int)Math.Ceiling(range * 8);
			float dif = MathHelper.TwoPi / (float)count;
			Vector3[] verts = new Vector3[count];
			for (int i = 0; i < count; i++) {
				float r = (float)i * dif;
				verts[i] = new Vector3((float)Math.Sin(r) * range, 0, (float)Math.Cos(r) * range);
			}
			SelectedGizmo.GetComponent<LineComponent>().Vertices = verts;
		}
	}
}
