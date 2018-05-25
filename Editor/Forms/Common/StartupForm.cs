using System;
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
		/// Form startup
		/// </summary>
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);

		}

		/// <summary>
		/// New project button event
		/// </summary>
		private void newProjectButton_Click(object sender, EventArgs e) {

			OpenFolderDialog fd = new OpenFolderDialog();
			fd.IsNewProject = true;
			if (fd.ShowDialog() == DialogResult.OK) {
				Hide();

				MainForm mf = new MainForm();
				mf.ShowDialog();

				Show();
			}

		}

		/// <summary>
		/// Open project button event
		/// </summary>
		private void openProjectButton_Click(object sender, EventArgs e) {

			/*
			OpenFolderDialog fd = new OpenFolderDialog();
			fd.IsNewProject = false;
			fd.ShowDialog();
			*/
			MessageBox.Show("Енот вонючка!", "О нет!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
	}
}
