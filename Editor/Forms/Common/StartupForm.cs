using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Cubed.Data.Defines;
using Cubed.Data.Files;
using Cubed.Forms.Dialogs;
using Cubed.Forms.Resources;

namespace Cubed.Forms.Common {

	/// <summary>
	/// Project selection form
	/// </summary>
	public partial class StartupForm : Form {

		/// <summary>
		/// Close after constructor
		/// </summary>
		public bool ExitWithoutShow {
			get;
			private set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public StartupForm() {
			InitializeComponent();
			loadingLabel.Dock = DockStyle.Fill;

			// Opening previous project
			string project = Properties.Settings.Default.LastProject;
			if (project != "" && File.Exists(System.IO.Path.Combine(project, ".cubed"))) {
				MainForm.CloseAction act = ShowMainForm(project, false);
				if (act == MainForm.CloseAction.FullClose) {
					ExitWithoutShow = true;
					return;
				}
			}
		}

		/// <summary>
		/// Form startup
		/// </summary>
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			Text = "Cubed v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			Activate();
		}

		/// <summary>
		/// New project button event
		/// </summary>
		private void newProjectButton_Click(object sender, EventArgs e) {

			// Showing dialog
			OpenFolderDialog fd = new OpenFolderDialog();
			fd.IsNewProject = true;
			if (fd.ShowDialog() == DialogResult.OK) {
				
				// Creating new project
				Chunk projData = ProjectInfo.CreateDefault(fd.Folder).Save();
				string error = "";
				if (!ChunkedFile.Write(System.IO.Path.Combine(fd.Folder, ".cubed"), projData, out error)) {
					MessageDialog.Open("Unable to create project", error, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				
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

			// Showing dialog
			OpenFolderDialog fd = new OpenFolderDialog();
			fd.IsNewProject = false;
			if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

				// Check for broken project
				bool newProj = false;
				ProjectBasicInfo info = ProjectBasicInfo.Read(fd.Folder, false);
				if (info == null) {
					if (MessageDialog.Open(MessageBoxData.brokenProjectTitle, MessageBoxData.brokenProjectBody, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No) {
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
