using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubed.UI.Basic {


	public class ToggleButton : Button {

		/// <summary>
		/// Current button state
		/// </summary>
		public bool State {
			get;
			private set;
		}

		/// <summary>
		/// Flag to draw checked buttons
		/// </summary>
		protected override bool DrawInactiveAsActive {
			get {
				return State;
			}
		}

		/// <summary>
		/// Handling action
		/// </summary>
		protected override void OnAction() {
			State = !State;
			base.OnAction();
		}

	}
}
