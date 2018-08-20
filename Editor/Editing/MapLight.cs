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
	public class MapLight : EditableObject {

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
			if (Prefab == null) {
				Prefab = new Prefabs.MapLight();
			}
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
					WireColor = Color.Yellow,
					WireWidth = 1f
				});
				Gizmo.AddComponent(new LineComponent() {
					Vertices = new Vector3[] {
						Vector3.Zero,
						Vector3.One * 1
					},
					WireColor = Color.Yellow,
					WireWidth = 1f
				});
				Gizmo.AddComponent(new SpriteComponent() {
					AffectedByLight = false,
					Facing = SpriteComponent.FacingMode.XY,
					Offset = Vector2.Zero,
					Scale = Vector2.One * 0.3f,
					Texture = gizmoIcon
				});
				scene.Entities.Add(Gizmo);
			}

			BoundPosition = Vector3.Zero;
			BoundSize = Vector3.One * 0.3f;
			Prefab.Assign(scene);
		}

		/// <summary>
		/// Destroying
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Destroy(Scene scene) {
			if (Prefab != null) {
				Prefab.Unassign(scene);
				Prefab = null;
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
			//throw new NotImplementedException();
		}

		/// <summary>
		/// Deselecting object
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Deselect(Scene scene) {
			//throw new NotImplementedException();
		}
	}
}
