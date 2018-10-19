namespace Cubed.Forms.Common {
	partial class StartupForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.newProjectButton = new Cubed.UI.Controls.NSIconicButton();
			this.openProjectButton = new Cubed.UI.Controls.NSIconicButton();
			this.loadingLabel = new Cubed.UI.Controls.NSLabel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.newProjectButton);
			this.panel1.Controls.Add(this.openProjectButton);
			this.panel1.Name = "panel1";
			// 
			// newProjectButton
			// 
			resources.ApplyResources(this.newProjectButton, "newProjectButton");
			this.newProjectButton.Corners.BottomLeft = false;
			this.newProjectButton.Corners.BottomRight = false;
			this.newProjectButton.Corners.TopLeft = true;
			this.newProjectButton.Corners.TopRight = true;
			this.newProjectButton.IconImage = ((System.Drawing.Image)(resources.GetObject("newProjectButton.IconImage")));
			this.newProjectButton.IconSize = new System.Drawing.Size(16, 16);
			this.newProjectButton.Large = false;
			this.newProjectButton.Name = "newProjectButton";
			this.newProjectButton.Vertical = false;
			this.newProjectButton.Click += new System.EventHandler(this.newProjectButton_Click);
			// 
			// openProjectButton
			// 
			resources.ApplyResources(this.openProjectButton, "openProjectButton");
			this.openProjectButton.Corners.BottomLeft = true;
			this.openProjectButton.Corners.BottomRight = true;
			this.openProjectButton.Corners.TopLeft = false;
			this.openProjectButton.Corners.TopRight = false;
			this.openProjectButton.IconImage = ((System.Drawing.Image)(resources.GetObject("openProjectButton.IconImage")));
			this.openProjectButton.IconSize = new System.Drawing.Size(16, 16);
			this.openProjectButton.Large = false;
			this.openProjectButton.Name = "openProjectButton";
			this.openProjectButton.Vertical = false;
			this.openProjectButton.Click += new System.EventHandler(this.openProjectButton_Click);
			// 
			// loadingLabel
			// 
			resources.ApplyResources(this.loadingLabel, "loadingLabel");
			this.loadingLabel.ForeColor = System.Drawing.Color.White;
			this.loadingLabel.Name = "loadingLabel";
			this.loadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// StartupForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.Controls.Add(this.loadingLabel);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StartupForm";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Controls.NSIconicButton newProjectButton;
		private UI.Controls.NSIconicButton openProjectButton;
		private System.Windows.Forms.Panel panel1;
		private UI.Controls.NSLabel loadingLabel;
	}
}