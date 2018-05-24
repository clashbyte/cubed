using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.AccessControl;
using System.Windows.Forms;
using Cubed.Forms.Resources;
using Cubed.UI.Controls;
using Cubed.UI.Graphics;
using System.Media;
using Cubed.Data.Defines;
using Cubed.Data.Projects;

namespace Cubed.Forms.Dialogs {

	/// <summary>
	/// Dialog for opening OS directories
	/// </summary>
	public partial class OpenFolderDialog : Form {

		/// <summary>
		/// Project opening mode
		/// </summary>
		[DefaultValue(false)]
		public bool IsNewProject {
			get;
			set;
		}

		/// <summary>
		/// Resulting folder
		/// </summary>
		public string Folder {
			get;
			private set;
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		public OpenFolderDialog() {
			InitializeComponent();
			switch (Environment.OSVersion.Platform) {
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
				case PlatformID.WinCE:
				case PlatformID.Xbox:
					isWindows = true;
					break;

				default:
					isWindows = false;
					break;
			}
			if (driveIcon == null || netDriveIcon == null || removableDriveIcon == null || folderIcon == null) {
				driveIcon = new UIIcon(DirectoryInspectorIcons.Drive);
				netDriveIcon = new UIIcon(DirectoryInspectorIcons.NetworkDrive);
				removableDriveIcon = new UIIcon(DirectoryInspectorIcons.RemovableDrive);
				folderIcon = new UIIcon(DirectoryInspectorIcons.Folder);
			}
			projects = new Dictionary<NSDirectoryInspector.Entry, ProjectBasicInfo>();
			pathBox.BaseInput.KeyDown += pathBox_KeyDown;
			pathBox.BaseInput.Location = new System.Drawing.Point(pathBox.BaseInput.Location.X, pathBox.BaseInput.Location.Y + 3);
		}

		/// <summary>
		/// Is selector running on Windows
		/// </summary>
		bool isWindows;
		
		/// <summary>
		/// Current path
		/// </summary>
		string currentPath;

		/// <summary>
		/// Existing file names
		/// </summary>
		string[] existingNames;

		/// <summary>
		/// Projects
		/// </summary>
		Dictionary<NSDirectoryInspector.Entry, ProjectBasicInfo> projects;

		/// <summary>
		/// Icons
		/// </summary>
		static UIIcon driveIcon, netDriveIcon, removableDriveIcon, folderIcon;

		/// <summary>
		/// Navigate to desktop
		/// </summary>
		private void goHomeButton_Click(object sender, EventArgs e) {
			Navigate(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
		}

		/// <summary>
		/// Navigate to PC
		/// </summary>
		private void goPCButton_Click(object sender, EventArgs e) {
			Navigate("");
		}

		/// <summary>
		/// Navigate to documents
		/// </summary>
		private void goDocsButton_Click(object sender, EventArgs e) {
			Navigate(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
		}

		/// <summary>
		/// Form focused - show same folder
		/// </summary>
		private void OpenFolderDialog_Activated(object sender, EventArgs e) {
			if (currentPath != null) {
				Navigate(currentPath);
			}
		}

		/// <summary>
		/// Going one directory up
		/// </summary>
		private void goUpButton_Click(object sender, EventArgs e) {
			DirectoryInfo parentDir = Directory.GetParent(currentPath);
			if (parentDir != null) {
				Navigate(parentDir.FullName);
			} else {
				Navigate("");
			}
		}

		/// <summary>
		/// Directory selector changes
		/// </summary>
		private void directoryBrowser_SelectionChanged(object sender) {

			// Switching button
			bool allow = false;
			NSDirectoryInspector.Entry se = directoryBrowser.SelectedEntry;
			if (se != null) {
				ProjectBasicInfo info = null;
				if (projects.ContainsKey(se)) {
					info = projects[se];
				}
				allow = (info == null) == IsNewProject;
			}
			selectButton.Enabled = allow;

			// Applying folder inspector
			if (se != null) {
				folderInfo.File = se;
			} else {
				folderInfo.File = null;
			}
		}

		/// <summary>
		/// Cancecling opening
		/// </summary>
		private void cancelButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Folder = "";
			Close();
		}

		/// <summary>
		/// Open button hit
		/// </summary>
		private void selectButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		/// <summary>
		/// Making new folder
		/// </summary>
		private void newFolderButton_Click(object sender, EventArgs e) {
			TextInputDialog dlg = new TextInputDialog();
			dlg.Text = "Making new folder";
			dlg.Description = "Specify folder name:";
			dlg.Validator = (val) => {
				if (val.Length == 0) {
					return false;
				}
				return !existingNames.Contains(val.ToLower());
			};
			if (dlg.ShowDialog() == DialogResult.OK) {
				try {
					Directory.CreateDirectory(Path.Combine(currentPath, dlg.Value));
					Navigate(currentPath);
				} catch (Exception ex) {
					MessageBox.Show(ex.ToString());
				}
			}
		}

		/// <summary>
		/// Submit key in path
		/// </summary>
		private void pathBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				e.SuppressKeyPress = true;

				string path = Environment.ExpandEnvironmentVariables(pathBox.Text).Trim();
				if (path != "") {
					if (!Directory.Exists(path)) {
						SystemSounds.Beep.Play();
						path = "";
					}
				}
				Navigate(path);
			}
		}

		/// <summary>
		/// Double click
		/// </summary>
		void directoryBrowser_DoubleClick(object sender, EventArgs e) {
			if (directoryBrowser.SelectedEntry != null) {
				object obj = directoryBrowser.SelectedEntry.Tag;
				if (obj is DirectoryInfo) {
					Navigate((obj as DirectoryInfo).FullName);
				}
			}
		}

		/// <summary>
		/// On form show
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			newFolderButton.Visible = IsNewProject;
			Navigate("");
		}

		/// <summary>
		/// Going to specific path
		/// </summary>
		/// <param name="path">Path to navigate</param>
		void Navigate(string path) {

			// Checking path
			if (path != "") {
				while (true) {
					if (Directory.Exists(path)) {
						break;
					}
					DirectoryInfo parent = (new DirectoryInfo(path)).Parent;
					if (parent == null) {
						path = "";
						break;
					} else {
						path = parent.FullName;
					}
				}
			}
			
			// Check for empty path
			NSDirectoryInspector.Entry[] items = null;
			if (path == "") {
				if (isWindows) {

					// Reading disks
					DriveInfo[] driveInfos = DriveInfo.GetDrives();
					List<NSDirectoryInspector.Entry> disks = new List<NSDirectoryInspector.Entry>();
					foreach (DriveInfo d in driveInfos) {
						if (d.IsReady) {
							NSDirectoryInspector.Entry en = new NSDirectoryInspector.Entry();
							if (d.VolumeLabel != "") {
								en.Name = d.VolumeLabel + " (" + d.Name + ")";
							} else {
								en.Name = d.Name;
							}
							switch (d.DriveType) {
								case DriveType.Fixed:
								case DriveType.Ram:
									en.MainIcon = driveIcon;
									en.SubName = "Hard drive";
									break;
								case DriveType.Removable:
									en.MainIcon = removableDriveIcon;
									en.SubName = "Removable";
									break;
								case DriveType.Network:
									en.MainIcon = netDriveIcon;
									en.SubName = "Network";
									break;
								default:
									continue;
							}
							en.Tag = d.RootDirectory;
							disks.Add(en);
						}
					}
					items = disks.ToArray();
					newFolderButton.Enabled = false;
				} else {
					path = "/";
					newFolderButton.Enabled = true;
				}
				goUpButton.Enabled = false;
			} else {
				goUpButton.Enabled = true;
				newFolderButton.Enabled = true;
			}
			Folder = path;
			projects.Clear();

			// Enumerating dirs
			if (items == null) {
				List<NSDirectoryInspector.Entry> folders = new List<NSDirectoryInspector.Entry>();
				List<string> existing = new List<string>();
				

				DirectoryInfo[] dirs = (new DirectoryInfo(path)).GetDirectories();
				foreach (DirectoryInfo dir in dirs) {

					// Adding to existing
					existing.Add(dir.Name.ToLower());

					// Checking permissions
					try {
						DirectorySecurity ds = Directory.GetAccessControl(dir.FullName);
					} catch (UnauthorizedAccessException) {
						continue;
					}
					if (dir.Attributes.HasFlag(FileAttributes.Hidden) || dir.Attributes.HasFlag(FileAttributes.System)) {
						continue;
					}

					// Making entry
					NSDirectoryInspector.Entry en = new NSDirectoryInspector.Entry();
					en.Name = dir.Name;
					en.MainIcon = folderIcon;
					en.SubName = "Folder";
					en.Tag = dir;

					// Checking for project
					ProjectBasicInfo info = Project.GetInfoByFolder(dir.FullName);
					if (info != null) {
						en.Name = info.Name;
						en.SubName = info.Author;
						en.Description = dir.Name;
						en.MainIcon = new UIIcon(info.Splash);
						projects.Add(en, info);
					}

					// Adding
					folders.Add(en);
				}

				// Files
				FileInfo[] files = (new DirectoryInfo(path)).GetFiles();
				foreach (FileInfo file in files) {
					existing.Add(file.Name.ToLower());
				}
				existingNames = existing.ToArray();
				items = folders.ToArray();
			} else {
				existingNames = new string[0];
			}

			// Populating control
			directoryBrowser.SuspendLayout();
			directoryBrowser.SelectedEntry = null;
			directoryBrowser.Entries.Clear();
			foreach (NSDirectoryInspector.Entry item in items) {
				directoryBrowser.Entries.Add(item);
			}
			directoryBrowser.ResumeLayout();
			currentPath = path;
			pathBox.Text = path;
			directoryBrowser.Focus();
		}

	}
}
