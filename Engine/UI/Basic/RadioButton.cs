using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.UI.Misc;

namespace Cubed.UI.Basic {


	public class RadioButton : Button {

		/// <summary>
		/// Current radio group
		/// </summary>
		public RadioGroup Group {
			get {
				return group;
			}
			set {
				if (group != value) {
					if (group != null) {
						group.Unregister(this);
					}
					group = value;
					if (group != null) {
						group.Register(this);
					}
				}
			}
		}

		/// <summary>
		/// Current button state
		/// </summary>
		public bool State {
			get {
				if (group != null && group.Current == this) {
					return true;
				}
				return false;
			}
			set {
				if (group != null) {
					group.Current = value ? this : null;
				}
			}
		}

		/// <summary>
		/// Current radio group
		/// </summary>
		RadioGroup group;

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
			if (!State) {
				State = true;
			}
		}

	}
}
