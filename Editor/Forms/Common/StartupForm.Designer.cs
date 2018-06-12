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
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.newProjectButton = new Cubed.UI.Controls.NSIconicButton();
			this.openProjectButton = new Cubed.UI.Controls.NSIconicButton();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.newProjectButton);
			this.panel1.Controls.Add(this.openProjectButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(240, 380);
			this.panel1.TabIndex = 3;
			// 
			// webBrowser
			// 
			this.webBrowser.AllowWebBrowserDrop = false;
			this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser.IsWebBrowserContextMenuEnabled = false;
			this.webBrowser.Location = new System.Drawing.Point(240, 0);
			this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new System.Drawing.Size(417, 380);
			this.webBrowser.TabIndex = 4;
			this.webBrowser.Url = new System.Uri("https://clashbyte.ru/cubed/news", System.UriKind.Absolute);
			this.webBrowser.WebBrowserShortcutsEnabled = false;
			// 
			// newProjectButton
			// 
			this.newProjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.newProjectButton.Corners.BottomLeft = false;
			this.newProjectButton.Corners.BottomRight = false;
			this.newProjectButton.Corners.TopLeft = true;
			this.newProjectButton.Corners.TopRight = true;
			this.newProjectButton.IconImage = ((System.Drawing.Image)(resources.GetObject("newProjectButton.IconImage")));
			this.newProjectButton.IconSize = new System.Drawing.Size(16, 16);
			this.newProjectButton.Large = false;
			this.newProjectButton.Location = new System.Drawing.Point(13, 12);
			this.newProjectButton.Name = "newProjectButton";
			this.newProjectButton.Size = new System.Drawing.Size(215, 35);
			this.newProjectButton.TabIndex = 1;
			this.newProjectButton.Text = "New Project!";
			this.newProjectButton.Vertical = false;
			this.newProjectButton.Click += new System.EventHandler(this.newProjectButton_Click);
			// 
			// openProjectButton
			// 
			this.openProjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.openProjectButton.Corners.BottomLeft = true;
			this.openProjectButton.Corners.BottomRight = true;
			this.openProjectButton.Corners.TopLeft = false;
			this.openProjectButton.Corners.TopRight = false;
			this.openProjectButton.IconImage = ((System.Drawing.Image)(resources.GetObject("openProjectButton.IconImage")));
			this.openProjectButton.IconSize = new System.Drawing.Size(16, 16);
			this.openProjectButton.Large = false;
			this.openProjectButton.Location = new System.Drawing.Point(13, 46);
			this.openProjectButton.Name = "openProjectButton";
			this.openProjectButton.Size = new System.Drawing.Size(215, 35);
			this.openProjectButton.TabIndex = 2;
			this.openProjectButton.Text = "Open Existing Project";
			this.openProjectButton.Vertical = false;
			this.openProjectButton.Click += new System.EventHandler(this.openProjectButton_Click);
			// 
			// StartupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(657, 380);
			this.Controls.Add(this.webBrowser);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StartupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cubed";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Controls.NSIconicButton newProjectButton;
		private UI.Controls.NSIconicButton openProjectButton;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.WebBrowser webBrowser;
	}
}