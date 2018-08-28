namespace Cubed.Forms.Inspections.Fields.Nested {
	partial class NumberEditor {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.textBox = new Cubed.UI.Controls.NSTextBox();
			this.label = new Cubed.UI.Controls.NSLabel();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(29, 30);
			this.panel1.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.textBox);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(29, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(173, 30);
			this.panel2.TabIndex = 1;
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
			this.textBox.Location = new System.Drawing.Point(3, 4);
			this.textBox.MaxLength = 32767;
			this.textBox.Multiline = false;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = false;
			this.textBox.Size = new System.Drawing.Size(167, 23);
			this.textBox.TabIndex = 1;
			this.textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.textBox.UseSystemPasswordChar = false;
			// 
			// label
			// 
			this.label.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.label.ForeColor = System.Drawing.Color.White;
			this.label.Location = new System.Drawing.Point(3, 3);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(23, 24);
			this.label.TabIndex = 0;
			this.label.Text = "X:";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// NumberEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "NumberEditor";
			this.Size = new System.Drawing.Size(202, 30);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private UI.Controls.NSLabel label;
		private UI.Controls.NSTextBox textBox;
		private System.Windows.Forms.Panel panel2;
	}
}
