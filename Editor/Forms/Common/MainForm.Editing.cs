using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Data.Editor;
using Cubed.Data.Projects;
using Cubed.Forms.Dialogs;

namespace Cubed.Forms.Common {
	partial class MainForm {

		/// <summary>
		/// Previous tab page
		/// </summary>
		TabPage prevTabPage;

		/// <summary>
		/// Trying to open editor
		/// </summary>
		/// <param name="entry">Entry to edit</param>
		void Project_OpenEditor(Project.Entry entry) {
			Type t = FileTypeManager.GetEditor(entry);
			if (t != null) {

				// Opening editor
				EditorForm editor = Activator.CreateInstance(t) as EditorForm;
				if (editor != null) {

					// Creating editor
					TabPage tp = new TabPage();
					editor.TopLevel = false;
					editor.Location = System.Drawing.Point.Empty;
					editor.Visible = true;
					editor.BringToFront();
					editor.SetFile(entry);
					tp.Controls.Add(editor);
					tp.Tag = editor;
					editor.Dock = DockStyle.Fill;
					editorsControl.AddTab(tp, true);

				}
			}
		}
		
		/// <summary>
		/// Selected tab changed
		/// </summary>
		void editorsControl_SelectedIndexChanged(object sender, EventArgs e) {
			if (prevTabPage != null) {
				if (prevTabPage.Tag is EditorForm) {
					(prevTabPage.Tag as EditorForm).Pause(); 
				}
			}

			object inspect = null;
			prevTabPage = editorsControl.SelectedTab;
			if (prevTabPage != null) {
				if (prevTabPage.Tag is EditorForm) {
					(prevTabPage.Tag as EditorForm).Resume();
					inspect = (prevTabPage.Tag as EditorForm).InspectingObject;
				}
			}
			inspector.Target = inspect;
			UpdateEditingMenu();
		}

		/// <summary>
		/// Closing tab page
		/// </summary>
		private void editorsControl_TabClose(object sender, UI.Controls.NSProjectControl.TabCloseEventArgs e) {
			if (e.Page.Tag is EditorForm) {
				e.Cancel = !CloseEditor(e.Page.Tag as EditorForm);
			}
		}

		/// <summary>
		/// Field changed
		/// </summary>
		private void inspector_FieldChanged(object sender, EventArgs e) {
			if (editorsControl.SelectedTab.Tag is EditorForm) {
				(editorsControl.SelectedTab.Tag as EditorForm).InspectedObjectModified();
			}
		}

	}
}
