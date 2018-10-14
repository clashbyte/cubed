using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cubed.Forms.Inspections.Fields {
	class ColorFieldInspector : FieldInspector {
		private UI.Controls.NSColorPickerButton colorPickerShowButton;

		bool customSwitch = false;

		Color color;

		Nested.ColorPickerForm pickerForm;

		public ColorFieldInspector()
			: base() {
				InitializeComponent();
		}

		public override void UpdateValue() {
			object obj = ReadValue();
			if (obj != null) {
				if (obj is Color && pickerForm == null) {
					customSwitch = true;
					color = (Color)obj;
					customSwitch = false;
				}
			}
			if (pickerForm != null) {
				//pickerForm.Value = color;
			}
			colorPickerShowButton.SelectedColor = color;
			colorPickerShowButton.Enabled = Info.CanWrite;
		}

		private void InitializeComponent() {
			this.colorPickerShowButton = new Cubed.UI.Controls.NSColorPickerButton();
			this.SuspendLayout();
			// 
			// colorPickerShowButton
			// 
			this.colorPickerShowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.colorPickerShowButton.Location = new System.Drawing.Point(297, 3);
			this.colorPickerShowButton.Name = "colorPickerShowButton";
			this.colorPickerShowButton.SelectedColor = System.Drawing.Color.White;
			this.colorPickerShowButton.Size = new System.Drawing.Size(60, 23);
			this.colorPickerShowButton.TabIndex = 0;
			this.colorPickerShowButton.Text = "nsColorPickerButton1";
			this.colorPickerShowButton.MouseClick += this.colorPickerShowButton_Click;
			// 
			// ColorFieldInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Controls.Add(this.colorPickerShowButton);
			this.Name = "ColorFieldInspector";
			this.ResumeLayout(false);

		}

		private void colorPickerShowButton_Click(object sender, EventArgs e) {
			
			// Searching parent
			Control parent = this;
			while (true) {
				if (parent == null || parent is Form) {
					break;
				}
				parent = parent.Parent;
			}

			Point loc = PointToScreen(new Point(Width, 0));
			loc = parent.PointToClient(loc);
			if (loc.Y + 200 > parent.Height) {
				loc.Y = loc.Y - 200;
			} else {
				loc.Y = loc.Y + 30;
			}
			loc.X -= 310;

			Nested.ColorPickerForm cpf = new Nested.ColorPickerForm();
			pickerForm = cpf;
			cpf.TopMost = true;
			cpf.TopLevel = false;
			cpf.Parent = parent;
			cpf.Value = color;
			cpf.Location = loc;
			cpf.Show();
			parent.Controls.Add(cpf);
			cpf.BringToFront();
			cpf.FormClosed += (esender, ee) => {
				cpf_ValueChanged(null, EventArgs.Empty);
				cpf.ValueChanged -= cpf_ValueChanged;
				parent.Controls.Remove(cpf);
				pickerForm = null;
			};
			cpf.ValueChanged += cpf_ValueChanged;

		}

		void cpf_ValueChanged(object sender, EventArgs e) {
			if (pickerForm != null) {
				Color clr = pickerForm.Value;
				colorPickerShowButton.SelectedColor = clr;
				color = clr;
				SetValue(clr);
			}
		}


	}
}
