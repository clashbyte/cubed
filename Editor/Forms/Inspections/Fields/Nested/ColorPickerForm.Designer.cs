namespace Cubed.Forms.Inspections.Fields.Nested {
	partial class ColorPickerForm {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.colorPanel = new System.Windows.Forms.Panel();
			this.nsLabel2 = new Cubed.UI.Controls.NSLabel();
			this.nsLabel1 = new Cubed.UI.Controls.NSLabel();
			this.okButton = new Cubed.UI.Controls.NSButton();
			this.rgbValue = new Cubed.UI.Controls.NSTextBox();
			this.hexValue = new Cubed.UI.Controls.NSTextBox();
			this.nsSeperator1 = new Cubed.UI.Controls.NSSeperator();
			this.picker = new Cubed.UI.Controls.NSColorPicker();
			this.SuspendLayout();
			// 
			// colorPanel
			// 
			this.colorPanel.Location = new System.Drawing.Point(203, 70);
			this.colorPanel.Name = "colorPanel";
			this.colorPanel.Size = new System.Drawing.Size(100, 40);
			this.colorPanel.TabIndex = 5;
			this.colorPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.colorPanel_Paint);
			// 
			// nsLabel2
			// 
			this.nsLabel2.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.nsLabel2.ForeColor = System.Drawing.Color.White;
			this.nsLabel2.Location = new System.Drawing.Point(160, 41);
			this.nsLabel2.Name = "nsLabel2";
			this.nsLabel2.Size = new System.Drawing.Size(37, 23);
			this.nsLabel2.TabIndex = 7;
			this.nsLabel2.Text = "RGB:";
			this.nsLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// nsLabel1
			// 
			this.nsLabel1.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.nsLabel1.ForeColor = System.Drawing.Color.White;
			this.nsLabel1.Location = new System.Drawing.Point(160, 12);
			this.nsLabel1.Name = "nsLabel1";
			this.nsLabel1.Size = new System.Drawing.Size(37, 23);
			this.nsLabel1.TabIndex = 6;
			this.nsLabel1.Text = "HEX:";
			this.nsLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(203, 129);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(100, 23);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// rgbValue
			// 
			this.rgbValue.Corners.BottomLeft = true;
			this.rgbValue.Corners.BottomRight = true;
			this.rgbValue.Corners.TopLeft = true;
			this.rgbValue.Corners.TopRight = true;
			this.rgbValue.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.rgbValue.Location = new System.Drawing.Point(203, 41);
			this.rgbValue.MaxLength = 32767;
			this.rgbValue.Multiline = false;
			this.rgbValue.Name = "rgbValue";
			this.rgbValue.ReadOnly = false;
			this.rgbValue.Size = new System.Drawing.Size(100, 23);
			this.rgbValue.TabIndex = 3;
			this.rgbValue.Text = "nsTextBox2";
			this.rgbValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.rgbValue.UseSystemPasswordChar = false;
			// 
			// hexValue
			// 
			this.hexValue.Corners.BottomLeft = true;
			this.hexValue.Corners.BottomRight = true;
			this.hexValue.Corners.TopLeft = true;
			this.hexValue.Corners.TopRight = true;
			this.hexValue.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.hexValue.Location = new System.Drawing.Point(203, 12);
			this.hexValue.MaxLength = 32767;
			this.hexValue.Multiline = false;
			this.hexValue.Name = "hexValue";
			this.hexValue.ReadOnly = false;
			this.hexValue.Size = new System.Drawing.Size(100, 23);
			this.hexValue.TabIndex = 2;
			this.hexValue.Text = "nsTextBox1";
			this.hexValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.hexValue.UseSystemPasswordChar = false;
			// 
			// nsSeperator1
			// 
			this.nsSeperator1.Location = new System.Drawing.Point(165, 116);
			this.nsSeperator1.Name = "nsSeperator1";
			this.nsSeperator1.Size = new System.Drawing.Size(138, 12);
			this.nsSeperator1.TabIndex = 1;
			this.nsSeperator1.Text = "nsSeperator1";
			// 
			// nsColorPicker1
			// 
			this.picker.Location = new System.Drawing.Point(1, 1);
			this.picker.Name = "nsColorPicker1";
			this.picker.SelectedColor = System.Drawing.Color.Red;
			this.picker.Size = new System.Drawing.Size(158, 158);
			this.picker.TabIndex = 0;
			this.picker.Text = "nsColorPicker1";
			this.picker.ValueChanged += new System.EventHandler(this.picker_ValueChanged);
			// 
			// ColorPickerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(310, 160);
			this.ControlBox = false;
			this.Controls.Add(this.nsLabel2);
			this.Controls.Add(this.nsLabel1);
			this.Controls.Add(this.colorPanel);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.rgbValue);
			this.Controls.Add(this.hexValue);
			this.Controls.Add(this.nsSeperator1);
			this.Controls.Add(this.picker);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ColorPickerForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "ColorPickerForm";
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Controls.NSColorPicker picker;
		private UI.Controls.NSSeperator nsSeperator1;
		private UI.Controls.NSTextBox hexValue;
		private UI.Controls.NSTextBox rgbValue;
		private UI.Controls.NSButton okButton;
		private System.Windows.Forms.Panel colorPanel;
		private UI.Controls.NSLabel nsLabel1;
		private UI.Controls.NSLabel nsLabel2;
	}
}