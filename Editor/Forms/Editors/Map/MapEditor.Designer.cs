namespace Cubed.Forms.Editors.Map {
	partial class MapEditor {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapEditor));
			this.listContainer = new System.Windows.Forms.SplitContainer();
			this.screen = new Cubed.Drivers.EngineControl();
			this.entityList = new Cubed.UI.Controls.NSDirectoryInspector();
			this.panel1 = new System.Windows.Forms.Panel();
			this.toolLogics = new Cubed.UI.Controls.NSRadioIconicButton();
			this.toolPaint = new Cubed.UI.Controls.NSRadioIconicButton();
			this.toolHeightmap = new Cubed.UI.Controls.NSRadioIconicButton();
			this.toolFloors = new Cubed.UI.Controls.NSRadioIconicButton();
			this.toolWalls = new Cubed.UI.Controls.NSRadioIconicButton();
			this.toolSelect = new Cubed.UI.Controls.NSRadioIconicButton();
			this.panel2 = new System.Windows.Forms.Panel();
			this.soundsEnabledFlag = new Cubed.UI.Controls.NSCheckboxIconicButton();
			this.lightsEnabledFlag = new Cubed.UI.Controls.NSCheckboxIconicButton();
			this.skyboxEnabledFlag = new Cubed.UI.Controls.NSCheckboxIconicButton();
			this.scriptOptionsButton = new Cubed.UI.Controls.NSIconicButton();
			this.envOptionsButton = new Cubed.UI.Controls.NSIconicButton();
			this.floorDown = new Cubed.UI.Controls.NSIconicButton();
			this.floorUp = new Cubed.UI.Controls.NSIconicButton();
			this.walkModeEnable = new Cubed.UI.Controls.NSCheckboxIconicButton();
			this.floorIndex = new Cubed.UI.Controls.NSLabel();
			((System.ComponentModel.ISupportInitialize)(this.listContainer)).BeginInit();
			this.listContainer.Panel1.SuspendLayout();
			this.listContainer.Panel2.SuspendLayout();
			this.listContainer.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// listContainer
			// 
			resources.ApplyResources(this.listContainer, "listContainer");
			this.listContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.listContainer.Name = "listContainer";
			// 
			// listContainer.Panel1
			// 
			this.listContainer.Panel1.Controls.Add(this.screen);
			// 
			// listContainer.Panel2
			// 
			this.listContainer.Panel2.Controls.Add(this.entityList);
			// 
			// screen
			// 
			this.screen.Display = null;
			resources.ApplyResources(this.screen, "screen");
			this.screen.Name = "screen";
			this.screen.DragDrop += new System.Windows.Forms.DragEventHandler(this.screen_DragDrop);
			this.screen.DragEnter += new System.Windows.Forms.DragEventHandler(this.screen_DragEnter);
			this.screen.DragOver += new System.Windows.Forms.DragEventHandler(this.screen_DragOver);
			this.screen.DragLeave += new System.EventHandler(this.screen_DragLeave);
			// 
			// entityList
			// 
			this.entityList.AllowDragging = true;
			resources.ApplyResources(this.entityList, "entityList");
			this.entityList.EmptyMessage = null;
			this.entityList.Name = "entityList";
			this.entityList.Offset = 0;
			this.entityList.SelectedEntry = null;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.panel1.Controls.Add(this.toolLogics);
			this.panel1.Controls.Add(this.toolPaint);
			this.panel1.Controls.Add(this.toolHeightmap);
			this.panel1.Controls.Add(this.toolFloors);
			this.panel1.Controls.Add(this.toolWalls);
			this.panel1.Controls.Add(this.toolSelect);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// toolLogics
			// 
			this.toolLogics.Checked = false;
			this.toolLogics.Corners.BottomLeft = true;
			this.toolLogics.Corners.BottomRight = true;
			this.toolLogics.Corners.TopLeft = false;
			this.toolLogics.Corners.TopRight = false;
			this.toolLogics.IconImage = ((System.Drawing.Image)(resources.GetObject("toolLogics.IconImage")));
			this.toolLogics.IconSize = new System.Drawing.Size(32, 32);
			this.toolLogics.Large = true;
			resources.ApplyResources(this.toolLogics, "toolLogics");
			this.toolLogics.Name = "toolLogics";
			this.toolLogics.CheckedChanged += new Cubed.UI.Controls.NSRadioIconicButton.CheckedChangedEventHandler(this.tool_CheckedChanged);
			// 
			// toolPaint
			// 
			this.toolPaint.Checked = false;
			this.toolPaint.Corners.BottomLeft = false;
			this.toolPaint.Corners.BottomRight = false;
			this.toolPaint.Corners.TopLeft = true;
			this.toolPaint.Corners.TopRight = true;
			this.toolPaint.IconImage = ((System.Drawing.Image)(resources.GetObject("toolPaint.IconImage")));
			this.toolPaint.IconSize = new System.Drawing.Size(32, 32);
			this.toolPaint.Large = true;
			resources.ApplyResources(this.toolPaint, "toolPaint");
			this.toolPaint.Name = "toolPaint";
			this.toolPaint.CheckedChanged += new Cubed.UI.Controls.NSRadioIconicButton.CheckedChangedEventHandler(this.tool_CheckedChanged);
			// 
			// toolHeightmap
			// 
			this.toolHeightmap.Checked = false;
			this.toolHeightmap.Corners.BottomLeft = true;
			this.toolHeightmap.Corners.BottomRight = true;
			this.toolHeightmap.Corners.TopLeft = false;
			this.toolHeightmap.Corners.TopRight = false;
			this.toolHeightmap.IconImage = ((System.Drawing.Image)(resources.GetObject("toolHeightmap.IconImage")));
			this.toolHeightmap.IconSize = new System.Drawing.Size(32, 32);
			this.toolHeightmap.Large = true;
			resources.ApplyResources(this.toolHeightmap, "toolHeightmap");
			this.toolHeightmap.Name = "toolHeightmap";
			this.toolHeightmap.CheckedChanged += new Cubed.UI.Controls.NSRadioIconicButton.CheckedChangedEventHandler(this.tool_CheckedChanged);
			// 
			// toolFloors
			// 
			this.toolFloors.Checked = false;
			this.toolFloors.Corners.BottomLeft = false;
			this.toolFloors.Corners.BottomRight = false;
			this.toolFloors.Corners.TopLeft = false;
			this.toolFloors.Corners.TopRight = false;
			this.toolFloors.IconImage = ((System.Drawing.Image)(resources.GetObject("toolFloors.IconImage")));
			this.toolFloors.IconSize = new System.Drawing.Size(32, 32);
			this.toolFloors.Large = true;
			resources.ApplyResources(this.toolFloors, "toolFloors");
			this.toolFloors.Name = "toolFloors";
			this.toolFloors.CheckedChanged += new Cubed.UI.Controls.NSRadioIconicButton.CheckedChangedEventHandler(this.tool_CheckedChanged);
			// 
			// toolWalls
			// 
			this.toolWalls.Checked = false;
			this.toolWalls.Corners.BottomLeft = false;
			this.toolWalls.Corners.BottomRight = false;
			this.toolWalls.Corners.TopLeft = true;
			this.toolWalls.Corners.TopRight = true;
			this.toolWalls.IconImage = ((System.Drawing.Image)(resources.GetObject("toolWalls.IconImage")));
			this.toolWalls.IconSize = new System.Drawing.Size(32, 32);
			this.toolWalls.Large = true;
			resources.ApplyResources(this.toolWalls, "toolWalls");
			this.toolWalls.Name = "toolWalls";
			this.toolWalls.CheckedChanged += new Cubed.UI.Controls.NSRadioIconicButton.CheckedChangedEventHandler(this.tool_CheckedChanged);
			// 
			// toolSelect
			// 
			this.toolSelect.Checked = true;
			this.toolSelect.Corners.BottomLeft = true;
			this.toolSelect.Corners.BottomRight = true;
			this.toolSelect.Corners.TopLeft = true;
			this.toolSelect.Corners.TopRight = true;
			this.toolSelect.IconImage = ((System.Drawing.Image)(resources.GetObject("toolSelect.IconImage")));
			this.toolSelect.IconSize = new System.Drawing.Size(32, 32);
			this.toolSelect.Large = true;
			resources.ApplyResources(this.toolSelect, "toolSelect");
			this.toolSelect.Name = "toolSelect";
			this.toolSelect.CheckedChanged += new Cubed.UI.Controls.NSRadioIconicButton.CheckedChangedEventHandler(this.tool_CheckedChanged);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.soundsEnabledFlag);
			this.panel2.Controls.Add(this.lightsEnabledFlag);
			this.panel2.Controls.Add(this.skyboxEnabledFlag);
			this.panel2.Controls.Add(this.scriptOptionsButton);
			this.panel2.Controls.Add(this.envOptionsButton);
			this.panel2.Controls.Add(this.floorDown);
			this.panel2.Controls.Add(this.floorUp);
			this.panel2.Controls.Add(this.walkModeEnable);
			this.panel2.Controls.Add(this.floorIndex);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// soundsEnabledFlag
			// 
			this.soundsEnabledFlag.Checked = true;
			this.soundsEnabledFlag.Corners.BottomLeft = false;
			this.soundsEnabledFlag.Corners.BottomRight = true;
			this.soundsEnabledFlag.Corners.TopLeft = false;
			this.soundsEnabledFlag.Corners.TopRight = true;
			this.soundsEnabledFlag.IconImage = ((System.Drawing.Image)(resources.GetObject("soundsEnabledFlag.IconImage")));
			this.soundsEnabledFlag.IconSize = new System.Drawing.Size(12, 12);
			this.soundsEnabledFlag.Large = false;
			resources.ApplyResources(this.soundsEnabledFlag, "soundsEnabledFlag");
			this.soundsEnabledFlag.Name = "soundsEnabledFlag";
			this.soundsEnabledFlag.CheckedChanged += new Cubed.UI.Controls.NSCheckboxIconicButton.CheckedChangedEventHandler(this.soundsEnabledFlag_CheckedChanged);
			// 
			// lightsEnabledFlag
			// 
			this.lightsEnabledFlag.Checked = true;
			this.lightsEnabledFlag.Corners.BottomLeft = false;
			this.lightsEnabledFlag.Corners.BottomRight = false;
			this.lightsEnabledFlag.Corners.TopLeft = false;
			this.lightsEnabledFlag.Corners.TopRight = false;
			this.lightsEnabledFlag.IconImage = ((System.Drawing.Image)(resources.GetObject("lightsEnabledFlag.IconImage")));
			this.lightsEnabledFlag.IconSize = new System.Drawing.Size(12, 12);
			this.lightsEnabledFlag.Large = false;
			resources.ApplyResources(this.lightsEnabledFlag, "lightsEnabledFlag");
			this.lightsEnabledFlag.Name = "lightsEnabledFlag";
			this.lightsEnabledFlag.CheckedChanged += new Cubed.UI.Controls.NSCheckboxIconicButton.CheckedChangedEventHandler(this.lightsEnabledFlag_CheckedChanged);
			// 
			// skyboxEnabledFlag
			// 
			this.skyboxEnabledFlag.Checked = true;
			this.skyboxEnabledFlag.Corners.BottomLeft = true;
			this.skyboxEnabledFlag.Corners.BottomRight = false;
			this.skyboxEnabledFlag.Corners.TopLeft = true;
			this.skyboxEnabledFlag.Corners.TopRight = false;
			this.skyboxEnabledFlag.IconImage = ((System.Drawing.Image)(resources.GetObject("skyboxEnabledFlag.IconImage")));
			this.skyboxEnabledFlag.IconSize = new System.Drawing.Size(12, 12);
			this.skyboxEnabledFlag.Large = false;
			resources.ApplyResources(this.skyboxEnabledFlag, "skyboxEnabledFlag");
			this.skyboxEnabledFlag.Name = "skyboxEnabledFlag";
			this.skyboxEnabledFlag.CheckedChanged += new Cubed.UI.Controls.NSCheckboxIconicButton.CheckedChangedEventHandler(this.skyboxEnabledFlag_CheckedChanged);
			// 
			// scriptOptionsButton
			// 
			this.scriptOptionsButton.Corners.BottomLeft = false;
			this.scriptOptionsButton.Corners.BottomRight = true;
			this.scriptOptionsButton.Corners.TopLeft = false;
			this.scriptOptionsButton.Corners.TopRight = true;
			this.scriptOptionsButton.IconImage = ((System.Drawing.Image)(resources.GetObject("scriptOptionsButton.IconImage")));
			this.scriptOptionsButton.IconSize = new System.Drawing.Size(12, 12);
			this.scriptOptionsButton.Large = false;
			resources.ApplyResources(this.scriptOptionsButton, "scriptOptionsButton");
			this.scriptOptionsButton.Name = "scriptOptionsButton";
			this.scriptOptionsButton.Vertical = false;
			// 
			// envOptionsButton
			// 
			this.envOptionsButton.Corners.BottomLeft = true;
			this.envOptionsButton.Corners.BottomRight = false;
			this.envOptionsButton.Corners.TopLeft = true;
			this.envOptionsButton.Corners.TopRight = false;
			this.envOptionsButton.IconImage = ((System.Drawing.Image)(resources.GetObject("envOptionsButton.IconImage")));
			this.envOptionsButton.IconSize = new System.Drawing.Size(12, 12);
			this.envOptionsButton.Large = false;
			resources.ApplyResources(this.envOptionsButton, "envOptionsButton");
			this.envOptionsButton.Name = "envOptionsButton";
			this.envOptionsButton.Vertical = false;
			this.envOptionsButton.Click += new System.EventHandler(this.envOptionsButton_Click);
			// 
			// floorDown
			// 
			resources.ApplyResources(this.floorDown, "floorDown");
			this.floorDown.Corners.BottomLeft = true;
			this.floorDown.Corners.BottomRight = true;
			this.floorDown.Corners.TopLeft = true;
			this.floorDown.Corners.TopRight = true;
			this.floorDown.IconImage = ((System.Drawing.Image)(resources.GetObject("floorDown.IconImage")));
			this.floorDown.IconSize = new System.Drawing.Size(12, 12);
			this.floorDown.Large = false;
			this.floorDown.Name = "floorDown";
			this.floorDown.Vertical = false;
			this.floorDown.Click += new System.EventHandler(this.floorDown_Click);
			// 
			// floorUp
			// 
			resources.ApplyResources(this.floorUp, "floorUp");
			this.floorUp.Corners.BottomLeft = true;
			this.floorUp.Corners.BottomRight = true;
			this.floorUp.Corners.TopLeft = true;
			this.floorUp.Corners.TopRight = true;
			this.floorUp.IconImage = ((System.Drawing.Image)(resources.GetObject("floorUp.IconImage")));
			this.floorUp.IconSize = new System.Drawing.Size(12, 12);
			this.floorUp.Large = false;
			this.floorUp.Name = "floorUp";
			this.floorUp.Vertical = false;
			this.floorUp.Click += new System.EventHandler(this.floorUp_Click);
			// 
			// walkModeEnable
			// 
			resources.ApplyResources(this.walkModeEnable, "walkModeEnable");
			this.walkModeEnable.Checked = false;
			this.walkModeEnable.Corners.BottomLeft = true;
			this.walkModeEnable.Corners.BottomRight = true;
			this.walkModeEnable.Corners.TopLeft = true;
			this.walkModeEnable.Corners.TopRight = true;
			this.walkModeEnable.IconImage = ((System.Drawing.Image)(resources.GetObject("walkModeEnable.IconImage")));
			this.walkModeEnable.IconSize = new System.Drawing.Size(12, 12);
			this.walkModeEnable.Large = false;
			this.walkModeEnable.Name = "walkModeEnable";
			this.walkModeEnable.CheckedChanged += new Cubed.UI.Controls.NSCheckboxIconicButton.CheckedChangedEventHandler(this.walkModeEnable_CheckedChanged);
			// 
			// floorIndex
			// 
			resources.ApplyResources(this.floorIndex, "floorIndex");
			this.floorIndex.ForeColor = System.Drawing.Color.White;
			this.floorIndex.Name = "floorIndex";
			this.floorIndex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MapEditor
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.Controls.Add(this.listContainer);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "MapEditor";
			this.listContainer.Panel1.ResumeLayout(false);
			this.listContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.listContainer)).EndInit();
			this.listContainer.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private UI.Controls.NSRadioIconicButton toolSelect;
		private UI.Controls.NSRadioIconicButton toolPaint;
		private UI.Controls.NSRadioIconicButton toolHeightmap;
		private UI.Controls.NSRadioIconicButton toolFloors;
		private UI.Controls.NSRadioIconicButton toolWalls;
		private Drivers.EngineControl screen;
		private System.Windows.Forms.Panel panel2;
		private UI.Controls.NSRadioIconicButton toolLogics;
		private UI.Controls.NSIconicButton floorDown;
		private UI.Controls.NSIconicButton floorUp;
		private UI.Controls.NSLabel floorIndex;
		private UI.Controls.NSCheckboxIconicButton walkModeEnable;
		private UI.Controls.NSIconicButton scriptOptionsButton;
		private UI.Controls.NSIconicButton envOptionsButton;
		private System.Windows.Forms.SplitContainer listContainer;
		private UI.Controls.NSDirectoryInspector entityList;
		private UI.Controls.NSCheckboxIconicButton soundsEnabledFlag;
		private UI.Controls.NSCheckboxIconicButton lightsEnabledFlag;
		private UI.Controls.NSCheckboxIconicButton skyboxEnabledFlag;
	}
}