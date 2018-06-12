using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cubed.Forms.Common
{
	public partial class MainForm : Form
	{

		/// <summary>
		/// Constructor
		/// </summary>
		public MainForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Showing form - process startup events
		/// </summary>
		private void MainForm_Shown(object sender, EventArgs e) {

		}

		/// <summary>
		/// Closing form
		/// </summary>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {





				// User closes the form - close parent
				ClosingAction = CloseAction.FullClose;
			}
		}

		/// <summary>
		/// Form closed totally
		/// </summary>
		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			




		}
	}
}
