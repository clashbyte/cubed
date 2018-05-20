namespace Cubed.Forms.Common {
	partial class StartupForm {
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
			this.nsIconicButton2 = new Cubed.UI.Controls.NSIconicButton();
			this.nsIconicButton1 = new Cubed.UI.Controls.NSIconicButton();
			this.SuspendLayout();
			// 
			// nsIconicButton2
			// 
			this.nsIconicButton2.Corners.BottomLeft = true;
			this.nsIconicButton2.Corners.BottomRight = true;
			this.nsIconicButton2.Corners.TopLeft = false;
			this.nsIconicButton2.Corners.TopRight = false;
			this.nsIconicButton2.IconImage = null;
			this.nsIconicButton2.IconSize = new System.Drawing.Size(0, 0);
			this.nsIconicButton2.Large = false;
			this.nsIconicButton2.Location = new System.Drawing.Point(351, 46);
			this.nsIconicButton2.Name = "nsIconicButton2";
			this.nsIconicButton2.Size = new System.Drawing.Size(200, 35);
			this.nsIconicButton2.TabIndex = 2;
			this.nsIconicButton2.Text = "nsIconicButton2";
			// 
			// nsIconicButton1
			// 
			this.nsIconicButton1.Corners.BottomLeft = false;
			this.nsIconicButton1.Corners.BottomRight = false;
			this.nsIconicButton1.Corners.TopLeft = true;
			this.nsIconicButton1.Corners.TopRight = true;
			this.nsIconicButton1.IconImage = null;
			this.nsIconicButton1.IconSize = new System.Drawing.Size(0, 0);
			this.nsIconicButton1.Large = false;
			this.nsIconicButton1.Location = new System.Drawing.Point(351, 12);
			this.nsIconicButton1.Name = "nsIconicButton1";
			this.nsIconicButton1.Size = new System.Drawing.Size(200, 35);
			this.nsIconicButton1.TabIndex = 1;
			this.nsIconicButton1.Text = "nsIconicButton1";
			// 
			// StartupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.ClientSize = new System.Drawing.Size(563, 345);
			this.Controls.Add(this.nsIconicButton2);
			this.Controls.Add(this.nsIconicButton1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StartupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "StartupForm";
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Controls.NSIconicButton nsIconicButton1;
		private UI.Controls.NSIconicButton nsIconicButton2;

	}
}