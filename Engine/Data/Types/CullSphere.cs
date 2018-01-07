using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Cubed.Data.Types {
	
	/// <summary>
	/// Culling sphere
	/// </summary>
	public class CullSphere {

		/// <summary>
		/// Sphere location
		/// </summary>
		public Vector3 Position {
			get;
			set;
		}

		/// <summary>
		/// Sphere radius
		/// </summary>
		public float Radius {
			get;
			set;
		}

	}
}
