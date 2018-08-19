using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Forms.Inspections.Fields {
	class BoolFieldInspector : FieldInspector {
		private UI.Controls.NSOnOffBox checkBox;

		bool customSwitch = false;

		public BoolFieldInspector()
			: base() {
				InitializeComponent();
		}

		public override void UpdateValue() {
			if (!Info.CanWrite) {
				checkBox.Enabled = false;
			}
			object obj = ReadValue();
			if (obj != null) {
				if (obj is bool) {
					customSwitch = true;
					checkBox.Checked = (bool)obj;
					customSwitch = false;
				}
			}
		}

		private void InitializeComponent() {
			this.checkBox = new Cubed.UI.Controls.NSOnOffBox();
			this.SuspendLayout();
			// 
			// checkBox
			// 
			this.checkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox.Checked = false;
			this.checkBox.Location = new System.Drawing.Point(301, 3);
			this.checkBox.MaximumSize = new System.Drawing.Size(56, 24);
			this.checkBox.MinimumSize = new System.Drawing.Size(56, 24);
			this.checkBox.Name = "checkBox";
			this.checkBox.Size = new System.Drawing.Size(56, 24);
			this.checkBox.TabIndex = 0;
			this.checkBox.Text = "nsOnOffBox1";
			this.checkBox.CheckedChanged += new Cubed.UI.Controls.NSOnOffBox.CheckedChangedEventHandler(this.checkBox_CheckedChanged);
			// 
			// BoolFieldInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Controls.Add(this.checkBox);
			this.Name = "BoolFieldInspector";
			this.ResumeLayout(false);

		}

		private void checkBox_CheckedChanged(object sender) {
			if (!customSwitch) {
				SetValue(checkBox.Checked);
			}
		}

	}
}
