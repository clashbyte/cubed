using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Data.Editor;
using Cubed.Data.Projects;
using Cubed.Forms.Dialogs;
using Cubed.Forms.Editors.Misc;
using Cubed.Forms.Resources;
using Cubed.UI.Controls;

namespace Cubed.Forms.Common {
	partial class MainForm {

		/// <summary>
		/// Opening other project
		/// </summary>
		private void openProjectToolStripMenuItem_Click(object sender, EventArgs e) {
			OpenFolderDialog fd = new OpenFolderDialog();
			fd.IsNewProject = false;
			if (fd.ShowDialog() == DialogResult.OK) {

				// Closing editor
				if (!CloseAllEditors()) {
					return;
				}

				// Closing editor
				ClosingAction = CloseAction.ProjectSwitch;
				CurrentProjectPath = fd.Folder;
				codeClosing = true;
				Close();

			}
		}

		/// <summary>
		/// Closing project
		/// </summary>
		private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e) {
			ClosingAction = CloseAction.ProjectClose;
			codeClosing = true;
			Close();
		}

		/// <summary>
		/// Saving all the data
		/// </summary>
		private void saveAllToolStripMenuItem_Click(object sender, EventArgs e) {
			foreach (TabPage tp in editorsControl.TabPages) {
				if (tp.Tag is EditorForm) {
					EditorForm ef = tp.Tag as EditorForm;
					ef.StartSaving();
					ef.Save();
				}
			}
		}

		/// <summary>
		/// Closing editor
		/// </summary>
		private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
			if (editorsControl.SelectedTab != null && editorsControl.TabPages.Count > 1) {
				TabPage tp = editorsControl.SelectedTab;
				EditorForm ef = tp.Tag as EditorForm;

				if (ef != null && !ef.Saved) {
					DialogResult dr = MessageDialog.Open(MessageBoxData.saveChangesTitle, MessageBoxData.saveChangesBody, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if (dr == DialogResult.Yes) {
						ef.StartSaving();
						ef.Save();
					} else if (dr == DialogResult.Cancel) {
						return;
					}
				}

				// Closing editor
				editorsControl.RemoveTab(tp);
			}
		}

		/// <summary>
		/// Closing editor
		/// </summary>
		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			ClosingAction = CloseAction.FullClose;
			codeClosing = true;
			Close();
		}

		/// <summary>
		/// Undo in editor
		/// </summary>
		private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
			if (editorsControl.SelectedTab.Tag is EditorForm) {
				(editorsControl.SelectedTab.Tag as EditorForm).Undo();
				UpdateEditingMenu();
			}
		}

		/// <summary>
		/// Redo in editor
		/// </summary>
		private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
			if (editorsControl.SelectedTab.Tag is EditorForm) {
				(editorsControl.SelectedTab.Tag as EditorForm).Redo();
				UpdateEditingMenu();
			}
		}

		/// <summary>
		/// Copy items in editor
		/// </summary>
		private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
			if (editorsControl.SelectedTab.Tag is EditorForm) {
				(editorsControl.SelectedTab.Tag as EditorForm).Copy(false);
				UpdateEditingMenu();
			}
		}

		/// <summary>
		/// Cut items in editor
		/// </summary>
		private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
			if (editorsControl.SelectedTab.Tag is EditorForm) {
				(editorsControl.SelectedTab.Tag as EditorForm).Copy(true);
				UpdateEditingMenu();
			}
		}

		/// <summary>
		/// Paste items in editor
		/// </summary>
		private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
			if (editorsControl.SelectedTab.Tag is EditorForm) {
				(editorsControl.SelectedTab.Tag as EditorForm).Paste();
				UpdateEditingMenu();
			}
		}

		/// <summary>
		/// Select all items in editor
		/// </summary>
		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
			if (editorsControl.SelectedTab.Tag is EditorForm) {
				(editorsControl.SelectedTab.Tag as EditorForm).SelectAll();
				UpdateEditingMenu();
			}
		}

		/// <summary>
		/// Open editor preferences
		/// </summary>
		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e) {
			
		}

		/// <summary>
		/// Run game test
		/// </summary>
		private void buildToolStripMenuItem_Click(object sender, EventArgs e) {
			saveAllToolStripMenuItem_Click(sender, e);
			Enabled = false;
			Process.Start(new ProcessStartInfo() {
				FileName = "Game.exe",
				Arguments = "-debug -unpacked -folder \""+Project.Root.FullPath+"\" -windowed"
			}).WaitForExit();
			Enabled = true;
			Activate();
		}

		/// <summary>
		/// Build game
		/// </summary>
		private void buildEXEToolStripMenuItem_Click(object sender, EventArgs e) {

		}

		/// <summary>
		/// Project properties
		/// </summary>
		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e) {

			// Searching for opened tab page
			foreach (TabPage ftp in editorsControl.TabPages) {
				if (ftp.Tag is GamePrefsEditor) {
					editorsControl.SelectedTab = ftp;
					return;
				}
			}

			// Creating prefs page
			GamePrefsEditor ppage = new GamePrefsEditor();
			TabPage tp = new TabPage();
			ppage.TopLevel = false;
			ppage.Location = System.Drawing.Point.Empty;
			ppage.Visible = true;
			ppage.BringToFront();
			tp.Controls.Add(ppage);
			tp.Tag = ppage;
			ppage.Dock = DockStyle.Fill;
			editorsControl.AddTab(tp, true);
		}

		/// <summary>
		/// Showing starting page
		/// </summary>
		private void startingPageToolStripMenuItem_Click(object sender, EventArgs e) {
			
			// Searching for opened tab page
			foreach (TabPage ftp in editorsControl.TabPages) {
				if (ftp.Tag is HomePage) {
					editorsControl.SelectedTab = ftp;
					return;
				}
			}

			// Creating start page
			HomePage hpage = new HomePage();
			TabPage tp = new TabPage();
			hpage.TopLevel = false;
			hpage.Location = System.Drawing.Point.Empty;
			hpage.Visible = true;
			hpage.BringToFront();
			tp.Controls.Add(hpage);
			tp.Tag = hpage;
			hpage.Dock = DockStyle.Fill;
			editorsControl.AddTab(tp, true);
		}



		/// <summary>
		/// Opening dropdown for creation
		/// </summary>
		private void createToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
			createToolStripMenuItem.DropDown = projectCreateMenu;
		}

		/// <summary>
		/// Creating new project entry
		/// </summary>
		private void contextMenuCreateItem_Click(object sender, EventArgs e) {
			ToolStripMenuItem m = sender as ToolStripMenuItem;

			// Creating restricted list
			List<string> restr = new List<string>();
			string ext = "";
			if (m.Tag != null) {
				FileTypeManager.FileType ftype = m.Tag as FileTypeManager.FileType;
				ext = ftype.Extensions[0];
				foreach (Project.Entry en in currentFolder.Entries) {
					string nm = en.Name.ToLower();
					string next = System.IO.Path.GetExtension(nm).ToLower();
					foreach (string fext in ftype.Extensions) {
						if (next == fext) {
							restr.Add(System.IO.Path.GetFileNameWithoutExtension(nm));
							break;
						}
					}
				}
			} else {
				foreach (Project.Folder fld in currentFolder.Folders) {
					restr.Add(fld.Name.ToLower());
				}
			}

			// Showing dialog
			TextInputDialog dlg = new TextInputDialog();
			dlg.Text = m.Tag != null ? MessageBoxData.newItemTitle : MessageBoxData.newFolderTitle;
			dlg.Description = m.Tag != null ? MessageBoxData.newItemBody : MessageBoxData.newFolderBody;
			dlg.Validator = name => {
				name = name.Trim().ToLower();
				if (!restr.Contains(name) && name.Length > 0) {
					return true;
				}
				return false;
			};
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				if (m.Tag != null) {
					System.IO.File.Create(System.IO.Path.Combine(currentFolder.FullPath, dlg.Value + ext)).Close();
				} else {
					System.IO.Directory.CreateDirectory(System.IO.Path.Combine(currentFolder.FullPath, dlg.Value));
				}
			}
		}

		/// <summary>
		/// Renaming entry
		/// </summary>
		private void projectRenameMenuItem_Click(object sender, EventArgs e) {

			// Creating restricted list
			List<string> restr = new List<string>();
			string ext = "";
			Project.EntryBase file = projectControl.SelectedEntry.Tag as Project.EntryBase;

			if (file is Project.Entry) {
				ext = System.IO.Path.GetExtension((file as Project.Entry).Name);
				foreach (Project.Entry en in currentFolder.Entries) {
					string nm = en.Name.ToLower();
					string next = System.IO.Path.GetExtension(nm).ToLower();
					if (next == ext && file != en) {
						restr.Add(System.IO.Path.GetFileNameWithoutExtension(nm));
						break;
					}
				}
			} else if (file is Project.Folder) {
				foreach (Project.Folder fld in currentFolder.Folders) {
					if (fld != file) {
						restr.Add(fld.Name.ToLower());
					}
				}
			}

			// Showing dialog
			TextInputDialog dlg = new TextInputDialog();
			dlg.Text = MessageBoxData.renameTitle;
			dlg.Description = MessageBoxData.renameBody;
			dlg.Value = file.Name;
			if (file is Project.Entry) {
				dlg.Value = (file as Project.Entry).NameWithoutExt;
			}
			dlg.Validator = name => {
				name = name.Trim().ToLower();
				if (!restr.Contains(name) && name.Length > 0) {
					return true;
				}
				return false;
			};
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				if (file is Project.Entry) {
					System.IO.File.Move(file.FullPath, System.IO.Path.Combine(currentFolder.FullPath, dlg.Value + ext));
				} else {
					System.IO.Directory.Move(file.FullPath, System.IO.Path.Combine(currentFolder.FullPath, dlg.Value));
				}
			}
		}

		/// <summary>
		/// Deleting entry
		/// </summary>
		private void projectDeleteMenuItem_Click(object sender, EventArgs e) {
			NSDirectoryInspector.Entry entry = projectControl.SelectedEntry;
			if (entry != null) {
				if (entry.Tag is Project.EntryBase) {
					Project.EntryBase eb = entry.Tag as Project.EntryBase;
					if (MessageDialog.Open(MessageBoxData.removeTitle, MessageBoxData.removeBody.Replace("%FILE%", eb.Name), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes) {
						if (eb is Project.Entry) {
							System.IO.File.Delete((eb as Project.Entry).FullPath);
						} else {
							System.IO.Directory.Delete((eb as Project.Folder).FullPath, true);
						}
					}
				}
			}
		}

		/// <summary>
		/// Open selected folder in Explorer
		/// </summary>
		private void openInExplorerToolStripMenuItem_Click(object sender, EventArgs e) {
			if (currentFolder != null) {
				ProcessStartInfo psi = new ProcessStartInfo(currentFolder.FullPath);
				psi.UseShellExecute = true;
				Process.Start(psi);
			}
		}

		/// <summary>
		/// Updating edit menu state
		/// </summary>
		public static void UpdateEditingMenu() {
			bool undo = false, redo = false, copy = false, paste = false, select = false;
			if (Current != null) {
				if (Current.editorsControl.SelectedTab != null) {
					if (Current.editorsControl.SelectedTab.Tag is EditorForm) {
						EditorForm ef = Current.editorsControl.SelectedTab.Tag as EditorForm;
						undo = ef.CanUndo;
						redo = ef.CanRedo;
						copy = ef.CanCopyOrCut;
						paste = ef.CanPaste;
						select = ef.CanSelectAll;
					}
				}
				Current.undoToolStripMenuItem.Enabled = undo;
				Current.redoToolStripMenuItem.Enabled = redo;
				Current.copyToolStripMenuItem.Enabled = Current.cutToolStripMenuItem.Enabled = copy;
				Current.pasteToolStripMenuItem.Enabled = paste;
				Current.selectAllToolStripMenuItem.Enabled = select;
			}
		}
	}
}
