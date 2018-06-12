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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.nsMenuStrip1 = new Cubed.UI.Controls.NSMenuStrip();
			this.projectControl = new Cubed.UI.Controls.NSProjectControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.projectControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// nsMenuStrip1
			// 
			this.nsMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.nsMenuStrip1.ForeColor = System.Drawing.Color.White;
			this.nsMenuStrip1.Location = new System.Drawing.Point(0, 0);
			this.nsMenuStrip1.Name = "nsMenuStrip1";
			this.nsMenuStrip1.Size = new System.Drawing.Size(784, 24);
			this.nsMenuStrip1.TabIndex = 0;
			this.nsMenuStrip1.Text = "menuStrip";
			// 
			// projectControl
			// 
			this.projectControl.Controls.Add(this.tabPage1);
			this.projectControl.Controls.Add(this.tabPage2);
			this.projectControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.projectControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.projectControl.ItemSize = new System.Drawing.Size(96, 26);
			this.projectControl.Location = new System.Drawing.Point(0, 24);
			this.projectControl.Name = "projectControl";
			this.projectControl.SelectedIndex = 0;
			this.projectControl.Size = new System.Drawing.Size(784, 537);
			this.projectControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.projectControl.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.tabPage1.Location = new System.Drawing.Point(4, 30);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(776, 503);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(776, 508);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.projectControl);
			this.Controls.Add(this.nsMenuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.nsMenuStrip1;
			this.MinimumSize = new System.Drawing.Size(800, 480);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cubed Editor";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.projectControl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Controls.NSMenuStrip nsMenuStrip1;
		private UI.Controls.NSProjectControl projectControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
	}
}