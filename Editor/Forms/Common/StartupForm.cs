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
			//webBrowser.Dock = DockStyle.Fill;
			loadingLabel.Dock = DockStyle.Fill;
			//webBrowser.Navigate("https://google.com");


			//Hide();
			ShowMainForm(Path.Combine(Directory.GetCurrentDirectory(), "./../../../Project"));
			//
		}

		/// <summary>
		/// Form startup
		/// </summary>
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			Text = "Cubed v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

			Close();
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
			}
			
		}

		/// <summary>
		/// Opening 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="isNew"></param>
		MainForm.CloseAction ShowMainForm(string path, bool isNew = false) {
			MainForm mf = new MainForm();
			mf.StartingAction = isNew ? MainForm.StartAction.OpenNew : MainForm.StartAction.Open;
			mf.CurrentProjectPath = path;
			mf.Open();
			return mf.ClosingAction;
		}

		/// <summary>
		/// Showing web browser when page loads
		/// </summary>
		void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
			//webBrowser.Visible = true;
		}

		void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e) {
			//webBrowser.Visible = true;
		}
	}
}
