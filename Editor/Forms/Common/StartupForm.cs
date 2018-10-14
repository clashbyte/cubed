using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Cubed.Data.Defines;
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
			loadingLabel.Dock = DockStyle.Fill;




			#if DEBUG
			// Showing default project in debug mode
			Hide();
			if (Directory.Exists("./TestProject")) {
				ShowMainForm(Path.Combine(Directory.GetCurrentDirectory(), "./TestProject"));
			} else {
				ShowMainForm(Path.Combine(Directory.GetCurrentDirectory(), "./../../../Project"));
			}
			#endif
		}

		/// <summary>
		/// Form startup
		/// </summary>
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			Text = "Cubed v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		/// <summary>
		/// New project button event
		/// </summary>
		private void newProjectButton_Click(object sender, EventArgs e) {

			OpenFolderDialog fd = new OpenFolderDialog();
			fd.IsNewProject = true;
			if (fd.ShowDialog() == DialogResult.OK) {
				
				// Creating new project
				
				
				// Showing main form
				Hide();
				MainForm.CloseAction act = ShowMainForm(fd.Folder, true);
				if (act == MainForm.CloseAction.FullClose) {
					Close();
					return;
				}
				Show();
				Activate();
			}

		}

		/// <summary>
		/// Open project button event
		/// </summary>
		private void openProjectButton_Click(object sender, EventArgs e) {

			
			OpenFolderDialog fd = new OpenFolderDialog();
			fd.IsNewProject = false;
			if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

				// Check for broken project
				bool newProj = false;
				ProjectBasicInfo info = ProjectBasicInfo.Read(fd.Folder, false);
				if (info == null) {
					if (MessageDialog.Open("Warning!", "You are trying to open broken project! Are you sure want to proceed?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No) {
						return;
					}
					newProj = true;
				}

				// Showing main form
				Hide();
				MainForm.CloseAction act = ShowMainForm(fd.Folder, newProj);
				if (act == MainForm.CloseAction.FullClose) {
					Close();
					return;
				}
				Show();
				Activate();
			}
			
		}

		/// <summary>
		/// Opening form
		/// </summary>
		/// <param name="path">Path to open</param>
		/// <param name="isNew">Is new project</param>
		MainForm.CloseAction ShowMainForm(string path, bool isNew = false) {
			while (true) {
				MainForm mf = new MainForm();
				mf.StartingAction = isNew ? MainForm.StartAction.OpenNew : MainForm.StartAction.Open;
				mf.CurrentProjectPath = path;
				mf.Open();
				switch (mf.ClosingAction) {
						
					case MainForm.CloseAction.ProjectSwitch:
						path = mf.CurrentProjectPath;
						isNew = false;
						break;

					case MainForm.CloseAction.ProjectClose:
					case MainForm.CloseAction.FullClose:
						return mf.ClosingAction;
				}
			}
		}
	}
}
