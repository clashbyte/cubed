namespace Cubed.Forms.Dialogs {
	partial class OpenFolderDialog {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenFolderDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this.goDocsButton = new Cubed.UI.Controls.NSIconicButton();
			this.goPCButton = new Cubed.UI.Controls.NSIconicButton();
			this.goHomeButton = new Cubed.UI.Controls.NSIconicButton();
			this.nsLabel1 = new Cubed.UI.Controls.NSLabel();
			this.directoryBrowser = new Cubed.UI.Controls.NSDirectoryInspector();
			this.goUpButton = new Cubed.UI.Controls.NSIconicButton();
			this.pathBox = new Cubed.UI.Controls.NSTextBox();
			this.newFolderButton = new Cubed.UI.Controls.NSIconicButton();
			this.selectButton = new Cubed.UI.Controls.NSIconicButton();
			this.cancelButton = new Cubed.UI.Controls.NSIconicButton();
			this.nsSeperator1 = new Cubed.UI.Controls.NSSeperator();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.newFolderButton);
			this.panel1.Controls.Add(this.selectButton);
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Controls.Add(this.nsSeperator1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 386);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(753, 54);
			this.panel1.TabIndex = 2;
			// 
			// goDocsButton
			// 
			this.goDocsButton.Corners.BottomLeft = true;
			this.goDocsButton.Corners.BottomRight = true;
			this.goDocsButton.Corners.TopLeft = false;
			this.goDocsButton.Corners.TopRight = false;
			this.goDocsButton.IconImage = ((System.Drawing.Image)(resources.GetObject("goDocsButton.IconImage")));
			this.goDocsButton.IconSize = new System.Drawing.Size(32, 32);
			this.goDocsButton.Large = false;
			this.goDocsButton.Location = new System.Drawing.Point(12, 186);
			this.goDocsButton.Name = "goDocsButton";
			this.goDocsButton.Size = new System.Drawing.Size(70, 70);
			this.goDocsButton.TabIndex = 9;
			this.goDocsButton.Text = "Documents";
			this.goDocsButton.Vertical = true;
			this.goDocsButton.Click += new System.EventHandler(this.goDocsButton_Click);
			// 
			// goPCButton
			// 
			this.goPCButton.Corners.BottomLeft = false;
			this.goPCButton.Corners.BottomRight = false;
			this.goPCButton.Corners.TopLeft = false;
			this.goPCButton.Corners.TopRight = false;
			this.goPCButton.IconImage = ((System.Drawing.Image)(resources.GetObject("goPCButton.IconImage")));
			this.goPCButton.IconSize = new System.Drawing.Size(32, 32);
			this.goPCButton.Large = false;
			this.goPCButton.Location = new System.Drawing.Point(12, 117);
			this.goPCButton.Name = "goPCButton";
			this.goPCButton.Size = new System.Drawing.Size(70, 70);
			this.goPCButton.TabIndex = 8;
			this.goPCButton.Text = "Computer";
			this.goPCButton.Vertical = true;
			this.goPCButton.Click += new System.EventHandler(this.goPCButton_Click);
			// 
			// goHomeButton
			// 
			this.goHomeButton.Corners.BottomLeft = false;
			this.goHomeButton.Corners.BottomRight = false;
			this.goHomeButton.Corners.TopLeft = true;
			this.goHomeButton.Corners.TopRight = true;
			this.goHomeButton.IconImage = ((System.Drawing.Image)(resources.GetObject("goHomeButton.IconImage")));
			this.goHomeButton.IconSize = new System.Drawing.Size(32, 32);
			this.goHomeButton.Large = false;
			this.goHomeButton.Location = new System.Drawing.Point(12, 48);
			this.goHomeButton.Name = "goHomeButton";
			this.goHomeButton.Size = new System.Drawing.Size(70, 70);
			this.goHomeButton.TabIndex = 7;
			this.goHomeButton.Text = "Home";
			this.goHomeButton.Vertical = true;
			this.goHomeButton.Click += new System.EventHandler(this.goHomeButton_Click);
			// 
			// nsLabel1
			// 
			this.nsLabel1.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.nsLabel1.ForeColor = System.Drawing.Color.White;
			this.nsLabel1.Location = new System.Drawing.Point(12, 12);
			this.nsLabel1.Name = "nsLabel1";
			this.nsLabel1.Size = new System.Drawing.Size(70, 30);
			this.nsLabel1.TabIndex = 6;
			this.nsLabel1.Text = "Location:";
			this.nsLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// directoryBrowser
			// 
			this.directoryBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.directoryBrowser.EmptyMessage = "Folder is empty";
			this.directoryBrowser.Font = new System.Drawing.Font("Tahoma", 8F);
			this.directoryBrowser.Location = new System.Drawing.Point(88, 48);
			this.directoryBrowser.Name = "directoryBrowser";
			this.directoryBrowser.Offset = 0;
			this.directoryBrowser.SelectedEntry = null;
			this.directoryBrowser.Size = new System.Drawing.Size(404, 332);
			this.directoryBrowser.TabIndex = 5;
			this.directoryBrowser.Text = "nsDirectoryInspector1";
			this.directoryBrowser.DoubleClick += new System.EventHandler(this.directoryBrowser_DoubleClick);
			// 
			// goUpButton
			// 
			this.goUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.goUpButton.Corners.BottomLeft = false;
			this.goUpButton.Corners.BottomRight = true;
			this.goUpButton.Corners.TopLeft = false;
			this.goUpButton.Corners.TopRight = true;
			this.goUpButton.IconImage = ((System.Drawing.Image)(resources.GetObject("goUpButton.IconImage")));
			this.goUpButton.IconSize = new System.Drawing.Size(16, 16);
			this.goUpButton.Large = false;
			this.goUpButton.Location = new System.Drawing.Point(653, 12);
			this.goUpButton.Name = "goUpButton";
			this.goUpButton.Size = new System.Drawing.Size(91, 30);
			this.goUpButton.TabIndex = 4;
			this.goUpButton.Text = "Parent";
			this.goUpButton.Vertical = false;
			// 
			// pathBox
			// 
			this.pathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pathBox.Corners.BottomLeft = true;
			this.pathBox.Corners.BottomRight = false;
			this.pathBox.Corners.TopLeft = true;
			this.pathBox.Corners.TopRight = false;
			this.pathBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.pathBox.Location = new System.Drawing.Point(88, 12);
			this.pathBox.MaxLength = 32767;
			this.pathBox.Multiline = false;
			this.pathBox.Name = "pathBox";
			this.pathBox.ReadOnly = false;
			this.pathBox.Size = new System.Drawing.Size(570, 30);
			this.pathBox.TabIndex = 3;
			this.pathBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.pathBox.UseSystemPasswordChar = false;
			// 
			// newFolderButton
			// 
			this.newFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.newFolderButton.Corners.BottomLeft = true;
			this.newFolderButton.Corners.BottomRight = true;
			this.newFolderButton.Corners.TopLeft = true;
			this.newFolderButton.Corners.TopRight = true;
			this.newFolderButton.IconImage = ((System.Drawing.Image)(resources.GetObject("newFolderButton.IconImage")));
			this.newFolderButton.IconSize = new System.Drawing.Size(16, 16);
			this.newFolderButton.Large = false;
			this.newFolderButton.Location = new System.Drawing.Point(372, 13);
			this.newFolderButton.Name = "newFolderButton";
			this.newFolderButton.Size = new System.Drawing.Size(120, 30);
			this.newFolderButton.TabIndex = 3;
			this.newFolderButton.Text = "New Folder";
			this.newFolderButton.Vertical = false;
			// 
			// selectButton
			// 
			this.selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.selectButton.Corners.BottomLeft = true;
			this.selectButton.Corners.BottomRight = true;
			this.selectButton.Corners.TopLeft = true;
			this.selectButton.Corners.TopRight = true;
			this.selectButton.IconImage = ((System.Drawing.Image)(resources.GetObject("selectButton.IconImage")));
			this.selectButton.IconSize = new System.Drawing.Size(16, 16);
			this.selectButton.Large = false;
			this.selectButton.Location = new System.Drawing.Point(498, 13);
			this.selectButton.Name = "selectButton";
			this.selectButton.Size = new System.Drawing.Size(120, 30);
			this.selectButton.TabIndex = 2;
			this.selectButton.Text = "Select";
			this.selectButton.Vertical = false;
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
			this.cancelButton.Location = new System.Drawing.Point(624, 13);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(120, 30);
			this.cancelButton.TabIndex = 0;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Vertical = false;
			// 
			// nsSeperator1
			// 
			this.nsSeperator1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nsSeperator1.Location = new System.Drawing.Point(0, 0);
			this.nsSeperator1.Name = "nsSeperator1";
			this.nsSeperator1.Size = new System.Drawing.Size(753, 54);
			this.nsSeperator1.TabIndex = 1;
			this.nsSeperator1.Text = "nsSeperator1";
			// 
			// OpenFolderDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(753, 440);
			this.Controls.Add(this.goDocsButton);
			this.Controls.Add(this.goPCButton);
			this.Controls.Add(this.goHomeButton);
			this.Controls.Add(this.nsLabel1);
			this.Controls.Add(this.directoryBrowser);
			this.Controls.Add(this.goUpButton);
			this.Controls.Add(this.pathBox);
			this.Controls.Add(this.panel1);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(530, 355);
			this.Name = "OpenFolderDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "OpenFolderDialog";
			this.Activated += new System.EventHandler(this.OpenFolderDialog_Activated);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Controls.NSIconicButton cancelButton;
		private UI.Controls.NSSeperator nsSeperator1;
		private System.Windows.Forms.Panel panel1;
		private UI.Controls.NSIconicButton newFolderButton;
		private UI.Controls.NSIconicButton selectButton;
		private UI.Controls.NSTextBox pathBox;
		private UI.Controls.NSIconicButton goUpButton;
		private UI.Controls.NSDirectoryInspector directoryBrowser;
		private UI.Controls.NSLabel nsLabel1;
		private UI.Controls.NSIconicButton goHomeButton;
		private UI.Controls.NSIconicButton goPCButton;
		private UI.Controls.NSIconicButton goDocsButton;
	}
}