using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Data.Editor;
using Cubed.Data.Files;
using Cubed.Data.Projects;
using Cubed.Forms.Dialogs;
using Cubed.UI.Graphics;

namespace Cubed.Forms.Common {

	/// <summary>
	/// Main prototype for editors
	/// </summary>
	public partial class EditorForm : Form {

		/// <summary>
		/// Custom icon for editor
		/// </summary>
		public virtual UIIcon CustomIcon {
			get {
				return null;
			}
		}

		/// <summary>
		/// Updating text
		/// </summary>
		public override string Text {
			get {
				if (File != null) {
					return File.NameWithoutExt + " - " + FileTypeManager.GetName(File);
				}
				return "Unknown file";
			}
			set {}
		}

		/// <summary>
		/// Is current file saved
		/// </summary>
		public bool Saved {
			get {
				return saved;
			}
			set {
				saved = value;
				if (Parent != null) {
					if (Parent.Parent != null) {
						Parent.Parent.Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Current project entry
		/// </summary>
		public Project.Entry File {
			get;
			protected set;
		}

		/// <summary>
		/// Saved flag
		/// </summary>
		bool saved = true;

		/// <summary>
		/// Supressing events on saving
		/// </summary>
		bool supressEvents = false;

		/// <summary>
		/// Constructor
		/// </summary>
		public EditorForm() {
			InitializeComponent();
			Saved = true;
		}

		/// <summary>
		/// Showing editor
		/// </summary>
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			Project.EntriesChangedEvent += Project_EntriesChangedEvent;
		}

		/// <summary>
		/// Closing editor
		/// </summary>
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);

			Project.EntriesChangedEvent -= Project_EntriesChangedEvent;
		}

		/// <summary>
		/// Setting file
		/// </summary>
		/// <param name="entry">File to read</param>
		public void SetFile(Project.Entry entry) {
			File = entry;
			Load();
			Saved = true;
		}

		/// <summary>
		/// Reading file data
		/// </summary>
		/// <returns>Chunk or null</returns>
		public Chunk Read() {
			if (System.IO.File.Exists(File.FullPath)) {
				return ChunkedFile.Read(File.FullPath, true);
			}
			return null;
		}

		/// <summary>
		/// Writing data to file
		/// </summary>
		/// <param name="content">Data to write</param>
		public void Write(Chunk content) {
			string error = "";
			if (content != null) {
				if (!ChunkedFile.Write(File.FullPath, content, out error)) {
					MessageDialog.Open("Error", error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		/// <summary>
		/// Entries events
		/// </summary>
		void Project_EntriesChangedEvent(object sender, Project.MultipleEntryEventArgs e) {
			foreach (Project.EntryEventArgs ea in e.Events) {
				if (ea.Entry == File) {
					if (!supressEvents) {
						if (ea.Type == Project.EntryEvent.Modified) {
							if (MessageDialog.Open("File changed", "Opened file is changed in another program. Do you want to reload it?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes) {
								Load();
								Saved = true;
							} else {
								Saved = false;
							}
						} else if (ea.Type == Project.EntryEvent.Deleted) {
							Saved = false;
							if (MessageDialog.Open("File deleted", "Opened file is removed from disk. Do you want to keep it in editor?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No) {
								
							}
						}
					}
					supressEvents = false;
					break;
				}
			}
		}

		/// <summary>
		/// Starting to save cell
		/// </summary>
		public void StartSaving() {
			supressEvents = true;
		}

		/// <summary>
		/// Pausing editor
		/// </summary>
		public virtual void Pause() { }

		/// <summary>
		/// Resuming editor
		/// </summary>
		public virtual void Resume() { }

		/// <summary>
		/// Saving file
		/// </summary>
		public virtual void Save() { }

		/// <summary>
		/// Loading file
		/// </summary>
		public virtual void Load() { }
	}
}
