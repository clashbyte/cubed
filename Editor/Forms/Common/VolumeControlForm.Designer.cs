namespace Cubed.Forms.Common {
	partial class VolumeControlForm {
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
			this.musicVolume = new Cubed.UI.Controls.NSTrackBar();
			this.nsLabel2 = new Cubed.UI.Controls.NSLabel();
			this.nsLabel1 = new Cubed.UI.Controls.NSLabel();
			this.soundsVolume = new Cubed.UI.Controls.NSTrackBar();
			this.SuspendLayout();
			// 
			// musicVolume
			// 
			this.musicVolume.Location = new System.Drawing.Point(68, 41);
			this.musicVolume.Maximum = 100;
			this.musicVolume.Minimum = 0;
			this.musicVolume.Name = "musicVolume";
			this.musicVolume.Size = new System.Drawing.Size(148, 23);
			this.musicVolume.TabIndex = 3;
			this.musicVolume.Text = "nsTrackBar2";
			this.musicVolume.Value = 100;
			// 
			// nsLabel2
			// 
			this.nsLabel2.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.nsLabel2.ForeColor = System.Drawing.Color.White;
			this.nsLabel2.Location = new System.Drawing.Point(12, 41);
			this.nsLabel2.Name = "nsLabel2";
			this.nsLabel2.Size = new System.Drawing.Size(50, 23);
			this.nsLabel2.TabIndex = 2;
			this.nsLabel2.Text = "Music:";
			this.nsLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// nsLabel1
			// 
			this.nsLabel1.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.nsLabel1.ForeColor = System.Drawing.Color.White;
			this.nsLabel1.Location = new System.Drawing.Point(12, 12);
			this.nsLabel1.Name = "nsLabel1";
			this.nsLabel1.Size = new System.Drawing.Size(50, 23);
			this.nsLabel1.TabIndex = 1;
			this.nsLabel1.Text = "Sounds:";
			this.nsLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// soundsVolume
			// 
			this.soundsVolume.Location = new System.Drawing.Point(68, 12);
			this.soundsVolume.Maximum = 100;
			this.soundsVolume.Minimum = 0;
			this.soundsVolume.Name = "soundsVolume";
			this.soundsVolume.Size = new System.Drawing.Size(148, 23);
			this.soundsVolume.TabIndex = 0;
			this.soundsVolume.Text = "nsTrackBar1";
			this.soundsVolume.Value = 100;
			// 
			// VolumeControlForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(231, 75);
			this.ControlBox = false;
			this.Controls.Add(this.musicVolume);
			this.Controls.Add(this.nsLabel2);
			this.Controls.Add(this.nsLabel1);
			this.Controls.Add(this.soundsVolume);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "VolumeControlForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "VolumeControlForm";
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Controls.NSTrackBar soundsVolume;
		private UI.Controls.NSLabel nsLabel1;
		private UI.Controls.NSLabel nsLabel2;
		private UI.Controls.NSTrackBar musicVolume;
	}
}