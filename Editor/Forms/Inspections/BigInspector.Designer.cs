namespace Cubed.Forms.Inspections {
	partial class BigInspector {
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
			this.itemList = new Cubed.UI.Controls.NSTabControl();
			this.SuspendLayout();
			// 
			// itemList
			// 
			this.itemList.Alignment = System.Windows.Forms.TabAlignment.Left;
			this.itemList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.itemList.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.itemList.ItemSize = new System.Drawing.Size(28, 170);
			this.itemList.Location = new System.Drawing.Point(0, 0);
			this.itemList.Multiline = true;
			this.itemList.Name = "itemList";
			this.itemList.SelectedIndex = 0;
			this.itemList.Size = new System.Drawing.Size(407, 284);
			this.itemList.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.itemList.TabIndex = 0;
			// 
			// BigInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.Controls.Add(this.itemList);
			this.Name = "BigInspector";
			this.Size = new System.Drawing.Size(407, 284);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Controls.NSTabControl itemList;
	}
}
