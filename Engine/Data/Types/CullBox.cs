using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Cubed.Data.Types {

	/// <summary>
	/// Culling box
	/// </summary>
	public class CullBox {

		/// <summary>
		/// Minimal box coords
		/// </summary>
		public Vector3 Min {
			get;
			set;
		}

		/// <summary>
		/// Maximal box coords
		/// </summary>
		public Vector3 Max {
			get;
			set;
		}

		/// <summary>
		/// Converting to sphere
		/// </summary>
		/// <returns>Sphere</returns>
		public CullSphere ToSphere() {
			return new CullSphere() {
				Position = (Max + Min) / 2f,
				Radius = (Max - Min).Length / 2f
			};
		}

		/// <summary>
		/// Collecting bounding boxes
		/// </summary>
		/// <param name="boxes">Array of child BBs</param>
		/// <returns>One box containing each other</returns>
		public static CullBox FromBoxes(CullBox[] boxes) {
			Vector3 min = Vector3.One * float.MaxValue, max = Vector3.One * float.MinValue;
			foreach (CullBox bb in boxes) {
				if (bb.Min.X < min.X) min.X = bb.Min.X;
				if (bb.Min.Y < min.Y) min.Y = bb.Min.Y;
				if (bb.Min.Z < min.Z) min.Z = bb.Min.Z;
				if (bb.Max.X > max.X) max.X = bb.Max.X;
				if (bb.Max.Y > max.Y) max.Y = bb.Max.Y;
				if (bb.Max.Z > max.Z) max.Z = bb.Max.Z;
			}
			return new CullBox() {
				Min = min,
				Max = max
			};
		}
	}
}
