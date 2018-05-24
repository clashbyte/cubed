namespace Cubed.Forms.Dialogs {
	partial class TextInputDialog {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextInputDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this.inputBox = new Cubed.UI.Controls.NSTextBox();
			this.descLabel = new Cubed.UI.Controls.NSLabel();
			this.submitButton = new Cubed.UI.Controls.NSIconicButton();
			this.cancelButton = new Cubed.UI.Controls.NSIconicButton();
			this.nsSeperator1 = new Cubed.UI.Controls.NSSeperator();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.submitButton);
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Controls.Add(this.nsSeperator1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 67);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(364, 54);
			this.panel1.TabIndex = 0;
			// 
			// inputBox
			// 
			this.inputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.inputBox.Corners.BottomLeft = true;
			this.inputBox.Corners.BottomRight = true;
			this.inputBox.Corners.TopLeft = true;
			this.inputBox.Corners.TopRight = true;
			this.inputBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.inputBox.Location = new System.Drawing.Point(12, 36);
			this.inputBox.MaxLength = 32767;
			this.inputBox.Multiline = false;
			this.inputBox.Name = "inputBox";
			this.inputBox.ReadOnly = false;
			this.inputBox.Size = new System.Drawing.Size(342, 23);
			this.inputBox.TabIndex = 2;
			this.inputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.inputBox.UseSystemPasswordChar = false;
			this.inputBox.TextChanged += new System.EventHandler(this.inputBox_TextChanged);
			// 
			// descLabel
			// 
			this.descLabel.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.descLabel.ForeColor = System.Drawing.Color.White;
			this.descLabel.Location = new System.Drawing.Point(12, 12);
			this.descLabel.Name = "descLabel";
			this.descLabel.Size = new System.Drawing.Size(340, 23);
			this.descLabel.TabIndex = 1;
			this.descLabel.Text = "Enter name:";
			this.descLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// submitButton
			// 
			this.submitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.submitButton.Corners.BottomLeft = true;
			this.submitButton.Corners.BottomRight = true;
			this.submitButton.Corners.TopLeft = true;
			this.submitButton.Corners.TopRight = true;
			this.submitButton.Enabled = false;
			this.submitButton.IconImage = ((System.Drawing.Image)(resources.GetObject("submitButton.IconImage")));
			this.submitButton.IconSize = new System.Drawing.Size(16, 16);
			this.submitButton.Large = false;
			this.submitButton.Location = new System.Drawing.Point(108, 15);
			this.submitButton.Name = "submitButton";
			this.submitButton.Size = new System.Drawing.Size(120, 30);
			this.submitButton.TabIndex = 2;
			this.submitButton.Text = "OK";
			this.submitButton.Vertical = false;
			this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.Corners.BottomLeft = true;
			this.cancelButton.Corners.BottomRight = true;
			this.cancelButton.Corners.TopLeft = true;
			this.cancelButton.Corners.TopRight = true;
			this.cancelButton.IconImage = ((System.Drawing.Image)(resources.GetObject("cancelButton.IconImage")));
			this.cancelButton.IconSize = new System.Drawing.Size(16, 16);
			this.cancelButton.Large = false;
			this.cancelButton.Location = new System.Drawing.Point(234, 15);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(120, 30);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Vertical = false;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// nsSeperator1
			// 
			this.nsSeperator1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nsSeperator1.Location = new System.Drawing.Point(0, 0);
			this.nsSeperator1.Name = "nsSeperator1";
			this.nsSeperator1.Size = new System.Drawing.Size(364, 54);
			this.nsSeperator1.TabIndex = 0;
			this.nsSeperator1.Text = "nsSeperator1";
			// 
			// TextInputDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(364, 121);
			this.Controls.Add(this.inputBox);
			this.Controls.Add(this.descLabel);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TextInputDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "TextInputDialog";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private UI.Controls.NSSeperator nsSeperator1;
		private UI.Controls.NSIconicButton cancelButton;
		private UI.Controls.NSIconicButton submitButton;
		private UI.Controls.NSLabel descLabel;
		private UI.Controls.NSTextBox inputBox;
	}
}