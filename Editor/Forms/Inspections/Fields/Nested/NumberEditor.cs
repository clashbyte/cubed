using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cubed.Forms.Inspections.Fields.Nested {
	public partial class NumberEditor : UserControl {

		/// <summary>
		/// Changing value
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Label for input
		/// </summary>
		public string Label {
			get {
				return labelText;
			}
			set {
				labelText = value;
				label.Text = labelText+":";
				panel1.Visible = labelText != "";
				panel1.Dock = labelText != "" ? DockStyle.Fill : DockStyle.None;
			}
		}

		/// <summary>
		/// Current value
		/// </summary>
		public float Value {
			get {
				return value;
			}
			set {
				float val = this.value;
				if (true) {
					
				}
				SetValue(value.ToString());
			}
		}

		/// <summary>
		/// Rounding to integer
		/// </summary>
		public bool IsInteger {
			get {
				return intMode;
			}
			set {
				intMode = value;
				Value = this.value;
			}
		}

		/// <summary>
		/// Maximal value
		/// </summary>
		public float Max {
			get {
				return max;
			}
			set {
				max = value;
				Value = this.value;
			}
		}

		/// <summary>
		/// Minimal value
		/// </summary>
		public float Min {
			get {
				return min;
			}
			set {
				min = value;
				Value = this.value;
			}
		}

		/// <summary>
		/// Inner text
		/// </summary>
		string labelText;

		/// <summary>
		/// Setting value by hand
		/// </summary>
		bool customChange = false;

		/// <summary>
		/// Current value
		/// </summary>
		float value;

		/// <summary>
		/// Run in integer mode
		/// </summary>
		bool intMode = false;

		/// <summary>
		/// Minimal and maximal values
		/// </summary>
		float min = float.MinValue, max = float.MaxValue;

		/// <summary>
		/// Constructor
		/// </summary>
		public NumberEditor() {
			InitializeComponent();
			textBox.BaseInput.LostFocus += textBox_LostFocus;
			textBox.BaseInput.KeyDown += textBox_KeyDown;
		}

		/// <summary>
		/// Key hit value
		/// </summary>
		void textBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				SetValue(textBox.Text);
				e.SuppressKeyPress = true;
				e.Handled = true;
				Control c = Parent;
				while (true) {
					if (c is Form) {
						(c as Form).ActiveControl = null;
						break;
					} else if(c.Parent != null) {
						c = c.Parent;
					} else {
						break;
					}
				}
				if (ValueChanged != null) {
					ValueChanged(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Parsing value
		/// </summary>
		void textBox_LostFocus(object sender, EventArgs e) {
			SetValue(textBox.Text);
			if (ValueChanged != null) {
				ValueChanged(this, EventArgs.Empty);
			}
		}



		/// <summary>
		/// Setting value
		/// </summary>
		/// <param name="data">Data</param>
		void SetValue(string data) {
			float d = Math.Max(0, min);
			if (!float.TryParse(data, out d) && data != "") {
				System.Media.SystemSounds.Exclamation.Play();
				d = value;
			}
			d = Math.Max(Math.Min(d, max), min);
			if (intMode) {
				d = (float)Math.Round(d);
			}
			value = d;
			customChange = true;
			textBox.Text = d.ToString();
			customChange = false;
		}
	}
}
