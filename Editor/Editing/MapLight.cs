using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using Cubed.Data.Editor.Attributes;
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
		/// Creating light
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Create(Scene scene) {
			if (Prefab == null) {
				Prefab = new Prefabs.MapLight();
			}
			if (Gizmo == null) {
				Gizmo = new Entity();
				Gizmo.Parent = Prefab;
				Gizmo.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.3f,
					WireColor = Color.MistyRose,
					WireWidth = 2f
				});
				scene.Entities.Add(Gizmo);
			}


			Prefab.Assign(scene);
		}

		public override void Destroy(Scene scene) {
			if (Prefab != null) {
				Prefab.Unassign(scene);
				Prefab = null;
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
