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
			this.folderInfo = new Cubed.UI.Controls.NSFileInfo();
			this.goDocsButton = new Cubed.UI.Controls.NSIconicButton();
			this.newFolderButton = new Cubed.UI.Controls.NSIconicButton();
			this.selectButton = new Cubed.UI.Controls.NSIconicButton();
			this.cancelButton = new Cubed.UI.Controls.NSIconicButton();
			this.nsSeperator1 = new Cubed.UI.Controls.NSSeperator();
			this.goPCButton = new Cubed.UI.Controls.NSIconicButton();
			this.goHomeButton = new Cubed.UI.Controls.NSIconicButton();
			this.nsLabel1 = new Cubed.UI.Controls.NSLabel();
			this.directoryBrowser = new Cubed.UI.Controls.NSDirectoryInspector();
			this.goUpButton = new Cubed.UI.Controls.NSIconicButton();
			this.pathBox = new Cubed.UI.Controls.NSTextBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.newFolderButton);
			this.panel1.Controls.Add(this.selectButton);
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Controls.Add(this.nsSeperator1);
			this.panel1.Name = "panel1";
			// 
			// folderInfo
			// 
			resources.ApplyResources(this.folderInfo, "folderInfo");
			this.folderInfo.File = null;
			this.folderInfo.IconPadding = 24;
			this.folderInfo.Name = "folderInfo";
			this.folderInfo.Vertical = true;
			// 
			// goDocsButton
			// 
			resources.ApplyResources(this.goDocsButton, "goDocsButton");
			this.goDocsButton.Corners.BottomLeft = true;
			this.goDocsButton.Corners.BottomRight = true;
			this.goDocsButton.Corners.TopLeft = false;
			this.goDocsButton.Corners.TopRight = false;
			this.goDocsButton.IconImage = ((System.Drawing.Image)(resources.GetObject("goDocsButton.IconImage")));
			this.goDocsButton.IconSize = new System.Drawing.Size(32, 32);
			this.goDocsButton.Large = false;
			this.goDocsButton.Name = "goDocsButton";
			this.goDocsButton.Vertical = true;
			this.goDocsButton.Click += new System.EventHandler(this.goDocsButton_Click);
			// 
			// newFolderButton
			// 
			resources.ApplyResources(this.newFolderButton, "newFolderButton");
			this.newFolderButton.Corners.BottomLeft = true;
			this.newFolderButton.Corners.BottomRight = true;
			this.newFolderButton.Corners.TopLeft = true;
			this.newFolderButton.Corners.TopRight = true;
			this.newFolderButton.IconImage = ((System.Drawing.Image)(resources.GetObject("newFolderButton.IconImage")));
			this.newFolderButton.IconSize = new System.Drawing.Size(16, 16);
			this.newFolderButton.Large = false;
			this.newFolderButton.Name = "newFolderButton";
			this.newFolderButton.Vertical = false;
			this.newFolderButton.Click += new System.EventHandler(this.newFolderButton_Click);
			// 
			// selectButton
			// 
			resources.ApplyResources(this.selectButton, "selectButton");
			this.selectButton.Corners.BottomLeft = true;
			this.selectButton.Corners.BottomRight = true;
			this.selectButton.Corners.TopLeft = true;
			this.selectButton.Corners.TopRight = true;
			this.selectButton.IconImage = ((System.Drawing.Image)(resources.GetObject("selectButton.IconImage")));
			this.selectButton.IconSize = new System.Drawing.Size(16, 16);
			this.selectButton.Large = false;
			this.selectButton.Name = "selectButton";
			this.selectButton.Vertical = false;
			this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.Corners.BottomLeft = true;
			this.cancelButton.Corners.BottomRight = true;
			this.cancelButton.Corners.TopLeft = true;
			this.cancelButton.Corners.TopRight = true;
			this.cancelButton.IconImage = ((System.Drawing.Image)(resources.GetObject("cancelButton.IconImage")));
			this.cancelButton.IconSize = new System.Drawing.Size(16, 16);
			this.cancelButton.Large = false;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Vertical = false;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// nsSeperator1
			// 
			resources.ApplyResources(this.nsSeperator1, "nsSeperator1");
			this.nsSeperator1.Name = "nsSeperator1";
			// 
			// goPCButton
			// 
			resources.ApplyResources(this.goPCButton, "goPCButton");
			this.goPCButton.Corners.BottomLeft = false;
			this.goPCButton.Corners.BottomRight = false;
			this.goPCButton.Corners.TopLeft = false;
			this.goPCButton.Corners.TopRight = false;
			this.goPCButton.IconImage = ((System.Drawing.Image)(resources.GetObject("goPCButton.IconImage")));
			this.goPCButton.IconSize = new System.Drawing.Size(32, 32);
			this.goPCButton.Large = false;
			this.goPCButton.Name = "goPCButton";
			this.goPCButton.Vertical = true;
			this.goPCButton.Click += new System.EventHandler(this.goPCButton_Click);
			// 
			// goHomeButton
			// 
			resources.ApplyResources(this.goHomeButton, "goHomeButton");
			this.goHomeButton.Corners.BottomLeft = false;
			this.goHomeButton.Corners.BottomRight = false;
			this.goHomeButton.Corners.TopLeft = true;
			this.goHomeButton.Corners.TopRight = true;
			this.goHomeButton.IconImage = ((System.Drawing.Image)(resources.GetObject("goHomeButton.IconImage")));
			this.goHomeButton.IconSize = new System.Drawing.Size(32, 32);
			this.goHomeButton.Large = false;
			this.goHomeButton.Name = "goHomeButton";
			this.goHomeButton.Vertical = true;
			this.goHomeButton.Click += new System.EventHandler(this.goHomeButton_Click);
			// 
			// nsLabel1
			// 
			resources.ApplyResources(this.nsLabel1, "nsLabel1");
			this.nsLabel1.ForeColor = System.Drawing.Color.White;
			this.nsLabel1.Name = "nsLabel1";
			this.nsLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// directoryBrowser
			// 
			resources.ApplyResources(this.directoryBrowser, "directoryBrowser");
			this.directoryBrowser.EmptyMessage = "Папка пуста";
			this.directoryBrowser.Name = "directoryBrowser";
			this.directoryBrowser.Offset = 0;
			this.directoryBrowser.SelectedEntry = null;
			this.directoryBrowser.SelectionChanged += new Cubed.UI.Controls.NSDirectoryInspector.SelectionChangedEventHandler(this.directoryBrowser_SelectionChanged);
			this.directoryBrowser.DoubleClick += new System.EventHandler(this.directoryBrowser_DoubleClick);
			// 
			// goUpButton
			// 
			resources.ApplyResources(this.goUpButton, "goUpButton");
			this.goUpButton.Corners.BottomLeft = false;
			this.goUpButton.Corners.BottomRight = true;
			this.goUpButton.Corners.TopLeft = false;
			this.goUpButton.Corners.TopRight = true;
			this.goUpButton.IconImage = ((System.Drawing.Image)(resources.GetObject("goUpButton.IconImage")));
			this.goUpButton.IconSize = new System.Drawing.Size(16, 16);
			this.goUpButton.Large = false;
			this.goUpButton.Name = "goUpButton";
			this.goUpButton.Vertical = false;
			this.goUpButton.Click += new System.EventHandler(this.goUpButton_Click);
			// 
			// pathBox
			// 
			resources.ApplyResources(this.pathBox, "pathBox");
			this.pathBox.Corners.BottomLeft = true;
			this.pathBox.Corners.BottomRight = false;
			this.pathBox.Corners.TopLeft = true;
			this.pathBox.Corners.TopRight = false;
			this.pathBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.pathBox.MaxLength = 32767;
			this.pathBox.Multiline = false;
			this.pathBox.Name = "pathBox";
			this.pathBox.ReadOnly = false;
			this.pathBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.pathBox.UseSystemPasswordChar = false;
			this.pathBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathBox_KeyDown);
			// 
			// OpenFolderDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.Controls.Add(this.folderInfo);
			this.Controls.Add(this.goDocsButton);
			this.Controls.Add(this.goPCButton);
			this.Controls.Add(this.goHomeButton);
			this.Controls.Add(this.nsLabel1);
			this.Controls.Add(this.directoryBrowser);
			this.Controls.Add(this.goUpButton);
			this.Controls.Add(this.pathBox);
			this.Controls.Add(this.panel1);
			this.MinimizeBox = false;
			this.Name = "OpenFolderDialog";
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
		private UI.Controls.NSFileInfo folderInfo;
	}
}