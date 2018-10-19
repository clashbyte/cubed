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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this.button1 = new Cubed.UI.Controls.NSIconicButton();
			this.button2 = new Cubed.UI.Controls.NSIconicButton();
			this.button3 = new Cubed.UI.Controls.NSIconicButton();
			this.nsSeperator1 = new Cubed.UI.Controls.NSSeperator();
			this.iconPanel = new System.Windows.Forms.Panel();
			this.contentLabel = new Cubed.UI.Controls.NSLabel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.nsSeperator1);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.Corners.BottomLeft = true;
			this.button1.Corners.BottomRight = true;
			this.button1.Corners.TopLeft = true;
			this.button1.Corners.TopRight = true;
			this.button1.IconImage = null;
			this.button1.IconSize = new System.Drawing.Size(16, 16);
			this.button1.Large = false;
			this.button1.Name = "button1";
			this.button1.Vertical = false;
			this.button1.Click += new System.EventHandler(this.button_Click);
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.button2.Corners.BottomLeft = true;
			this.button2.Corners.BottomRight = true;
			this.button2.Corners.TopLeft = true;
			this.button2.Corners.TopRight = true;
			this.button2.IconImage = null;
			this.button2.IconSize = new System.Drawing.Size(16, 16);
			this.button2.Large = false;
			this.button2.Name = "button2";
			this.button2.Vertical = false;
			this.button2.Click += new System.EventHandler(this.button_Click);
			// 
			// button3
			// 
			resources.ApplyResources(this.button3, "button3");
			this.button3.Corners.BottomLeft = true;
			this.button3.Corners.BottomRight = true;
			this.button3.Corners.TopLeft = true;
			this.button3.Corners.TopRight = true;
			this.button3.IconImage = null;
			this.button3.IconSize = new System.Drawing.Size(16, 16);
			this.button3.Large = false;
			this.button3.Name = "button3";
			this.button3.Vertical = false;
			this.button3.Click += new System.EventHandler(this.button_Click);
			// 
			// nsSeperator1
			// 
			resources.ApplyResources(this.nsSeperator1, "nsSeperator1");
			this.nsSeperator1.Name = "nsSeperator1";
			// 
			// iconPanel
			// 
			resources.ApplyResources(this.iconPanel, "iconPanel");
			this.iconPanel.Name = "iconPanel";
			this.iconPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.iconPanel_Paint);
			// 
			// contentLabel
			// 
			resources.ApplyResources(this.contentLabel, "contentLabel");
			this.contentLabel.ForeColor = System.Drawing.Color.White;
			this.contentLabel.Name = "contentLabel";
			this.contentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// MessageDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ControlBox = false;
			this.Controls.Add(this.contentLabel);
			this.Controls.Add(this.iconPanel);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MessageDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private UI.Controls.NSSeperator nsSeperator1;
		private System.Windows.Forms.Panel iconPanel;
		private UI.Controls.NSLabel contentLabel;
		private UI.Controls.NSIconicButton button2;
		private UI.Controls.NSIconicButton button3;
		private UI.Controls.NSIconicButton button1;
	}
}