using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.World;

namespace Cubed.Prefabs {

	/// <summary>
	/// Game prefab
	/// </summary>
	public abstract class GamePrefab : Entity {

		/// <summary>
		/// Assigning to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public abstract void Assign(Scene scene);

		/// <summary>
		/// Removing from scene
		/// </summary>
		/// <param name="scene">Scene to create</param>
		public abstract void Unassign(Scene scene);

	}
}
