using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Data.Editor.Attributes {

	/// <summary>
	/// Constrain field in inspector
	/// </summary>
	public class InspectorRangeAttribute : Attribute {

		/// <summary>
		/// Minimal value
		/// </summary>
		public object Min {
			get;
			private set;
		}

		/// <summary>
		/// Maximal value
		/// </summary>
		public object Max {
			get;
			private set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public InspectorRangeAttribute(object min, object max) {
			Min = min;
			Max = max;
		}

	}
}
