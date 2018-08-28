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
		/// Gizmo maximal position
		/// </summary>
		public Vector3 MaxPosition {
			get {
				return maxPosition;			
			}
			set {
				maxPosition = value;
			}
		}

		/// <summary>
		/// Gizmo minimal position
		/// </summary>
		public Vector3 MinPosition {
			get {
				return minPosition;
			}
			set {
				minPosition = value;
			}
		}

		/// <summary>
		/// Minimal value
		/// </summary>
		public Vector3 MinValue {
			get {
				return maxValue;
			}
			set {
				minValue = value;
			}
		}

		/// <summary>
		/// Maximal value
		/// </summary>
		public Vector3 MaxValue {
			get {
				return maxValue;
			}
			set {
				maxValue = value;
			}
		}

		/// <summary>
		/// Minimal and maximal value
		/// </summary>
		protected Vector3 minValue = Vector3.One * float.MinValue, maxValue = Vector3.One * float.MaxValue;
		
		/// <summary>
		/// Minimal and maximal position
		/// </summary>
		protected Vector3 minPosition = Vector3.One * float.MinValue, maxPosition = Vector3.One * float.MaxValue;

		/// <summary>
		/// Axis constraints
		/// </summary>
		public enum AxisConstraints {
			All,
			X,
			Y,
			Z,
			XZ,
			XY,
			ZY
		}
	}
}
