namespace Cubed.Forms.Common
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.filesContainer = new System.Windows.Forms.SplitContainer();
			this.propsContainer = new System.Windows.Forms.SplitContainer();
			this.editorsControl = new Cubed.UI.Controls.NSProjectControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.inspector = new Cubed.Forms.Inspections.Inspector();
			this.engineControl1 = new Cubed.Drivers.EngineControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.projectControl = new Cubed.UI.Controls.NSDirectoryInspector();
			this.panel2 = new System.Windows.Forms.Panel();
			this.volumeButton = new Cubed.UI.Controls.NSCheckboxIconicButton();
			this.projectUpButton = new Cubed.UI.Controls.NSIconicButton();
			this.projectPath = new Cubed.UI.Controls.NSTextBox();
			this.projectFileInfo = new Cubed.UI.Controls.NSFileInfo();
			this.menuStrip = new Cubed.UI.Controls.NSMenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildEXEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startingPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logicTimer = new System.Windows.Forms.Timer(this.components);
			this.projectCreateMenu = new Cubed.UI.Controls.NSContextMenu();
			this.projectCreateFolderItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.projectPopupMenu = new Cubed.UI.Controls.NSContextMenu();
			this.projectCreateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.projectRenameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.projectDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.filesContainer)).BeginInit();
			this.filesContainer.Panel1.SuspendLayout();
			this.filesContainer.Panel2.SuspendLayout();
			this.filesContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.propsContainer)).BeginInit();
			this.propsContainer.Panel1.SuspendLayout();
			this.propsContainer.Panel2.SuspendLayout();
			this.propsContainer.SuspendLayout();
			this.editorsControl.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.projectCreateMenu.SuspendLayout();
			this.projectPopupMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// filesContainer
			// 
			resources.ApplyResources(this.filesContainer, "filesContainer");
			this.filesContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.filesContainer.Name = "filesContainer";
			// 
			// filesContainer.Panel1
			// 
			this.filesContainer.Panel1.Controls.Add(this.propsContainer);
			// 
			// filesContainer.Panel2
			// 
			this.filesContainer.Panel2.Controls.Add(this.engineControl1);
			this.filesContainer.Panel2.Controls.Add(this.panel1);
			this.filesContainer.Panel2.Controls.Add(this.projectFileInfo);
			// 
			// propsContainer
			// 
			resources.ApplyResources(this.propsContainer, "propsContainer");
			this.propsContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.propsContainer.Name = "propsContainer";
			// 
			// propsContainer.Panel1
			// 
			this.propsContainer.Panel1.Controls.Add(this.editorsControl);
			// 
			// propsContainer.Panel2
			// 
			this.propsContainer.Panel2.Controls.Add(this.inspector);
			// 
			// editorsControl
			// 
			this.editorsControl.Controls.Add(this.tabPage1);
			this.editorsControl.Controls.Add(this.tabPage2);
			resources.ApplyResources(this.editorsControl, "editorsControl");
			this.editorsControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.editorsControl.Name = "editorsControl";
			this.editorsControl.SelectedIndex = 0;
			this.editorsControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.editorsControl.TabClose += new Cubed.UI.Controls.NSProjectControl.TabCloseEventHandler(this.editorsControl_TabClose);
			this.editorsControl.SelectedIndexChanged += new System.EventHandler(this.editorsControl_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			resources.ApplyResources(this.tabPage1, "tabPage1");
			this.tabPage1.Name = "tabPage1";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			resources.ApplyResources(this.tabPage2, "tabPage2");
			this.tabPage2.Name = "tabPage2";
			// 
			// inspector
			// 
			this.inspector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			resources.ApplyResources(this.inspector, "inspector");
			this.inspector.InfoPanelVisible = true;
			this.inspector.Name = "inspector";
			this.inspector.Target = null;
			this.inspector.FieldChanged += new System.EventHandler(this.inspector_FieldChanged);
			// 
			// engineControl1
			// 
			this.engineControl1.Display = null;
			resources.ApplyResources(this.engineControl1, "engineControl1");
			this.engineControl1.Name = "engineControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.projectControl);
			this.panel1.Controls.Add(this.panel2);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// projectControl
			// 
			this.projectControl.AllowDragging = true;
			resources.ApplyResources(this.projectControl, "projectControl");
			this.projectControl.EmptyMessage = "Папка пуста!";
			this.projectControl.Name = "projectControl";
			this.projectControl.Offset = 0;
			this.projectControl.SelectedEntry = null;
			this.projectControl.SelectionChanged += new Cubed.UI.Controls.NSDirectoryInspector.SelectionChangedEventHandler(this.projectControl_SelectionChanged);
			this.projectControl.DoubleClick += new System.EventHandler(this.projectControl_DoubleClick);
			this.projectControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.projectControl_MouseClick);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.volumeButton);
			this.panel2.Controls.Add(this.projectUpButton);
			this.panel2.Controls.Add(this.projectPath);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// volumeButton
			// 
			this.volumeButton.Checked = false;
			this.volumeButton.Corners.BottomLeft = true;
			this.volumeButton.Corners.BottomRight = true;
			this.volumeButton.Corners.TopLeft = true;
			this.volumeButton.Corners.TopRight = true;
			this.volumeButton.IconImage = ((System.Drawing.Image)(resources.GetObject("volumeButton.IconImage")));
			this.volumeButton.IconSize = new System.Drawing.Size(12, 12);
			this.volumeButton.Large = false;
			resources.ApplyResources(this.volumeButton, "volumeButton");
			this.volumeButton.Name = "volumeButton";
			this.volumeButton.CheckedChanged += new Cubed.UI.Controls.NSCheckboxIconicButton.CheckedChangedEventHandler(this.volumeButton_CheckedChanged);
			// 
			// projectUpButton
			// 
			resources.ApplyResources(this.projectUpButton, "projectUpButton");
			this.projectUpButton.Corners.BottomLeft = false;
			this.projectUpButton.Corners.BottomRight = true;
			this.projectUpButton.Corners.TopLeft = false;
			this.projectUpButton.Corners.TopRight = true;
			this.projectUpButton.IconImage = ((System.Drawing.Image)(resources.GetObject("projectUpButton.IconImage")));
			this.projectUpButton.IconSize = new System.Drawing.Size(16, 16);
			this.projectUpButton.Large = false;
			this.projectUpButton.Name = "projectUpButton";
			this.projectUpButton.Vertical = false;
			this.projectUpButton.Click += new System.EventHandler(this.projectUpButton_Click);
			// 
			// projectPath
			// 
			resources.ApplyResources(this.projectPath, "projectPath");
			this.projectPath.Corners.BottomLeft = true;
			this.projectPath.Corners.BottomRight = false;
			this.projectPath.Corners.TopLeft = true;
			this.projectPath.Corners.TopRight = false;
			this.projectPath.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.projectPath.MaxLength = 32767;
			this.projectPath.Multiline = false;
			this.projectPath.Name = "projectPath";
			this.projectPath.ReadOnly = true;
			this.projectPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.projectPath.UseSystemPasswordChar = false;
			// 
			// projectFileInfo
			// 
			resources.ApplyResources(this.projectFileInfo, "projectFileInfo");
			this.projectFileInfo.File = null;
			this.projectFileInfo.IconPadding = 8;
			this.projectFileInfo.Name = "projectFileInfo";
			this.projectFileInfo.Vertical = true;
			// 
			// menuStrip
			// 
			this.menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.menuStrip.ForeColor = System.Drawing.Color.White;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.createToolStripMenuItem,
            this.gameToolStripMenuItem,
            this.infoToolStripMenuItem});
			resources.ApplyResources(this.menuStrip, "menuStrip");
			this.menuStrip.Name = "menuStrip";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeProjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveAllToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
			// 
			// closeProjectToolStripMenuItem
			// 
			this.closeProjectToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
			resources.ApplyResources(this.closeProjectToolStripMenuItem, "closeProjectToolStripMenuItem");
			this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.ForeColor = System.Drawing.Color.White;
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// saveAllToolStripMenuItem
			// 
			this.saveAllToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
			resources.ApplyResources(this.saveAllToolStripMenuItem, "saveAllToolStripMenuItem");
			this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			resources.ApplyResources(this.closeToolStripMenuItem, "closeToolStripMenuItem");
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.ForeColor = System.Drawing.Color.White;
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator3,
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator5,
            this.preferencesToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			resources.ApplyResources(this.undoToolStripMenuItem, "undoToolStripMenuItem");
			this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
			// 
			// redoToolStripMenuItem
			// 
			this.redoToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
			resources.ApplyResources(this.redoToolStripMenuItem, "redoToolStripMenuItem");
			this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
			this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
			// 
			// preferencesToolStripMenuItem
			// 
			this.preferencesToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			resources.ApplyResources(this.preferencesToolStripMenuItem, "preferencesToolStripMenuItem");
			this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
			// 
			// createToolStripMenuItem
			// 
			this.createToolStripMenuItem.Name = "createToolStripMenuItem";
			resources.ApplyResources(this.createToolStripMenuItem, "createToolStripMenuItem");
			this.createToolStripMenuItem.DropDownOpening += new System.EventHandler(this.createToolStripMenuItem_DropDownOpening);
			// 
			// gameToolStripMenuItem
			// 
			this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildToolStripMenuItem,
            this.buildEXEToolStripMenuItem,
            this.toolStripSeparator6,
            this.propertiesToolStripMenuItem});
			this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
			resources.ApplyResources(this.gameToolStripMenuItem, "gameToolStripMenuItem");
			// 
			// buildToolStripMenuItem
			// 
			this.buildToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
			resources.ApplyResources(this.buildToolStripMenuItem, "buildToolStripMenuItem");
			this.buildToolStripMenuItem.Click += new System.EventHandler(this.buildToolStripMenuItem_Click);
			// 
			// buildEXEToolStripMenuItem
			// 
			this.buildEXEToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.buildEXEToolStripMenuItem.Name = "buildEXEToolStripMenuItem";
			resources.ApplyResources(this.buildEXEToolStripMenuItem, "buildEXEToolStripMenuItem");
			this.buildEXEToolStripMenuItem.Click += new System.EventHandler(this.buildEXEToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
			// 
			// propertiesToolStripMenuItem
			// 
			this.propertiesToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
			resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
			this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
			// 
			// infoToolStripMenuItem
			// 
			this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startingPageToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
			resources.ApplyResources(this.infoToolStripMenuItem, "infoToolStripMenuItem");
			// 
			// startingPageToolStripMenuItem
			// 
			this.startingPageToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.startingPageToolStripMenuItem.Name = "startingPageToolStripMenuItem";
			resources.ApplyResources(this.startingPageToolStripMenuItem, "startingPageToolStripMenuItem");
			this.startingPageToolStripMenuItem.Click += new System.EventHandler(this.startingPageToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
			// 
			// logicTimer
			// 
			this.logicTimer.Enabled = true;
			this.logicTimer.Interval = 10;
			this.logicTimer.Tick += new System.EventHandler(this.logicTimer_Tick);
			// 
			// projectCreateMenu
			// 
			this.projectCreateMenu.ForeColor = System.Drawing.Color.White;
			this.projectCreateMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectCreateFolderItem,
            this.toolStripSeparator7});
			this.projectCreateMenu.Name = "projectCreateMenu";
			resources.ApplyResources(this.projectCreateMenu, "projectCreateMenu");
			// 
			// projectCreateFolderItem
			// 
			this.projectCreateFolderItem.Name = "projectCreateFolderItem";
			resources.ApplyResources(this.projectCreateFolderItem, "projectCreateFolderItem");
			this.projectCreateFolderItem.Click += new System.EventHandler(this.contextMenuCreateItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
			// 
			// projectPopupMenu
			// 
			this.projectPopupMenu.ForeColor = System.Drawing.Color.White;
			this.projectPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectCreateMenuItem,
            this.openInExplorerToolStripMenuItem,
            this.toolStripSeparator4,
            this.projectRenameMenuItem,
            this.projectDeleteMenuItem});
			this.projectPopupMenu.Name = "projectPopupMenu";
			resources.ApplyResources(this.projectPopupMenu, "projectPopupMenu");
			// 
			// projectCreateMenuItem
			// 
			this.projectCreateMenuItem.Name = "projectCreateMenuItem";
			resources.ApplyResources(this.projectCreateMenuItem, "projectCreateMenuItem");
			// 
			// openInExplorerToolStripMenuItem
			// 
			this.openInExplorerToolStripMenuItem.Name = "openInExplorerToolStripMenuItem";
			resources.ApplyResources(this.openInExplorerToolStripMenuItem, "openInExplorerToolStripMenuItem");
			this.openInExplorerToolStripMenuItem.Click += new System.EventHandler(this.openInExplorerToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
			// 
			// projectRenameMenuItem
			// 
			this.projectRenameMenuItem.Name = "projectRenameMenuItem";
			resources.ApplyResources(this.projectRenameMenuItem, "projectRenameMenuItem");
			this.projectRenameMenuItem.Click += new System.EventHandler(this.projectRenameMenuItem_Click);
			// 
			// projectDeleteMenuItem
			// 
			this.projectDeleteMenuItem.Name = "projectDeleteMenuItem";
			resources.ApplyResources(this.projectDeleteMenuItem, "projectDeleteMenuItem");
			this.projectDeleteMenuItem.Click += new System.EventHandler(this.projectDeleteMenuItem_Click);
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.Controls.Add(this.filesContainer);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Activated += new System.EventHandler(this.MainForm_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.filesContainer.Panel1.ResumeLayout(false);
			this.filesContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.filesContainer)).EndInit();
			this.filesContainer.ResumeLayout(false);
			this.propsContainer.Panel1.ResumeLayout(false);
			this.propsContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.propsContainer)).EndInit();
			this.propsContainer.ResumeLayout(false);
			this.editorsControl.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.projectCreateMenu.ResumeLayout(false);
			this.projectPopupMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Controls.NSMenuStrip menuStrip;
		private UI.Controls.NSProjectControl editorsControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.SplitContainer filesContainer;
		private System.Windows.Forms.SplitContainer propsContainer;
		private UI.Controls.NSDirectoryInspector projectControl;
		private UI.Controls.NSFileInfo projectFileInfo;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Timer logicTimer;
		private UI.Controls.NSIconicButton projectUpButton;
		private UI.Controls.NSTextBox projectPath;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildEXEToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
		private Inspections.Inspector inspector;
		private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startingPageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private UI.Controls.NSContextMenu projectCreateMenu;
		private System.Windows.Forms.ToolStripMenuItem projectCreateFolderItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private UI.Controls.NSContextMenu projectPopupMenu;
		private System.Windows.Forms.ToolStripMenuItem projectCreateMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem projectRenameMenuItem;
		private System.Windows.Forms.ToolStripMenuItem projectDeleteMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openInExplorerToolStripMenuItem;
		private Drivers.EngineControl engineControl1;
		private UI.Controls.NSCheckboxIconicButton volumeButton;
	}
}