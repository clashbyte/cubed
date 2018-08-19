using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.World;

namespace Cubed.Prefabs {

	/// <summary>
	/// Map light entity
	/// </summary>
	public class MapLight : GamePrefab {

		/// <summary>
		/// Light entity
		/// </summary>
		Light light;

		/// <summary>
		/// Constructor
		/// </summary>
		public MapLight() {
			light = new Light() {
				Color = Color.White,
				Range = 5
			};
			light.Parent = this;
		}

		/// <summary>
		/// Assigning to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Assign(Scene scene) {
			scene.Entities.Add(this);
			scene.Entities.Add(light);
		}

		/// <summary>
		/// Unassigning from scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Unassign(Scene scene) {
			scene.Entities.Remove(this);
			scene.Entities.Remove(light);
		}
	}
}
