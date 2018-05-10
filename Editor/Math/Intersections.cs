using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Cubed.Math {

	/// <summary>
	/// Intersections class
	/// </summary>
	public class Intersections {

		public static bool RayPlane(Vector3 plane, Vector3 normal, Vector3 ray, Vector3 direction, out Vector3 hit) {
			
			// Calculating delta
			float d = Vector3.Dot(normal, plane);
			if (System.Math.Abs(Vector3.Dot(normal, direction)) < 0.0001f) {
				hit = Vector3.Zero;
				return false;
			}
			float x = (d - Vector3.Dot(normal, ray)) / Vector3.Dot(normal, direction);
			hit = ray + direction.Normalized() * x;
			return true;
		}
	}
}
