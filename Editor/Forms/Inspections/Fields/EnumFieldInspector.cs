using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Forms.Inspections.Fields {
	class EnumFieldInspector : FieldInspector {
		private UI.Controls.NSComboBox comboBox;

		bool customChange = false;

		string[] opts;

		Array values;

		public EnumFieldInspector()
			: base() {
				InitializeComponent();
		}

		public override void UpdateValue() {
			customChange = true;
			if (opts == null) {
				opts = Enum.GetNames(Info.PropertyType);
				values = Enum.GetValues(Info.PropertyType);
				comboBox.SuspendLayout();
				comboBox.Items = string.Join("\n", opts);
				comboBox.ResumeLayout();
				comboBox.Enabled = Info.CanWrite;
			}
			Array vals = Enum.GetValues(Info.PropertyType);
			int idx = Array.IndexOf(vals, ReadValue());
			if (idx != comboBox.SelectedIndex) {
				comboBox.SelectedIndex = idx;
			}
			customChange = false;
		}

		private void InitializeComponent() {
			this.comboBox = new Cubed.UI.Controls.NSComboBox();
			this.SuspendLayout();
			// 
			// comboBox
			// 
			this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.comboBox.ForeColor = System.Drawing.Color.White;
			this.comboBox.Items = "";
			this.comboBox.Location = new System.Drawing.Point(3, 3);
			this.comboBox.Name = "comboBox";
			this.comboBox.SelectedIndex = 0;
			this.comboBox.Size = new System.Drawing.Size(354, 23);
			this.comboBox.TabIndex = 0;
			this.comboBox.IndexChanged += new Cubed.UI.Controls.NSComboBox.SelectionChangedEventHandler(this.comboBox_IndexChanged);
			// 
			// EnumFieldInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Controls.Add(this.comboBox);
			this.Name = "EnumFieldInspector";
			this.ResumeLayout(false);

		}

		private void comboBox_IndexChanged(object sender) {
			if (!customChange) {
				SetValue(Enum.Parse(Info.PropertyType, opts[comboBox.SelectedIndex]));
			}
		}
	}
}
