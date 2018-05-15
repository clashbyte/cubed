using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Components {

	/// <summary>
	/// Updatable late component interface
	/// </summary>
	interface ILateUpdatable {

		/// <summary>
		/// Update component logic
		/// </summary>
		void LateUpdate();

	}
}
