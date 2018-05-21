using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.AccessControl;
using System.Windows.Forms;
using Cubed.Forms.Resources;
using Cubed.UI.Controls;
using Cubed.UI.Graphics;

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
			Navigate("C:\\");
		}

		/// <summary>
		/// Going to specific path
		/// </summary>
		/// <param name="path">Path to navigate</param>
		void Navigate(string path) {

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
									break;
								case DriveType.Removable:
									en.MainIcon = removableDriveIcon;
									break;
								case DriveType.Network:
									en.MainIcon = netDriveIcon;
									break;
								default:
									continue;
							}
							en.Tag = d.RootDirectory;
							disks.Add(en);
						}
					}
					items = disks.ToArray();
				} else {
					path = "/";
				}
			}

			// Enumerating dirs
			if (items == null) {
				List<NSDirectoryInspector.Entry> folders = new List<NSDirectoryInspector.Entry>();
				
				string[] dirs = Directory.GetDirectories(path);
				foreach (string dirName in dirs) {

					// Checking permissions
					try {
						DirectorySecurity ds = Directory.GetAccessControl(dirName);
					} catch (UnauthorizedAccessException) {
						continue;
					}

					DirectoryInfo dir = new DirectoryInfo(dirName);
					if (dir.Attributes.HasFlag(FileAttributes.Hidden) || dir.Attributes.HasFlag(FileAttributes.System)) {
						continue;
					}


					NSDirectoryInspector.Entry en = new NSDirectoryInspector.Entry();
					en.Name = dir.Name;
					en.MainIcon = folderIcon;
					en.Tag = dir;
					folders.Add(en);
				}

				items = folders.ToArray();
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
