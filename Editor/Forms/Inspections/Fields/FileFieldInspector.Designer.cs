namespace Cubed.Forms.Inspections.Fields {
	partial class FileFieldInspector {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.fileDropper = new Cubed.UI.Controls.NSFileDropControl();
			this.SuspendLayout();
			// 
			// fileDropper
			// 
			this.fileDropper.AllowDrop = true;
			this.fileDropper.AllowedTypes = ".png|.jpg|.jpeg|.gif|.bmp";
			this.fileDropper.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.fileDropper.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.fileDropper.File = null;
			this.fileDropper.Font = new System.Drawing.Font("Tahoma", 8F);
			this.fileDropper.Location = new System.Drawing.Point(269, 3);
			this.fileDropper.Name = "fileDropper";
			this.fileDropper.Size = new System.Drawing.Size(88, 100);
			this.fileDropper.TabIndex = 0;
			this.fileDropper.Text = "nsFileDropControl1";
			this.fileDropper.FileChanged += new System.EventHandler(this.fileDropper_FileChanged);
			// 
			// FileFieldInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.fileDropper);
			this.Name = "FileFieldInspector";
			this.Size = new System.Drawing.Size(360, 106);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Controls.NSFileDropControl fileDropper;
	}
}
