using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Data.Editor;
using Cubed.Data.Editor.Previews;
using Cubed.Data.Projects;
using Cubed.Forms.Resources;
using Cubed.UI.Controls;
using Cubed.UI.Graphics;

namespace Cubed.Forms.Dialogs {
	public partial class ProjectPickerDialog : Form {

		/// <summary>
		/// Parent dropper
		/// </summary>
		public NSFileDropControl Dropper {
			get;
			set;
		}

		/// <summary>
		/// Current folder
		/// </summary>
		Project.Folder currentDir;

		/// <summary>
		/// Previous file
		/// </summary>
		Project.Entry prevFile;

		/// <summary>
		/// Flag for setting file
		/// </summary>
		bool fileSet;

		/// <summary>
		/// Constructor
		/// </summary>
		public ProjectPickerDialog() {
			InitializeComponent();
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			e.Graphics.DrawRectangle(new Pen(Color.FromArgb(30, 30, 30)), 0, 0, Width - 1, Height - 1);
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			prevFile = Dropper.File;
			if (Dropper.File != null) {
				currentDir = Dropper.File.Parent as Project.Folder;
			} else {
				currentDir = Project.Root;
			}
			Populate(true);
			Preview.PreviewReady += Preview_PreviewReady;
			Focus();
		}

		/// <summary>
		/// Preview completed
		/// </summary>
		void Preview_PreviewReady(object sender, Preview.PreviewEventArgs e) {
			inspector.Invalidate();
		}

		/// <summary>
		/// Closing
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			Preview.PreviewReady -= Preview_PreviewReady;
		}

		/// <summary>
		/// Leaving form
		/// </summary>
		protected override void OnLeave(EventArgs e) {
			base.OnLeave(e);
			if (!fileSet) {
				Dropper.File = prevFile;
			}
			Close();
		}

		/// <summary>
		/// Canceling
		/// </summary>
		private void cancelButton_Click(object sender, EventArgs e) {
			Close();
		}

		/// <summary>
		/// Selecting entry
		/// </summary>
		private void submitButton_Click(object sender, EventArgs e) {
			if (inspector.SelectedEntry != null) {
				if (inspector.SelectedEntry.Tag is Project.Folder) {
					currentDir = inspector.SelectedEntry.Tag as Project.Folder;
					inspector.SelectedEntry = null;
					submitButton.Enabled = false;
					Populate();
				} else {
					DialogResult = System.Windows.Forms.DialogResult.OK;
					Dropper.File = inspector.SelectedEntry.Tag as Project.Entry;
					fileSet = true;
					Close();
				}
			}
		}

		/// <summary>
		/// Clearing value
		/// </summary>
		private void clearButton_Click(object sender, EventArgs e) {
			DialogResult = System.Windows.Forms.DialogResult.OK;
			if (Dropper != null) {
				Dropper.File = null;
			}
			fileSet = true;
			Close();
		}

		/// <summary>
		/// Going up
		/// </summary>
		private void nsUpButton_Click(object sender, EventArgs e) {
			if (currentDir.Parent != null) {
				currentDir = currentDir.Parent as Project.Folder;
				Populate();
			}
		}

		/// <summary>
		/// Picking entry
		/// </summary>
		private void inspector_MouseDown(object sender, MouseEventArgs e) {
			submitButton.Enabled = inspector.SelectedEntry != null;
			if (inspector.SelectedEntry != null) {
				if (Dropper != null && !(inspector.SelectedEntry.Tag is Project.Folder)) {
					Dropper.File = (Project.Entry)inspector.SelectedEntry.Tag;
				}
			}
		}

		/// <summary>
		/// Populating list
		/// </summary>
		private void Populate(bool select = false) {

			// Очистка
			inspector.Entries.Clear();
			inspector.Offset = 0;

			// Путь до папки
			fileLabel.Text = currentDir.Name;
			submitButton.Enabled = false;

			// Заполнение папками
			UIIcon folderIcon = new UIIcon(DirectoryInspectorIcons.Folder);
			foreach (Project.Folder folder in currentDir.Folders) {
				NSDirectoryInspector.Entry en = new NSDirectoryInspector.Entry();
				en.Tag = (object)folder;
				en.IsDraggable = false;
				en.Name = folder.Name;
				en.MainIcon = folderIcon;
				inspector.Entries.Add(en);
			}

			// Заполнение файлами
			foreach (Project.Entry e in currentDir.Entries) {
				if (Dropper.FileSupported(e.Name)) {
					NSDirectoryInspector.Entry en = new NSDirectoryInspector.Entry();
					en.Tag = (object)e;
					en.IsDraggable = false;
					en.Name = e.NameWithoutExt;
					en.SubName = FileTypeManager.GetName(e);
					en.MainIcon = e.Icon.Icon;
					if (e.Icon.ShowSubIcon) {
						en.BulletIcon = e.Icon.SubIcon;
					}
					inspector.Entries.Add(en);
					if (select && prevFile == e) {
						inspector.SelectedEntry = inspector.Entries[inspector.Entries.Count - 1];
						submitButton.Enabled = true;
					}
				}
			}

		}
	}
}
