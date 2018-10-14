using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Core;
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
		/// Selected inspector object
		/// </summary>
		public static object SelectedTarget {
			get {
				if (Current != null) {
					return Current.inspector.Target;
				}
				return null;
			}
			set {
				if (Current != null) {
					Current.inspector.Target = value;
				}
			}
		}

		/// <summary>
		/// Currently selected entry
		/// </summary>
		public static Project.EntryBase SelectedEntry {
			get {
				if (Current != null) {
					if (Current.projectControl.SelectedEntry != null) {
						return Current.projectControl.SelectedEntry.Tag as Project.EntryBase;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Current engine
		/// </summary>
		public static Engine CurrentEngine {
			get;
			set;
		}

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
		public void PopulateProjectView(bool skipPos = false) {
			
			// Open root folder
			if (currentFolder == null) {
				currentFolder = Project.Root;
			}

			// Suspending browser
			projectControl.SuspendLayout();
			projectControl.Entries.Clear();
			if (!skipPos) {
				projectControl.Offset = 0;
			}

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

					// Picking file
					foreach (TabPage page in editorsControl.TabPages) {
						if (page.Tag is EditorForm) {
							if ((page.Tag as EditorForm).File == (obj as Project.Entry)) {
								editorsControl.SelectedTab = page;
								return;
							}
						}
					}

					// Detecting type
					Type t = FileTypeManager.GetEditor(obj as Project.Entry);
					if (t != null) {
						Project_OpenEditor(obj as Project.Entry);
					} else {

						// Opening file
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
		}

		/// <summary>
		/// Picking file for preview
		/// </summary>
		private void projectControl_SelectionChanged(object sender) {
			//inspector.Target = projectControl.SelectedEntry;
			projectFileInfo.File = projectControl.SelectedEntry;
		}

		/// <summary>
		/// Clicking mouse
		/// </summary>
		private void projectControl_MouseClick(object sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Right) {
				for (int i = 2; i < projectPopupMenu.Items.Count; i++) {
					projectPopupMenu.Items[i].Visible = projectControl.SelectedEntry != null;
				}
				
				// Showing popup
				System.Drawing.Point mouse = projectControl.PointToScreen(e.Location);
				projectPopupMenu.Show(mouse);
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

		/// <summary>
		/// Entries list changed - check for rebuild
		/// </summary>
		void Project_EntriesChangedEvent(object sender, Project.MultipleEntryEventArgs e) {
			bool changed = false;
			bool pos = false;
			foreach (Project.EntryEventArgs evarg in e.Events) {
				if (evarg.Entry.Parent == currentFolder) {
					if (evarg.Type == Project.EntryEvent.Modified) {
						foreach (NSDirectoryInspector.Entry en in projectControl.Entries) {
							if (en.Tag == (evarg.Entry as object)) {
								projectControl.PatchPreview(en);
								break;
							}
						}
					}
					if (evarg.Type != Project.EntryEvent.Modified) {
						changed = true;
					}
					if (evarg.Type == Project.EntryEvent.Deleted) {
						pos = true;
					}
				}
			}
			if (changed) {
				PopulateProjectView(pos);
			}
		}

		/// <summary>
		/// Focusing editor
		/// </summary>
		/// <param name="editor">Editor to focus</param>
		public static void FocusEditor(EditorForm editor) {
			foreach (TabPage tp in Current.editorsControl.TabPages) {
				if (tp.Tag == editor) {
					Current.editorsControl.SelectTab(Current.editorsControl.TabPages.IndexOf(tp));
					break;
				}
			}
		}

		/// <summary>
		/// Closing editor
		/// </summary>
		/// <param name="editor">Editor to close</param>
		public static bool CloseEditor(EditorForm editor, bool force = false) {
			
			// Checking for close
			TabPage tp = null;
			foreach (TabPage t in Current.editorsControl.TabPages) {
				if (t.Tag == editor) {
					tp = t;
					break;
				}
			}
			if (tp == null) {
				return true;
			}

			// Prompting for close
			if (!force && !editor.Saved) {
				DialogResult dr = MessageDialog.Open("Unsaved changes", "File has unsaved changes. Do you want to save them?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (dr == DialogResult.Yes) {
					editor.StartSaving();
					editor.Save();
				} else if (dr == DialogResult.Cancel) {
					return false;
				}
			}

			// Closing editor
			Current.editorsControl.RemoveTab(tp);
			editor.Close();
			return true;
		}

		/// <summary>
		/// Close all opened editors
		/// </summary>
		/// <returns>True if closing succeded</returns>
		public static bool CloseAllEditors() {

			// Filling list of editors
			List<EditorForm> editors = new List<EditorForm>();
			foreach (TabPage page in Current.editorsControl.TabPages) {
				if (page.Tag is EditorForm) {
					editors.Add(page.Tag as EditorForm);
				}
			}

			// Checking for close
			foreach (EditorForm ef in editors) {
				if (!CloseEditor(ef)) {
					return false;
				}
			}
			return true;
		}
		
	}
}
