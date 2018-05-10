using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.UI.Basic;

namespace Cubed.UI.Misc {

	/// <summary>
	/// Group of radio buttons
	/// </summary>
	public class RadioGroup {

		/// <summary>
		/// List of all buttons
		/// </summary>
		List<RadioButton> buttons;

		/// <summary>
		/// Current selected button
		/// </summary>
		RadioButton current;

		/// <summary>
		/// Current selected button
		/// </summary>
		public RadioButton Current {
			get {
				return current;
			}
			set {
				if ((buttons.Contains(value) || value == null) && current != value) {
					current = value;
				}
			}
		}

		/// <summary>
		/// Radio group constructor
		/// </summary>
		public RadioGroup() {
			buttons = new List<RadioButton>();
		}

		/// <summary>
		/// Getting all the buttons
		/// </summary>
		/// <returns>Radio button array</returns>
		internal RadioButton[] GetAll() {
			return buttons.ToArray();
		}

		/// <summary>
		/// Registering button
		/// </summary>
		/// <param name="rb">Button</param>
		internal void Register(RadioButton rb) {
			if (!buttons.Contains(rb)) {
				buttons.Add(rb);
			}
		}

		/// <summary>
		/// Unregistering button
		/// </summary>
		/// <param name="rb">Button</param>
		internal void Unregister(RadioButton rb) {
			if (buttons.Contains(rb)) {
				buttons.Remove(rb);
			}
			if (current == rb) {
				current = null;
			}
		}
	}
}
