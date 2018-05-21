using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Forms.Dialogs;

namespace Cubed.Forms.Common {

	/// <summary>
	/// Project selection form
	/// </summary>
	public partial class StartupForm : Form {

		/// <summary>
		/// Constructor
		/// </summary>
		public StartupForm() {
			InitializeComponent();
		}

		/// <summary>
		/// New project button event
		/// </summary>
		private void newProjectButton_Click(object sender, EventArgs e) {

			OpenFolderDialog fd = new OpenFolderDialog();
			fd.IsNewProject = true;
			fd.ShowDialog();

		}

		/// <summary>
		/// Open project button event
		/// </summary>
		private void openProjectButton_Click(object sender, EventArgs e) {

			OpenFolderDialog fd = new OpenFolderDialog();
			fd.IsNewProject = false;
			fd.ShowDialog();

		}
	}
}
