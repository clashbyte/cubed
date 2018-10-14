using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.World;
using OpenTK;

namespace Cubed.Editing.Gizmos {
	
	/// <summary>
	/// Gizmo class for interactions
	/// </summary>
	public abstract class Gizmo : Entity {

		/// <summary>
		/// Assigning to scene
		/// </summary>
		public abstract void Assign(Scene scene, Entity parent);

		/// <summary>
		/// Removing from scene
		/// </summary>
		public abstract void Unassign(Scene scene);

		/// <summary>
		/// Checking for intersection with picking
		/// </summary>
		/// <param name="rayPos">Position</param>
		/// <param name="rayDir">Direction</param>
		/// <returns>True if ray hit gizmo</returns>
		public abstract bool Hit(Vector3 rayPos, Vector3 rayDir, out float hitDist);

		/// <summary>
		/// Starting iteraction with this gizmo
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		public abstract void StartIteraction(Vector3 rayPos, Vector3 rayDir, bool ctrl);

		/// <summary>
		/// Updating iteraction
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		public abstract void UpdateIteraction(Vector3 rayPos, Vector3 rayDir);

		/// <summary>
		/// Stopping iteraction
		/// </summary>
		/// <param name="rayPos"></param>
		/// <param name="rayDir"></param>
		public abstract void EndIteraction(Vector3 rayPos, Vector3 rayDir);

		/// <summary>
		/// Updating gizmo
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="tween">Tween value</param>
		/// <param name="cursor">Cursor</param>
		public virtual void Update(Vector3 rayPos, Vector3 rayDir, float tween, bool current, out System.Windows.Forms.Cursor cursor) {
			cursor = System.Windows.Forms.Cursors.Default;
		}
	}
}
