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
using Cubed.Forms.Resources;
using Cubed.UI.Graphics;

namespace Cubed.Forms.Common {

	/// <summary>
	/// Main prototype for editors
	/// </summary>
	public partial class EditorForm : Form {

		/// <summary>
		/// Inspecting object
		/// </summary>
		public object InspectingObject {
			get {
				return inspectTarget;
			}
			set {
				inspectTarget = value;
				MainForm.InspectObject(this, value);
			}
		}

		/// <summary>
		/// Internal undo stack depth
		/// </summary>
		protected virtual int UndoStackDepth {
			get {
				return 16;
			}
		}

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
		/// Can editor undo changes
		/// </summary>
		public virtual bool CanUndo {
			get {
				return undoStack.Count > 0;
			}
		}

		/// <summary>
		/// Can editor redo changes
		/// </summary>
		public virtual bool CanRedo {
			get {
				return redoStack.Count > 0;
			}
		}

		/// <summary>
		/// Can user copy or cut items
		/// </summary>
		public virtual bool CanCopyOrCut {
			get {
				return false;
			}
		}

		/// <summary>
		/// Can user paste items
		/// </summary>
		public virtual bool CanPaste {
			get {
				return false;
			}
		}

		/// <summary>
		/// Can user select all items
		/// </summary>
		public virtual bool CanSelectAll {
			get {
				return false;
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
		/// Stack for undo items
		/// </summary>
		Stack<Chunk> undoStack = new Stack<Chunk>();

		/// <summary>
		/// Stack for redo items
		/// </summary>
		Stack<Chunk> redoStack = new Stack<Chunk>();

		/// <summary>
		/// Inspecting object
		/// </summary>
		object inspectTarget;

		/// <summary>
		/// Last changes event time
		/// </summary>
		DateTime lastChangesTime;

		/// <summary>
		/// Constructor
		/// </summary>
		public EditorForm() {
			InitializeComponent();
			Saved = true;
			lastChangesTime = DateTime.Now;
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
							if (MessageDialog.Open(MessageBoxData.fileChangedTitle, MessageBoxData.fileChangedBody, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes) {
								Load();
								Saved = true;
							} else {
								Saved = false;
							}
						} else if (ea.Type == Project.EntryEvent.Deleted) {
							Saved = false;
							if (MessageDialog.Open(MessageBoxData.fileRemovedTitle, MessageBoxData.fileRemovedBody, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No) {
								MainForm.CloseEditor(this, true);
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

		/// <summary>
		/// Undo changes
		/// </summary>
		public void Undo() {
			if (undoStack.Count > 0) {
				Chunk item = undoStack.Pop();
				redoStack.Push(GetHistoryItem());
				if (redoStack.Count > UndoStackDepth) {
					redoStack = new Stack<Chunk>(redoStack.ToArray().Take(UndoStackDepth).Reverse());
				}
				RestoreHistoryItem(item);
				Saved = false;
			}
		}

		/// <summary>
		/// Redo changes
		/// </summary>
		public void Redo() {
			if (redoStack.Count > 0) {
				Chunk item = redoStack.Pop();
				undoStack.Push(GetHistoryItem());
				if (undoStack.Count > UndoStackDepth) {
					undoStack = new Stack<Chunk>(undoStack.ToArray().Take(UndoStackDepth).Reverse());
				}
				RestoreHistoryItem(item);
			}
		}

		/// <summary>
		/// Saving selection to clipboard
		/// </summary>
		/// <param name="cut">Remove objects after copy</param>
		public virtual void Copy(bool cut) {}

		/// <summary>
		/// Paste copied items
		/// </summary>
		public virtual void Paste() {}

		/// <summary>
		/// Select all items
		/// </summary>
		public virtual void SelectAll() {}

		/// <summary>
		/// Internal event for object changes
		/// </summary>
		public virtual void InspectedObjectModified() {
			TriggerChanges();
		}

		/// <summary>
		/// Convert current document to history item
		/// </summary>
		/// <returns>Chunk</returns>
		protected virtual Chunk GetHistoryItem() { return null; }

		/// <summary>
		/// Restoring history item
		/// </summary>
		/// <param name="chunk">Chunk to read from</param>
		protected virtual void RestoreHistoryItem(Chunk chunk) {}

		/// <summary>
		/// Removing saved flag and pushing to undo
		/// </summary>
		protected void TriggerChanges() {
			DateTime now = DateTime.Now;
			if ((now - lastChangesTime).TotalMilliseconds < 500) {
				return;
			}
			undoStack.Push(GetHistoryItem());
			if (undoStack.Count > UndoStackDepth) {
				undoStack = new Stack<Chunk>(undoStack.ToArray().Take(UndoStackDepth).Reverse());
			}
			redoStack.Clear();
			MainForm.UpdateEditingMenu();
			Saved = false;
			lastChangesTime = now;
		}


	}
}
