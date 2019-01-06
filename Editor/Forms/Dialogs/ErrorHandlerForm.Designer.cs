namespace Cubed.Forms.Dialogs {
	partial class ErrorHandlerForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorHandlerForm));
			this.errorInfo = new Cubed.UI.Controls.NSTextBox();
			this.descLabel = new Cubed.UI.Controls.NSLabel();
			this.submitButton = new Cubed.UI.Controls.NSIconicButton();
			this.nsSeperator1 = new Cubed.UI.Controls.NSSeperator();
			this.SuspendLayout();
			// 
			// errorInfo
			// 
			resources.ApplyResources(this.errorInfo, "errorInfo");
			this.errorInfo.Corners.BottomLeft = true;
			this.errorInfo.Corners.BottomRight = true;
			this.errorInfo.Corners.TopLeft = true;
			this.errorInfo.Corners.TopRight = true;
			this.errorInfo.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.errorInfo.ForeColor = System.Drawing.Color.White;
			this.errorInfo.MaxLength = 32767;
			this.errorInfo.Multiline = true;
			this.errorInfo.Name = "errorInfo";
			this.errorInfo.ReadOnly = true;
			this.errorInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.errorInfo.UseSystemPasswordChar = false;
			// 
			// descLabel
			// 
			resources.ApplyResources(this.descLabel, "descLabel");
			this.descLabel.ForeColor = System.Drawing.Color.White;
			this.descLabel.Name = "descLabel";
			this.descLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// submitButton
			// 
			resources.ApplyResources(this.submitButton, "submitButton");
			this.submitButton.Corners.BottomLeft = true;
			this.submitButton.Corners.BottomRight = true;
			this.submitButton.Corners.TopLeft = true;
			this.submitButton.Corners.TopRight = true;
			this.submitButton.IconImage = ((System.Drawing.Image)(resources.GetObject("submitButton.IconImage")));
			this.submitButton.IconSize = new System.Drawing.Size(16, 16);
			this.submitButton.Large = false;
			this.submitButton.Name = "submitButton";
			this.submitButton.Vertical = false;
			this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
			// 
			// nsSeperator1
			// 
			resources.ApplyResources(this.nsSeperator1, "nsSeperator1");
			this.nsSeperator1.Name = "nsSeperator1";
			// 
			// ErrorHandlerForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.Controls.Add(this.errorInfo);
			this.Controls.Add(this.descLabel);
			this.Controls.Add(this.submitButton);
			this.Controls.Add(this.nsSeperator1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorHandlerForm";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ErrorHandlerForm_FormClosed);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Controls.NSSeperator nsSeperator1;
		private UI.Controls.NSIconicButton submitButton;
		private UI.Controls.NSTextBox errorInfo;
		private UI.Controls.NSLabel descLabel;
	}
}