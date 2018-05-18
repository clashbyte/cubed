namespace Cubed.Windows {
	partial class MapEditorForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapEditorForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.radioButton5 = new System.Windows.Forms.RadioButton();
			this.radioButton4 = new System.Windows.Forms.RadioButton();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolsSplitter = new System.Windows.Forms.SplitContainer();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.workbenchSplitter = new System.Windows.Forms.SplitContainer();
			this.screen = new Cubed.Drivers.EngineControl();
			this.panel1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.toolsSplitter)).BeginInit();
			this.toolsSplitter.Panel2.SuspendLayout();
			this.toolsSplitter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.workbenchSplitter)).BeginInit();
			this.workbenchSplitter.Panel1.SuspendLayout();
			this.workbenchSplitter.Panel2.SuspendLayout();
			this.workbenchSplitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radioButton5);
			this.panel1.Controls.Add(this.radioButton4);
			this.panel1.Controls.Add(this.radioButton3);
			this.panel1.Controls.Add(this.radioButton2);
			this.panel1.Controls.Add(this.radioButton1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(40, 395);
			this.panel1.TabIndex = 1;
			// 
			// radioButton5
			// 
			this.radioButton5.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton5.Image = ((System.Drawing.Image)(resources.GetObject("radioButton5.Image")));
			this.radioButton5.Location = new System.Drawing.Point(3, 143);
			this.radioButton5.Name = "radioButton5";
			this.radioButton5.Size = new System.Drawing.Size(34, 34);
			this.radioButton5.TabIndex = 4;
			this.radioButton5.TabStop = true;
			this.radioButton5.UseVisualStyleBackColor = true;
			// 
			// radioButton4
			// 
			this.radioButton4.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton4.Image = ((System.Drawing.Image)(resources.GetObject("radioButton4.Image")));
			this.radioButton4.Location = new System.Drawing.Point(3, 108);
			this.radioButton4.Name = "radioButton4";
			this.radioButton4.Size = new System.Drawing.Size(34, 34);
			this.radioButton4.TabIndex = 3;
			this.radioButton4.TabStop = true;
			this.radioButton4.UseVisualStyleBackColor = true;
			// 
			// radioButton3
			// 
			this.radioButton3.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton3.Image = ((System.Drawing.Image)(resources.GetObject("radioButton3.Image")));
			this.radioButton3.Location = new System.Drawing.Point(3, 73);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(34, 34);
			this.radioButton3.TabIndex = 2;
			this.radioButton3.TabStop = true;
			this.radioButton3.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			this.radioButton2.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton2.Image = ((System.Drawing.Image)(resources.GetObject("radioButton2.Image")));
			this.radioButton2.Location = new System.Drawing.Point(3, 38);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(34, 34);
			this.radioButton2.TabIndex = 1;
			this.radioButton2.TabStop = true;
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// radioButton1
			// 
			this.radioButton1.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton1.Image = ((System.Drawing.Image)(resources.GetObject("radioButton1.Image")));
			this.radioButton1.Location = new System.Drawing.Point(3, 3);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(34, 34);
			this.radioButton1.TabIndex = 0;
			this.radioButton1.TabStop = true;
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(624, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 419);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(624, 22);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolsSplitter
			// 
			this.toolsSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolsSplitter.Location = new System.Drawing.Point(0, 0);
			this.toolsSplitter.Name = "toolsSplitter";
			this.toolsSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// toolsSplitter.Panel2
			// 
			this.toolsSplitter.Panel2.Controls.Add(this.propertyGrid1);
			this.toolsSplitter.Size = new System.Drawing.Size(231, 395);
			this.toolsSplitter.SplitterDistance = 197;
			this.toolsSplitter.TabIndex = 4;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(231, 194);
			this.propertyGrid1.TabIndex = 0;
			// 
			// workbenchSplitter
			// 
			this.workbenchSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.workbenchSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.workbenchSplitter.Location = new System.Drawing.Point(40, 24);
			this.workbenchSplitter.Name = "workbenchSplitter";
			// 
			// workbenchSplitter.Panel1
			// 
			this.workbenchSplitter.Panel1.Controls.Add(this.screen);
			// 
			// workbenchSplitter.Panel2
			// 
			this.workbenchSplitter.Panel2.Controls.Add(this.toolsSplitter);
			this.workbenchSplitter.Size = new System.Drawing.Size(584, 395);
			this.workbenchSplitter.SplitterDistance = 349;
			this.workbenchSplitter.TabIndex = 5;
			// 
			// screen
			// 
			this.screen.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("screen.BackgroundImage")));
			this.screen.Display = null;
			this.screen.Dock = System.Windows.Forms.DockStyle.Fill;
			this.screen.Location = new System.Drawing.Point(0, 0);
			this.screen.Name = "screen";
			this.screen.Size = new System.Drawing.Size(349, 395);
			this.screen.TabIndex = 0;
			this.screen.Text = "engineControl1";
			// 
			// MapEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 441);
			this.Controls.Add(this.workbenchSplitter);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.statusStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "MapEditorForm";
			this.Text = "MapEditorForm";
			this.panel1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.toolsSplitter.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.toolsSplitter)).EndInit();
			this.toolsSplitter.ResumeLayout(false);
			this.workbenchSplitter.Panel1.ResumeLayout(false);
			this.workbenchSplitter.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.workbenchSplitter)).EndInit();
			this.workbenchSplitter.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Drivers.EngineControl screen;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.SplitContainer toolsSplitter;
		private System.Windows.Forms.SplitContainer workbenchSplitter;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton5;
		private System.Windows.Forms.RadioButton radioButton4;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.PropertyGrid propertyGrid1;

	}
}