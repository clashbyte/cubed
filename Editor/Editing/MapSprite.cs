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
	/// Map sprite entity
	/// </summary>
	[InspectorIcon("Sprite")]
	[InspectorName("Sprite")]
	[InspectorDescription("SpriteDesc")]
	[TargetPrefab(typeof(Prefabs.MapSprite))]
	public class MapSprite : EditableObject {

		/// <summary>
		/// Texture
		/// </summary>
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
		/// Affected by light
		/// </summary>
		public bool AffectedByLight {
			get {
				return (Prefab as Prefabs.MapSprite).AffectedByLight;
			}
			set {
				(Prefab as Prefabs.MapSprite).AffectedByLight = value;
			}
		}

		public Vector2 Offset {
			get {
				return (Prefab as Prefabs.MapSprite).Offset;
			}
			set {
				(Prefab as Prefabs.MapSprite).Offset = value;
			}
		}

		public Vector2 Scale {
			get {
				return (Prefab as Prefabs.MapSprite).Scale;
			}
			set {
				(Prefab as Prefabs.MapSprite).Scale = value;
			}
		}

		public Color Tint {
			get {
				return (Prefab as Prefabs.MapSprite).Tint;
			}
			set {
				(Prefab as Prefabs.MapSprite).Tint = value;
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
				SelectedGizmo.LocalPosition = Vector3.Zero;
				scene.Entities.Add(SelectedGizmo);
			}
			BoundPosition = Vector3.Zero;
			BoundSize = Vector3.One * 0.3f;
			Prefab.Assign(scene);
			Texture = (Prefab as Prefabs.MapSprite).Texture;
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
			Gizmo.Visible = false;
			SelectedGizmo.Visible = true;
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
	}
}
