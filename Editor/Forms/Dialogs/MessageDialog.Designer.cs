namespace Cubed.Forms.Dialogs {
	partial class MessageDialog {
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.nsSeperator1 = new Cubed.UI.Controls.NSSeperator();
			this.panel2 = new System.Windows.Forms.Panel();
			this.nsLabel1 = new Cubed.UI.Controls.NSLabel();
			this.button2 = new Cubed.UI.Controls.NSIconicButton();
			this.button3 = new Cubed.UI.Controls.NSIconicButton();
			this.button1 = new Cubed.UI.Controls.NSIconicButton();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.nsSeperator1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 87);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(390, 54);
			this.panel1.TabIndex = 0;
			// 
			// nsSeperator1
			// 
			this.nsSeperator1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nsSeperator1.Location = new System.Drawing.Point(0, 0);
			this.nsSeperator1.Name = "nsSeperator1";
			this.nsSeperator1.Size = new System.Drawing.Size(390, 54);
			this.nsSeperator1.TabIndex = 0;
			this.nsSeperator1.Text = "nsSeperator1";
			// 
			// panel2
			// 
			this.panel2.Location = new System.Drawing.Point(14, 14);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(66, 66);
			this.panel2.TabIndex = 1;
			// 
			// nsLabel1
			// 
			this.nsLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.nsLabel1.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.nsLabel1.ForeColor = System.Drawing.Color.White;
			this.nsLabel1.Location = new System.Drawing.Point(95, 14);
			this.nsLabel1.Name = "nsLabel1";
			this.nsLabel1.Size = new System.Drawing.Size(283, 64);
			this.nsLabel1.TabIndex = 2;
			this.nsLabel1.Text = "Text";
			this.nsLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Corners.BottomLeft = true;
			this.button2.Corners.BottomRight = true;
			this.button2.Corners.TopLeft = true;
			this.button2.Corners.TopRight = true;
			this.button2.Enabled = false;
			this.button2.IconImage = null;
			this.button2.IconSize = new System.Drawing.Size(16, 16);
			this.button2.Large = false;
			this.button2.Location = new System.Drawing.Point(134, 15);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(120, 30);
			this.button2.TabIndex = 4;
			this.button2.Text = "2";
			this.button2.Vertical = false;
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Corners.BottomLeft = true;
			this.button3.Corners.BottomRight = true;
			this.button3.Corners.TopLeft = true;
			this.button3.Corners.TopRight = true;
			this.button3.IconImage = null;
			this.button3.IconSize = new System.Drawing.Size(16, 16);
			this.button3.Large = false;
			this.button3.Location = new System.Drawing.Point(260, 15);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(120, 30);
			this.button3.TabIndex = 3;
			this.button3.Text = "3";
			this.button3.Vertical = false;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Corners.BottomLeft = true;
			this.button1.Corners.BottomRight = true;
			this.button1.Corners.TopLeft = true;
			this.button1.Corners.TopRight = true;
			this.button1.Enabled = false;
			this.button1.IconImage = null;
			this.button1.IconSize = new System.Drawing.Size(16, 16);
			this.button1.Large = false;
			this.button1.Location = new System.Drawing.Point(8, 15);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(120, 30);
			this.button1.TabIndex = 5;
			this.button1.Text = "1";
			this.button1.Vertical = false;
			// 
			// MessageDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(390, 141);
			this.ControlBox = false;
			this.Controls.Add(this.nsLabel1);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MessageDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Info";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private UI.Controls.NSSeperator nsSeperator1;
		private System.Windows.Forms.Panel panel2;
		private UI.Controls.NSLabel nsLabel1;
		private UI.Controls.NSIconicButton button2;
		private UI.Controls.NSIconicButton button3;
		private UI.Controls.NSIconicButton button1;
	}
}