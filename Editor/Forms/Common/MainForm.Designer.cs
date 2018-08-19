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
			this.nsMenuStrip1 = new Cubed.UI.Controls.NSMenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editorsControl = new Cubed.UI.Controls.NSProjectControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.filesContainer = new System.Windows.Forms.SplitContainer();
			this.propsContainer = new System.Windows.Forms.SplitContainer();
			this.inspector = new Cubed.Forms.Inspections.Inspector();
			this.panel1 = new System.Windows.Forms.Panel();
			this.projectControl = new Cubed.UI.Controls.NSDirectoryInspector();
			this.panel2 = new System.Windows.Forms.Panel();
			this.projectUpButton = new Cubed.UI.Controls.NSIconicButton();
			this.projectPath = new Cubed.UI.Controls.NSTextBox();
			this.projectFileInfo = new Cubed.UI.Controls.NSFileInfo();
			this.logicTimer = new System.Windows.Forms.Timer(this.components);
			this.nsMenuStrip1.SuspendLayout();
			this.editorsControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.filesContainer)).BeginInit();
			this.filesContainer.Panel1.SuspendLayout();
			this.filesContainer.Panel2.SuspendLayout();
			this.filesContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.propsContainer)).BeginInit();
			this.propsContainer.Panel1.SuspendLayout();
			this.propsContainer.Panel2.SuspendLayout();
			this.propsContainer.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// nsMenuStrip1
			// 
			this.nsMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.nsMenuStrip1.ForeColor = System.Drawing.Color.White;
			this.nsMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.createToolStripMenuItem,
            this.editToolStripMenuItem,
            this.testToolStripMenuItem,
            this.optionsToolStripMenuItem});
			this.nsMenuStrip1.Location = new System.Drawing.Point(0, 0);
			this.nsMenuStrip1.Name = "nsMenuStrip1";
			this.nsMenuStrip1.Size = new System.Drawing.Size(1008, 24);
			this.nsMenuStrip1.TabIndex = 0;
			this.nsMenuStrip1.Text = "menuStrip";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripSeparator1,
            this.openToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.fileToolStripMenuItem.Text = "Project";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.newToolStripMenuItem.Text = "New";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.openToolStripMenuItem.Text = "Open";
			// 
			// saveAllToolStripMenuItem
			// 
			this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
			this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.saveAllToolStripMenuItem.Text = "Save all";
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.closeToolStripMenuItem.Text = "Close";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			// 
			// createToolStripMenuItem
			// 
			this.createToolStripMenuItem.Name = "createToolStripMenuItem";
			this.createToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.createToolStripMenuItem.Text = "Create";
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.editToolStripMenuItem.Text = "Editing";
			// 
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
			this.testToolStripMenuItem.Text = "Game";
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionsToolStripMenuItem.Text = "Options";
			// 
			// editorsControl
			// 
			this.editorsControl.Controls.Add(this.tabPage1);
			this.editorsControl.Controls.Add(this.tabPage2);
			this.editorsControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editorsControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.editorsControl.ItemSize = new System.Drawing.Size(150, 26);
			this.editorsControl.Location = new System.Drawing.Point(0, 0);
			this.editorsControl.Name = "editorsControl";
			this.editorsControl.SelectedIndex = 0;
			this.editorsControl.Size = new System.Drawing.Size(700, 373);
			this.editorsControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.editorsControl.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.tabPage1.Location = new System.Drawing.Point(4, 30);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(692, 339);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.tabPage2.Location = new System.Drawing.Point(4, 30);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(0);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(734, 339);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			// 
			// filesContainer
			// 
			this.filesContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.filesContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.filesContainer.Location = new System.Drawing.Point(0, 24);
			this.filesContainer.Name = "filesContainer";
			this.filesContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// filesContainer.Panel1
			// 
			this.filesContainer.Panel1.Controls.Add(this.propsContainer);
			// 
			// filesContainer.Panel2
			// 
			this.filesContainer.Panel2.Controls.Add(this.panel1);
			this.filesContainer.Panel2.Controls.Add(this.projectFileInfo);
			this.filesContainer.Panel2MinSize = 200;
			this.filesContainer.Size = new System.Drawing.Size(1008, 537);
			this.filesContainer.SplitterDistance = 373;
			this.filesContainer.TabIndex = 2;
			// 
			// propsContainer
			// 
			this.propsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propsContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.propsContainer.Location = new System.Drawing.Point(0, 0);
			this.propsContainer.Name = "propsContainer";
			// 
			// propsContainer.Panel1
			// 
			this.propsContainer.Panel1.Controls.Add(this.editorsControl);
			// 
			// propsContainer.Panel2
			// 
			this.propsContainer.Panel2.Controls.Add(this.inspector);
			this.propsContainer.Panel2MinSize = 250;
			this.propsContainer.Size = new System.Drawing.Size(1008, 373);
			this.propsContainer.SplitterDistance = 700;
			this.propsContainer.TabIndex = 2;
			// 
			// inspector1
			// 
			this.inspector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.inspector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.inspector.Location = new System.Drawing.Point(0, 0);
			this.inspector.Name = "inspector1";
			this.inspector.Size = new System.Drawing.Size(304, 373);
			this.inspector.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.projectControl);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(888, 160);
			this.panel1.TabIndex = 2;
			// 
			// projectControl
			// 
			this.projectControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.projectControl.EmptyMessage = "Directory is empty!";
			this.projectControl.Font = new System.Drawing.Font("Tahoma", 8F);
			this.projectControl.Location = new System.Drawing.Point(0, 0);
			this.projectControl.Name = "projectControl";
			this.projectControl.Offset = 0;
			this.projectControl.SelectedEntry = null;
			this.projectControl.Size = new System.Drawing.Size(888, 123);
			this.projectControl.TabIndex = 1;
			this.projectControl.Text = "nsDirectoryInspector1";
			this.projectControl.SelectionChanged += new Cubed.UI.Controls.NSDirectoryInspector.SelectionChangedEventHandler(this.projectControl_SelectionChanged);
			this.projectControl.DoubleClick += new System.EventHandler(this.projectControl_DoubleClick);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.projectUpButton);
			this.panel2.Controls.Add(this.projectPath);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 123);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(888, 37);
			this.panel2.TabIndex = 2;
			// 
			// projectUpButton
			// 
			this.projectUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.projectUpButton.Corners.BottomLeft = false;
			this.projectUpButton.Corners.BottomRight = true;
			this.projectUpButton.Corners.TopLeft = false;
			this.projectUpButton.Corners.TopRight = true;
			this.projectUpButton.IconImage = ((System.Drawing.Image)(resources.GetObject("projectUpButton.IconImage")));
			this.projectUpButton.IconSize = new System.Drawing.Size(16, 16);
			this.projectUpButton.Large = false;
			this.projectUpButton.Location = new System.Drawing.Point(797, 6);
			this.projectUpButton.Name = "projectUpButton";
			this.projectUpButton.Size = new System.Drawing.Size(91, 25);
			this.projectUpButton.TabIndex = 8;
			this.projectUpButton.Text = "Наверх";
			this.projectUpButton.Vertical = false;
			this.projectUpButton.Click += new System.EventHandler(this.projectUpButton_Click);
			// 
			// projectPath
			// 
			this.projectPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.projectPath.Corners.BottomLeft = true;
			this.projectPath.Corners.BottomRight = false;
			this.projectPath.Corners.TopLeft = true;
			this.projectPath.Corners.TopRight = false;
			this.projectPath.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.projectPath.Location = new System.Drawing.Point(372, 6);
			this.projectPath.MaxLength = 32767;
			this.projectPath.Multiline = false;
			this.projectPath.Name = "projectPath";
			this.projectPath.ReadOnly = true;
			this.projectPath.Size = new System.Drawing.Size(429, 25);
			this.projectPath.TabIndex = 7;
			this.projectPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.projectPath.UseSystemPasswordChar = false;
			// 
			// projectFileInfo
			// 
			this.projectFileInfo.Dock = System.Windows.Forms.DockStyle.Right;
			this.projectFileInfo.File = null;
			this.projectFileInfo.IconPadding = 8;
			this.projectFileInfo.Location = new System.Drawing.Point(888, 0);
			this.projectFileInfo.Name = "projectFileInfo";
			this.projectFileInfo.Size = new System.Drawing.Size(120, 160);
			this.projectFileInfo.TabIndex = 0;
			this.projectFileInfo.Text = "nsFileInfo1";
			this.projectFileInfo.Vertical = true;
			// 
			// logicTimer
			// 
			this.logicTimer.Enabled = true;
			this.logicTimer.Interval = 10;
			this.logicTimer.Tick += new System.EventHandler(this.logicTimer_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(1008, 561);
			this.Controls.Add(this.filesContainer);
			this.Controls.Add(this.nsMenuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.nsMenuStrip1;
			this.MinimumSize = new System.Drawing.Size(800, 480);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cubed Editor";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Activated += new System.EventHandler(this.MainForm_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.nsMenuStrip1.ResumeLayout(false);
			this.nsMenuStrip1.PerformLayout();
			this.editorsControl.ResumeLayout(false);
			this.filesContainer.Panel1.ResumeLayout(false);
			this.filesContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.filesContainer)).EndInit();
			this.filesContainer.ResumeLayout(false);
			this.propsContainer.Panel1.ResumeLayout(false);
			this.propsContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.propsContainer)).EndInit();
			this.propsContainer.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Controls.NSMenuStrip nsMenuStrip1;
		private UI.Controls.NSProjectControl editorsControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
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
		private Inspections.Inspector inspector;
	}
}