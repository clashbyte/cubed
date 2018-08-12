using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cubed.Data.Editor;
using Cubed.Data.Editor.Previews;
using Cubed.Data.Projects;
using Cubed.Forms.Dialogs;
using Cubed.Forms.Resources;
using Cubed.UI.Controls;
using Cubed.UI.Graphics;

namespace Cubed.Forms.Common {
	
	partial class MainForm {

		/// <summary>
		/// Current opened folder
		/// </summary>
		Project.Folder currentFolder;

		/// <summary>
		/// Current inspected entry
		/// </summary>
		Project.EntryBase currentEntry;
		
		/// <summary>
		/// Fill project view with files
		/// </summary>
		public void PopulateProjectView() {
			
			// Open root folder
			if (currentFolder == null) {
				currentFolder = Project.Root;
			}

			// Suspending browser
			projectControl.SuspendLayout();
			projectControl.Entries.Clear();

			// Creating folders
			UIIcon folderIcon = new UIIcon(DirectoryInspectorIcons.Folder);
			foreach (Project.Folder folder in currentFolder.Folders) {
				NSDirectoryInspector.Entry en = new NSDirectoryInspector.Entry();
				en.Tag = (object)folder;
				en.IsDraggable = false;
				en.Name = folder.Name;
				en.MainIcon = folderIcon;
				projectControl.Entries.Add(en);
			}

			// And entries
			UIIcon fileIcon = new UIIcon(DirectoryInspectorIcons.RemovableDrive);
			foreach (Project.Entry entry in currentFolder.Entries) {
				NSDirectoryInspector.Entry en = new NSDirectoryInspector.Entry();
				en.Tag = (object)entry;
				en.IsDraggable = true;
				en.Name = entry.NameWithoutExt;
				en.SubName = FileTypeManager.GetName(entry);
				en.MainIcon = entry.Icon.Icon;
				if (entry.Icon.ShowSubIcon) {
					en.BulletIcon = entry.Icon.SubIcon;
				}
				projectControl.Entries.Add(en);
			}

			// Resuming
			projectControl.ResumeLayout();

			// Go up button
			projectPath.Text = currentFolder.Path;
			projectUpButton.Enabled = currentFolder.Parent != null;
		}

		/// <summary>
		/// Handling ready images
		/// </summary>
		void Preview_PreviewReady(object sender, Preview.PreviewEventArgs e) {
			foreach (NSDirectoryInspector.Entry en in projectControl.Entries) {
				if (en.Tag is Project.Entry) {
					Project.Entry pe = en.Tag as Project.Entry;
					if (pe.Icon == e.Preview) {
						en.MainIcon = e.Preview.Icon;
						if (e.Preview.ShowSubIcon) {
							en.BulletIcon = e.Preview.SubIcon;
						} else {
							en.BulletIcon = null;
						}
						projectControl.PatchPreview(en);
						projectControl.Invalidate();
						break;
					}
				}
			}
		}

		/// <summary>
		/// Double-clicked item in project control
		/// </summary>
		private void projectControl_DoubleClick(object sender, EventArgs e) {
			if (projectControl.SelectedEntry != null) {
				object obj = projectControl.SelectedEntry.Tag;
				if (obj is Project.Folder) {
					currentFolder = obj as Project.Folder;
					PopulateProjectView();
				} else if (obj is Project.Entry)  {

					try {
						ProcessStartInfo psi = new ProcessStartInfo((obj as Project.Entry).FullPath);
						psi.UseShellExecute = true;
						Process.Start(psi);
					} catch (Exception ex) {
						MessageDialog.Open("Unable to open", "Failed to start associated app!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
						throw;
					}
					

				}
			}
		}
		
		/// <summary>
		/// Go up
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void projectUpButton_Click(object sender, EventArgs e) {
			if (currentFolder.Parent != null) {
				currentFolder = currentFolder.Parent as Project.Folder;
				PopulateProjectView();
			}
		}
		
	}
}
