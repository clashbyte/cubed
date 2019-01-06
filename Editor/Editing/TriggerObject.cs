using Cubed.Components.Rendering;
using Cubed.Data.Editor.Attributes;
using Cubed.Editing.Gizmos;
using Cubed.Graphics;
using Cubed.Prefabs;
using Cubed.World;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubed.Editing {

	/// <summary>
	/// Abstract class for all zone triggers
	/// </summary>
	public abstract class TriggerObject : EditableObject {

		/// <summary>
		/// Trigger size
		/// </summary>
		[InspectorName("Scale")]
		public Vector3 Size {
			get {
				return (Prefab as TriggerPrefab).Bounds;
			}
			set {
				(Prefab as TriggerPrefab).Bounds = value;
			}
		}


		/// <summary>
		/// Range gizmos
		/// </summary>
		public override Gizmo[] ControlGizmos {
			get {
				List<Gizmo> gizmos = new List<Gizmo>() {
					boundGizmo
				};

				return gizmos.ToArray();
			}
		}

		/// <summary>
		/// Gizmo accent color
		/// </summary>
		protected virtual Color GizmoColor {
			get {
				return Color.White;
			}
		}

		/// <summary>
		/// Internal gizmo icon
		/// </summary>
		protected virtual Texture GizmoIcon {
			get {
				return null;
			}
		}

		/// <summary>
		/// Is trigger selected
		/// </summary>
		bool selected;

		/// <summary>
		/// Bound gizmo
		/// </summary>
		ZoneGizmo boundGizmo;

		/// <summary>
		/// Creating light
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Create(Scene scene) {
			Texture texture = GizmoIcon;
			if (Gizmo == null) {
				Gizmo = new Entity();
				Gizmo.Parent = Prefab;
				Gizmo.AddComponent(new SpriteComponent() {
					AffectedByLight = false,
					Scale = Vector2.One * 0.3f,
					Texture = GizmoIcon
				});
				Gizmo.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.3f,
					WireColor = Color.FromArgb(100, GizmoColor),
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
					Texture = texture
				});
				SelectedGizmo.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.3f,
					WireColor = GizmoColor,
					WireWidth = 1.5f
				});
				SelectedGizmo.LocalPosition = Vector3.Zero;
				scene.Entities.Add(SelectedGizmo);
			}
			BoundPosition = Vector3.Zero;
			BoundSize = Vector3.One * 0.3f;
			boundGizmo = new ZoneGizmo(GizmoColor, 0.05f) {
				Filter = ZoneFilter
			};

			Prefab.Assign(scene);
			UpdateBounds();
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
			Gizmo.Visible = true;
			SelectedGizmo.Visible = true;
			UpdateBounds();
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
		/// Handling bounds
		/// </summary>
		void UpdateBounds() {
			boundGizmo.BoxSize = (Prefab as TriggerPrefab).Bounds;
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
			Prefab.Position += pos;
			Size = size;
			UpdateBounds();
			return new Tuple<Vector3, Vector3>(Vector3.Zero, size);
		}
	}
}
