using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Maths {
	public static class Lerps {

		/// <summary>
		/// Lerp from one float to another
		/// </summary>
		/// <param name="a">First</param>
		/// <param name="b">Second</param>
		/// <param name="dist">Distance</param>
		/// <returns>Lerped value</returns>
		public static float Lerp(float a, float b, float dist) {
			return a + (b - a) * Math.Max(Math.Min(dist, 1), 0);
		}

	}
}
