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
			get {
				return check;
			}
			set {
				check = value;
				base.OnAction();
			}
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
		/// Flag for enabled state
		/// </summary>
		bool check;

		/// <summary>
		/// Handling action
		/// </summary>
		protected override void OnAction() {
			State = !State;
			base.OnAction();
		}

	}
}
