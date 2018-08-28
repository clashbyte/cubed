using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Cubed.World {
	
	/// <summary>
	/// Fog parameters
	/// </summary>
	public class Fog {

		/// <summary>
		/// Color
		/// </summary>
		public Color Color {
			get;
			set;
		}

		/// <summary>
		/// Near clipping plane
		/// </summary>
		public float Near {
			get;
			set;
		}

		/// <summary>
		/// Far clipping plane
		/// </summary>
		public float Far {
			get;
			set;
		}

	}
}
