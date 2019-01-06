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
	/// Player spawn entity
	/// </summary>
	[InspectorIcon("Player")]
	[InspectorName("Player")]
	[InspectorDescription("PlayerDesc")]
	[TargetPrefab(typeof(Prefabs.PlayerStart))]
	public class PlayerSpawn : EditableObject {

		/// <summary>
		/// Current texture
		/// </summary>
		static Texture gizmoIcon;

		/// <summary>
		/// Looking angle
		/// </summary>
		public float Angle {
			get {
				return (Prefab as Prefabs.PlayerStart).Angle;
			}
			set {
				(Prefab as Prefabs.PlayerStart).Angle = value;
				if (angleGizmo != null) {
					angleGizmo.Angle = value;
				}
			}
		}
		
		/// <summary>
		/// Is light selected
		/// </summary>
		bool selected;

		/// <summary>
		/// Gizmo for angle
		/// </summary>
		AngleGizmo angleGizmo;

		/// <summary>
		/// Controls for angle
		/// </summary>
		public override Gizmo[] ControlGizmos {
			get {
				return new Gizmo[] {
					angleGizmo	
				};
			}
		}

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
					gizmoIcon = new Texture(Forms.Resources.InspectorIcons.Player) {
						Filtering = Texture.FilterMode.Enabled
					};
				}
				Gizmo = new Entity();
				Gizmo.Parent = Prefab;
				Gizmo.AddComponent(new WireCubeComponent() {
					Size = new Vector3(0.3f, 0.75f, 0.3f),
					WireColor = Color.FromArgb(150, Color.LimeGreen),
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
					Size = new Vector3(0.3f, 0.75f, 0.3f),
					WireColor = Color.LimeGreen,
					WireWidth = 1.5f
				});
				SelectedGizmo.LocalPosition = Vector3.Zero;
				scene.Entities.Add(SelectedGizmo);
			}
			BoundPosition = Vector3.Zero;
			BoundSize = new Vector3(0.3f, 0.75f, 0.3f);

			// Adding scene gizmos
			if (angleGizmo == null) {
				angleGizmo = new AngleGizmo(Color.LimeGreen, 0.15f, 0.32f) {
					Parent = Prefab,
					Position = Prefab.Position,
					Filter = AngleFilter
				};
				angleGizmo.Angle = (Prefab as Prefabs.PlayerStart).Angle;
			}
			Prefab.Assign(scene);
		}

		/// <summary>
		/// Destroying
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Destroy(Scene scene) {
			if (Prefab != null) {
				Prefab.Unassign(scene);
				Prefab.Destroy();
			}
			if (Gizmo != null) {
				scene.Entities.Remove(Gizmo);
				Gizmo.Destroy();
			}
			if (SelectedGizmo != null) {
				scene.Entities.Remove(SelectedGizmo);
				SelectedGizmo.Destroy();
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
		/// Callback for gizmo
		/// </summary>
		/// <param name="gizmo">Angle gizmo</param>
		/// <param name="target">Target angle</param>
		/// <returns>Computed angle</returns>
		float AngleFilter(AngleGizmo gizmo, float target) {
			return (Prefab as Prefabs.PlayerStart).Angle = target;
		}
	}
}
