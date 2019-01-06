using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubed.Forms.Common {
	public partial class VolumeControlForm : Form {

		/// <summary>
		/// Constructor
		/// </summary>
		public VolumeControlForm() {
			InitializeComponent();
		}

		/// <summary>
		/// Drawing rectangle
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			e.Graphics.DrawRectangle(new Pen(Color.FromArgb(30, 30, 30)), 0, 0, Width - 1, Height - 1);
		}

		/// <summary>
		/// Showing form
		/// </summary>
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			Focus();
		}

		/// <summary>
		/// Leaving form
		/// </summary>
		protected override void OnLeave(EventArgs e) {
			base.OnLeave(e);
			Hide();
		}

		/// <summary>
		/// Changing visibility
		/// </summary>
		/// <param name="e"></param>
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if (Visible) {
				Focus();
			}
		}
	}
}
