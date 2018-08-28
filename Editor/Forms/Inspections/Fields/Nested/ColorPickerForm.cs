using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Cubed.Forms.Inspections.Fields.Nested {
	public partial class ColorPickerForm : Form {

		public event EventHandler ValueChanged;

		bool customChange = false;

		/// <summary>
		/// Color value
		/// </summary>
		public Color Value {
			get {
				return picker.SelectedColor;
			}
			set {
				picker.SelectedColor = value;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ColorPickerForm() {
			InitializeComponent();
			typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, colorPanel, new object[] { true });
			hexValue.BaseInput.LostFocus += BaseInput_LostFocus;
			hexValue.BaseInput.KeyDown += BaseInput_KeyDown;
			rgbValue.BaseInput.LostFocus += BaseInput_LostFocus;
			rgbValue.BaseInput.KeyDown += BaseInput_KeyDown;
		}

		/// <summary>
		/// Keypress in field
		/// </summary>
		void BaseInput_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				if (sender == hexValue.BaseInput) {
					ParseHex();
				} else {
					ParseRGB();
				}
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}

		/// <summary>
		/// Color field focus loss
		/// </summary>
		void BaseInput_LostFocus(object sender, EventArgs e) {
			if (sender == hexValue.BaseInput) {
				ParseHex();
			} else {
				ParseRGB();
			}
		}

		/// <summary>
		/// Painting form
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			e.Graphics.DrawRectangle(new Pen(Color.FromArgb(35, 35, 35)), 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
		}

		/// <summary>
		/// Leaving form
		/// </summary>
		protected override void OnLeave(EventArgs e) {
			base.OnLeave(e);
			Close();
		}

		/// <summary>
		/// Click on OK button
		/// </summary>
		private void okButton_Click(object sender, EventArgs e) {
			Close();
		}

		/// <summary>
		/// Drawing color
		/// </summary>
		private void colorPanel_Paint(object sender, PaintEventArgs e) {
			e.Graphics.Clear(picker.SelectedColor);
			e.Graphics.DrawRectangle(new Pen(Color.FromArgb(35, 35, 35)), 0, 0, colorPanel.ClientSize.Width - 1, colorPanel.ClientSize.Height - 1);
		}
		
		/// <summary>
		/// Calculating value
		/// </summary>
		private void picker_ValueChanged(object sender, EventArgs e) {
			colorPanel.Invalidate();
			customChange = true;
			SetHexValue(picker.SelectedColor);
			SetRGBValue(picker.SelectedColor);
			customChange = false;
			if (ValueChanged != null) {
				ValueChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Setting RGB field
		/// </summary>
		/// <param name="c">Color to set</param>
		void SetRGBValue(Color c) {
			rgbValue.Text = c.R + ", " + c.G + ", " + c.B;
		}

		/// <summary>
		/// Setting Hex field
		/// </summary>
		/// <param name="c">Color to set</param>
		void SetHexValue(Color c) {
			hexValue.Text = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
		}

		/// <summary>
		/// Parsing RGB
		/// </summary>
		void ParseRGB() {

			// Preparing params
			bool parsed = false;
			Color newColor = Color.White;
			string value = rgbValue.Text;

			// Splitting parts
			string[] parts = value.Split(',');
			if (parts.Length == 3) {
				byte[] values = new byte[3];
				for (int i = 0; i < 3; i++) {
					string v = parts[i].Trim();
					byte bv = 0;
					if (!byte.TryParse(v, out bv)) {
						values = null;
						break;
					}
					values[i] = bv;
				}
				if (values != null) {
					newColor = Color.FromArgb(values[0], values[1], values[2]);
					parsed = true;
				}
			}

			// Parsing
			if (parsed) {
				picker.SelectedColor = newColor;
			} else {
				customChange = true;
				SetRGBValue(picker.SelectedColor);
				customChange = false;
				System.Media.SystemSounds.Exclamation.Play();
			}
		}

		/// <summary>
		/// Parsing Hex
		/// </summary>
		void ParseHex() {

			// Preparing params
			bool parsed = false;
			Color newColor = Color.White;
			string value = hexValue.Text.Trim().Replace(" ", "");

			// Splitting parts
			if (value.StartsWith("#")) {
				value = value.Substring(1);
			}
			if (value.Length == 3 || value.Length == 6) {
				if (Regex.IsMatch(value, "^[a-fA-F0-9]+$")) {
					if (value.Length == 3) {
						value = new string(new char[]{
							value[0], value[0], value[1], value[1], value[2], value[2]
						});
					}
					value = value.ToUpper();
					byte[] values = new byte[3];
					for (int i = 0; i < 3; i++) {
						string sub = value.Substring(i * 2, 2);
						values[i] = Convert.ToByte(sub, 16);
					}
					newColor = Color.FromArgb(values[0], values[1], values[2]);
					parsed = true;
				}
			}

			// Parsing
			if (parsed) {
				picker.SelectedColor = newColor;
			} else {
				customChange = true;
				SetHexValue(picker.SelectedColor);
				customChange = false;
				System.Media.SystemSounds.Exclamation.Play();
			}
		}

	}
}
