using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using Cubed.Data.Editor.Attributes;
using Cubed.Editing.Gizmos;
using Cubed.Graphics;
using Cubed.World;
using OpenTK;

namespace Cubed.Editing {

	/// <summary>
	/// Map sprite entity
	/// </summary>
	[InspectorIcon("Sprite")]
	[InspectorName("Sprite")]
	[InspectorDescription("SpriteDesc")]
	[TargetPrefab(typeof(Prefabs.MapSprite))]
	[InspectorSection(0, "VisualGroup", "VisualGroup")]
	[InspectorSection(1, "BrokenGroup", "BrokenGroup")]
	[InspectorSection(2, "SizeGroup", "SizeGroup")]
	[InspectorSection(3, "BoundGroup", "BoundGroup")]
	public class MapSprite : EditableObject {

		public override Gizmo[] ControlGizmos {
			get {
				return new Gizmo[] {
					colliderGizmo	
				};
			}
		}

		/// <summary>
		/// Texture
		/// </summary>
		[InspectorSection(0)]
		[InspectorName("Texture")]
		public Texture Texture {
			get {
				return (Prefab as Prefabs.MapSprite).Texture;
			}
			set {
				(Prefab as Prefabs.MapSprite).Texture = value;
				Gizmo.GetComponent<SpriteComponent>().Enabled = SelectedGizmo.GetComponent<SpriteComponent>().Enabled = (Prefab as Prefabs.MapSprite).Texture == null;
				Prefab.GetComponent<SpriteComponent>().Enabled = (Prefab as Prefabs.MapSprite).Texture != null;
			}
		}

		/// <summary>
		/// Facing
		/// </summary>
		[InspectorSection(2)]
		[InspectorName("Facing")]
		public Prefabs.MapSprite.FacingMode Facing {
			get {
				return (Prefab as Prefabs.MapSprite).Facing;
			}
			set {
				(Prefab as Prefabs.MapSprite).Facing = value;
			}
		}

		/// <summary>
		/// Blending
		/// </summary>
		[InspectorSection(0)]
		[InspectorName("Blending")]
		public Prefabs.MapSprite.BlendingMode Blending {
			get {
				return (Prefab as Prefabs.MapSprite).Blending;
			}
			set {
				(Prefab as Prefabs.MapSprite).Blending = value;
			}
		}

		/// <summary>
		/// Affected by light
		/// </summary>
		[InspectorSection(0)]
		[InspectorName("AffectedByLight")]
		public bool AffectedByLight {
			get {
				return (Prefab as Prefabs.MapSprite).AffectedByLight;
			}
			set {
				(Prefab as Prefabs.MapSprite).AffectedByLight = value;
			}
		}

		/// <summary>
		/// Affected by fog
		/// </summary>
		[InspectorSection(0)]
		[InspectorName("AffectedByFog")]
		public bool AffectedByFog {
			get {
				return (Prefab as Prefabs.MapSprite).AffectedByFog;
			}
			set {
				(Prefab as Prefabs.MapSprite).AffectedByFog = value;
			}
		}

		/// <summary>
		/// Sprite offset
		/// </summary>
		[InspectorSection(2)]
		[InspectorName("Offset")]
		public Vector2 Offset {
			get {
				return (Prefab as Prefabs.MapSprite).Offset;
			}
			set {
				(Prefab as Prefabs.MapSprite).Offset = value;
			}
		}

		/// <summary>
		/// Sprite scale
		/// </summary>
		[InspectorSection(2)]
		[InspectorName("Scale")]
		public Vector2 Scale {
			get {
				return (Prefab as Prefabs.MapSprite).Scale;
			}
			set {
				(Prefab as Prefabs.MapSprite).Scale = value;
			}
		}

		/// <summary>
		/// Angle
		/// </summary>
		[InspectorSection(2)]
		[InspectorName("Angle")]
		public float Angle {
			get {
				return (Prefab).Angles.Y;
			}
			set {
				Prefab.Angles = Vector3.UnitY * value;
			}
		}

		[InspectorSection(0)]
		[InspectorName("Tint")]
		public Color Tint {
			get {
				return (Prefab as Prefabs.MapSprite).Tint;
			}
			set {
				(Prefab as Prefabs.MapSprite).Tint = value;
			}
		}

		[InspectorSection(3)]
		[InspectorName("Solid")]
		public bool Solid {
			get {
				return (Prefab as Prefabs.MapSprite).Solid;
			}
			set {
				(Prefab as Prefabs.MapSprite).Solid = value;
				UpdateCollider();
			}
		}

		[InspectorSection(3)]
		[InspectorName("Offset")]
		public Vector3 ColliderOffset {
			get {
				return (Prefab as Prefabs.MapSprite).BoundOffset;
			}
			set {
				(Prefab as Prefabs.MapSprite).BoundOffset = value;
				UpdateCollider();
			}
		}

		[InspectorSection(3)]
		[InspectorName("Scale")]
		public Vector3 ColliderSize {
			get {
				return (Prefab as Prefabs.MapSprite).BoundSize;
			}
			set {
				(Prefab as Prefabs.MapSprite).BoundSize = value;
				UpdateCollider();
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
		/// Collider gizmo
		/// </summary>
		ZoneGizmo colliderGizmo;

		/// <summary>
		/// Creating light
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Create(Scene scene) {
			if (gizmoIcon != null && gizmoIcon.IsReleased) {
				gizmoIcon = null;
			}
			if (gizmoIcon == null) {
				gizmoIcon = new Texture(Forms.Resources.InspectorIcons.Sprite) {
					Filtering = Texture.FilterMode.Enabled
				};
			}
			if (Gizmo == null) {
				Gizmo = new Entity();
				Gizmo.Parent = Prefab; 
				Gizmo.AddComponent(new SpriteComponent() {
					AffectedByLight = false,
					Scale = Vector2.One * 0.3f,
					Texture = gizmoIcon
				});
				Gizmo.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.3f,
					WireColor = Color.FromArgb(100, Color.Cyan),
					WireWidth = 1f
				});
				Gizmo.LocalPosition = Vector3.Zero;
				scene.Entities.Add(Gizmo);
			}
			if (SelectedGizmo == null) {
				SelectedGizmo = new Entity();
				SelectedGizmo.Visible = false;
				SelectedGizmo.Parent = Prefab;
				SelectedGizmo.AddComponent(new SpriteComponent() {
					AffectedByLight = false,
					Scale = Vector2.One * 0.3f,
					Texture = gizmoIcon
				});
				SelectedGizmo.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.3f,
					WireColor = Color.Cyan,
					WireWidth = 1.5f
				});
				/*
				SelectedGizmo.AddComponent(colliderBox = new WireCubeComponent() {
					WireColor = Color.Orange,
					WireWidth = 1f
				});
				 */
				SelectedGizmo.LocalPosition = Vector3.Zero;
				scene.Entities.Add(SelectedGizmo);
			}
			BoundPosition = Vector3.Zero;
			BoundSize = Vector3.One * 0.3f;
			colliderGizmo = new ZoneGizmo(Color.Orange, 0.05f) {
				Filter = ZoneFilter
			};

			Prefab.Assign(scene);
			Texture = (Prefab as Prefabs.MapSprite).Texture;
			//UpdateCollider();
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
			Gizmo.Visible = true;
			SelectedGizmo.Visible = true;
			UpdateCollider();
		}

		/// <summary>
		/// Deselecting object
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Deselect(Scene scene) {
			selected = false;
			Gizmo.Visible = true;
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
			Gizmo.Visible = !selected;
			SelectedGizmo.Visible = selected;
		}

		/// <summary>
		/// Handling collider
		/// </summary>
		void UpdateCollider() {
			colliderGizmo.BoxPosition = (Prefab as Prefabs.MapSprite).BoundOffset;
			colliderGizmo.BoxSize = (Prefab as Prefabs.MapSprite).BoundSize;
		}

		/// <summary>
		/// Filter for collider size
		/// </summary>
		Tuple<Vector3, Vector3> ZoneFilter(ZoneGizmo gizmo, Tuple<Vector3, Vector3> input) {
			Vector3 pos = input.Item1;
			Vector3 size = input.Item2;
			size.X = Math.Max(size.X, 0.05f);
			size.Y = Math.Max(size.Y, 0.05f);
			size.Z = Math.Max(size.Z, 0.05f);
			ColliderOffset = pos;
			ColliderSize = size;
			UpdateCollider();
			return new Tuple<Vector3, Vector3>(pos, size);
		}
	}
}
