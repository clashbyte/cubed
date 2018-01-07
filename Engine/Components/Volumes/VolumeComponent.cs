using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Cubed.Components.Volumes {

	/// <summary>
	/// Volume component for intersection test
	/// </summary>
	public abstract class VolumeComponent : EntityComponent, IRenderable {

		/// <summary>
		/// Test for ray intersection
		/// </summary>
		internal abstract bool RayHit(Vector3 rayPos, Vector3 rayDir, float rayLength, out Vector3 pos, out Vector3 normal);

		/// <summary>
		/// Component rendering
		/// </summary>
		void IRenderable.Render() {
			//if (GameEngine.DebugMode) {
			//	RenderDebug();
			//}
		}

		/// <summary>
		/// Debug rendering
		/// </summary>
		protected virtual void RenderDebug() { }
	}
}
