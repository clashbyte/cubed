using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubed.Forms.Dialogs {

	/// <summary>
	/// Form for exception handling
	/// </summary>
	public partial class ErrorHandlerForm : Form {

		/// <summary>
		/// Form constructor
		/// </summary>
		/// <param name="ex">Error info</param>
		public ErrorHandlerForm(Exception ex) {
			InitializeComponent();
			errorInfo.Text = ex.ToString();
		}

		/// <summary>
		/// Terminating application
		/// </summary>
		private void ErrorHandlerForm_FormClosed(object sender, FormClosedEventArgs e) {
			Application.Exit();
		}

		/// <summary>
		/// Closing app
		/// </summary>
		private void submitButton_Click(object sender, EventArgs e) {
			Close();
		}
	}
}
