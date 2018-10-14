using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Forms.Inspections.Fields {
	class StringFieldInspector : FieldInspector {

		private UI.Controls.NSTextBox textBox;

		bool customChange = false;

		public StringFieldInspector()
			: base() {
				InitializeComponent();
		}

		public override void UpdateValue() {
			string data = "";
			object obj = ReadValue();
			if (obj != null) {
				if (obj is string) {
					data = (string)obj;
				}
			}
			if (!ContainsFocus) {
				customChange = true;
				textBox.Text = data;
				customChange = false;
			}
		}


		private void InitializeComponent() {
			this.textBox = new Cubed.UI.Controls.NSTextBox();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Corners.BottomLeft = true;
			this.textBox.Corners.BottomRight = true;
			this.textBox.Corners.TopLeft = true;
			this.textBox.Corners.TopRight = true;
			this.textBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.textBox.Location = new System.Drawing.Point(3, 3);
			this.textBox.MaxLength = 32767;
			this.textBox.Multiline = false;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = false;
			this.textBox.Size = new System.Drawing.Size(354, 23);
			this.textBox.TabIndex = 0;
			this.textBox.Text = "nsTextBox1";
			this.textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.textBox.UseSystemPasswordChar = false;
			// 
			// StringFieldInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Controls.Add(this.textBox);
			this.Name = "StringFieldInspector";
			this.ResumeLayout(false);
			textBox.TextChanged += textBox_TextChanged;
		}

		void textBox_TextChanged(object sender, EventArgs e) {
			if (!customChange) {
				SetValue(textBox.Text);
			}
		}
	}
}
