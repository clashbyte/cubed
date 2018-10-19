namespace Cubed.Forms.Editors.Misc {
	partial class GamePrefsEditor {
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
			this.inspector = new Cubed.Forms.Inspections.BigInspector();
			this.SuspendLayout();
			// 
			// inspector
			// 
			this.inspector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.inspector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.inspector.Location = new System.Drawing.Point(0, 0);
			this.inspector.Name = "inspector";
			this.inspector.Size = new System.Drawing.Size(336, 278);
			this.inspector.TabIndex = 0;
			this.inspector.Target = null;
			this.inspector.FieldChanged += new System.EventHandler(this.inspector_FieldChanged);
			// 
			// GamePrefsEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(336, 278);
			this.Controls.Add(this.inspector);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "GamePrefsEditor";
			this.ResumeLayout(false);

		}

		#endregion

		private Inspections.BigInspector inspector;

	}
}