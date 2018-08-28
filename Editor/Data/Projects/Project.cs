using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Cubed.Data.Defines;
using Cubed.Data.Editor.Previews;
using Cubed.Drivers.Files;

namespace Cubed.Data.Projects {

	/// <summary>
	/// Class for project handling
	/// </summary>
	public static class Project {

		/// <summary>
		/// Array for single scan result
		/// </summary>
		public static event EventHandler<EntryEventArgs> EntryChangedEvent;

		/// <summary>
		/// Array for all scan results
		/// </summary>
		public static event EventHandler<MultipleEntryEventArgs> EntriesChangedEvent;

		/// <summary>
		/// Current project information
		/// </summary>
		public static ProjectBasicInfo Info {
			get;
			private set;
		}

		/// <summary>
		/// Root folder for current project
		/// </summary>
		public static Project.Folder Root {
			get;
			private set;
		}

		/// <summary>
		/// Watcher for changes tracking
		/// </summary>
		static ProjectWatcher watcher;

		/// <summary>
		/// Driver
		/// </summary>
		static FileSystem driver;

		/// <summary>
		/// Path to project folder
		/// </summary>
		static string rootPath;

		/// <summary>
		/// Picking entry by name
		/// </summary>
		/// <param name="path">Path in project</param>
		/// <returns>Entry or null</returns>
		public static Project.EntryBase GetFile(string path) {
			path = path.ToLower();
			string[] parts = path.Split(new char[] {
				'/', '\\'	
			}, StringSplitOptions.RemoveEmptyEntries);
			Folder current = Root;
			for (int i = 0; i < parts.Length; i++) {
				bool found = false;
				foreach (Folder f in ((Folder)current).Folders) {
					if (f.Name.ToLower() == parts[i]) {
						if (i < parts.Length - 1) {
							current = f;
							found = true;
							break;
						} else {
							return f;
						}
					}
				}
				if (found) {
					continue;
				}
				foreach (Entry e in ((Folder)current).Entries) {
					if (e.Name.ToLower() == parts[i]) {
						if (i == parts.Length - 1) {
							return e;
						} else {
							return null;
						}
					}
				}
			}
			return current;
		}

		/// <summary>
		/// Get project info by folder
		/// </summary>
		/// <returns></returns>
		public static ProjectBasicInfo GetInfoByFolder(string folder) {
			if (File.Exists(Path.Combine(folder, ".cubed"))) {
				return ProjectBasicInfo.Read(folder);
			}
			return null;
		}

		/// <summary>
		/// Opening project folder
		/// </summary>
		/// <returns>True if project opened</returns>
		public static bool Open(string folder) {
			if (File.Exists(Path.Combine(folder, ".cubed"))) {
				Info = ProjectBasicInfo.Read(folder);
				rootPath = Path.GetFullPath(folder);
				driver = new Cubed.Drivers.Files.FolderFileSystem() {
					RootFolder = rootPath
				};
				Root = CreateFileTree("/");
				watcher = new ProjectWatcher();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Updating project state
		/// </summary>
		public static void Update() {
			List<EntryEventArgs> events = new List<EntryEventArgs>();
			ProjectWatcher.FileChange[] changes = watcher.Update();
			foreach (ProjectWatcher.FileChange fc in changes) {
				string path = fc.Path.Substring(rootPath.Length).Replace("\\", "/");
				Folder parent = GetFile(Path.GetDirectoryName(path)) as Folder;
				switch (fc.Event) {
					
					case EntryEvent.Created:
						// Added file or folder
						EntryBase eb = null;
						if (File.Exists(fc.Path)) {
							eb = new Entry(path, File.GetLastWriteTime(fc.Path), parent);
							List<Entry> ffiles = new List<Entry>(parent.Entries);
							ffiles.Add(eb as Entry);
							parent.SetChildren(parent.Folders, ffiles.ToArray());
						} else {
							eb = ScanSubfolder(path, null, parent);
							List<Folder> ffolders = new List<Folder>(parent.Folders);
							ffolders.Add(eb as Folder);
							parent.SetChildren(ffolders.ToArray(), parent.Entries);
						}
						events.AddRange(NotifyEntry(eb, EntryEvent.Created));
						break;

					case EntryEvent.Modified:
						// File or folder modified
						EntryBase mb = GetFile(path);
						if (mb != null) {
							events.AddRange(NotifyEntry(mb, EntryEvent.Modified));
						} else {
							throw new Exception("Unable to locate modified file");
						}
						break;

					case EntryEvent.Deleted:
						// File or folder removed
						EntryBase db = GetFile(path);
						if (db != null) {
							events.AddRange(NotifyEntry(db, EntryEvent.Deleted));
							if (db is Entry) {
								List<Entry> ffiles = new List<Entry>(parent.Entries);
								ffiles.Remove(db as Entry);
								parent.SetChildren(parent.Folders, ffiles.ToArray());
							} else {
								List<Folder> ffolders = new List<Folder>(parent.Folders);
								ffolders.Add(db as Folder);
								parent.SetChildren(ffolders.ToArray(), parent.Entries);
							}
						} else {
							throw new Exception("Unable to locate removed entry");
						}
						break;

				}
			}
			if (events.Count > 0) {
				MultipleEntryEventArgs mea = new MultipleEntryEventArgs() {
					Events = events.ToArray()
				};
				foreach (EntryEventArgs ea in mea.Events) {
					if (ea.Entry is Entry && ea.Type == EntryEvent.Modified) {
						Preview.Update(ea.Entry as Entry);
					}
				}
				if (EntriesChangedEvent != null) {
					EntriesChangedEvent(null, mea);
				}
				if (EntryChangedEvent != null) {
					foreach (EntryEventArgs ea in mea.Events) {
						if (ea.Entry is Entry && ea.Type == EntryEvent.Modified) {
							Preview.Update(ea.Entry as Entry);
						}
						EntryChangedEvent(null, ea);
					}
				}
			}
		}

		/// <summary>
		/// Closing the project
		/// </summary>
		public static void Close() {
			if (rootPath != string.Empty) {
				rootPath = "";
				Info = null;
				watcher.Stop();
				watcher = null;
			}
		}

		/// <summary>
		/// Scanning project tree
		/// </summary>
		static Folder CreateFileTree(string path) {
			return ScanSubfolder(path);
		}

		/// <summary>
		/// Scanning subfolder for files and other folders
		/// </summary>
		/// <param name="path">Path to scan</param>
		/// <returns>Scanned folder</returns>
		static Folder ScanSubfolder(string path, DirectoryInfo current = null, EntryBase parent = null) {

			List<Entry> entries = new List<Entry>();
			List<Folder> folders = new List<Folder>();
			if (current == null) {
				current = new DirectoryInfo(Path.GetFullPath(Path.Combine(rootPath, "." + path)));
			}
			Folder folder = new Folder(path, current.LastWriteTime, parent);

			// Iterating folders
			DirectoryInfo[] folderInfos = current.GetDirectories();
			foreach (DirectoryInfo di in folderInfos) {
				Folder f = ScanSubfolder(Path.Combine(path, di.Name).Replace("\\", "/"), di, folder);
				if (f != null) {
					folders.Add(f);
				}
			}

			// Iterating files
			FileInfo[] fileInfos = current.GetFiles();
			foreach (FileInfo fi in fileInfos) {
				if (!fi.Name.StartsWith(".")) {
					entries.Add(new Entry(Path.Combine(path, fi.Name).Replace("\\", "/"), fi.LastWriteTime, folder));	
				}
			}
			folder.SetChildren(folders, entries);
			
			// Making folder
			return folder;
		}

		/// <summary>
		/// Recursive entry notification
		/// </summary>
		/// <param name="entry">Entry</param>
		/// <param name="ev">Event type</param>
		/// <returns>Array of output events</returns>
		static EntryEventArgs[] NotifyEntry(EntryBase entry, EntryEvent ev) {
			List<EntryEventArgs> list = new List<EntryEventArgs>();
			if (entry is Folder) {
				Folder fld = entry as Folder;
				foreach (Folder f in fld.Folders) {
					list.AddRange(NotifyEntry(f, ev));
				}
				foreach (Entry e in fld.Entries) {
					list.AddRange(NotifyEntry(e, ev));
				}
			}
			if (entry is Folder || entry is Entry) {
				list.Add(new EntryEventArgs() {
					Entry = entry,
					Type = ev
				});
			}
			return list.ToArray();
		}


		/// <summary>
		/// Base class for all project entries
		/// </summary>
		public abstract class EntryBase {

			/// <summary>
			/// Parents
			/// </summary>
			public EntryBase Parent {
				get;
				set;
			}

			/// <summary>
			/// Local path in project
			/// </summary>
			public string Path {
				get;
				protected set;
			}

			/// <summary>
			/// Entry name without path
			/// </summary>
			public string Name {
				get {
					return System.IO.Path.GetFileName(Path);
				}
			}

			/// <summary>
			/// Full path in OS
			/// </summary>
			public string FullPath {
				get {
					return System.IO.Path.GetFullPath(System.IO.Path.Combine(rootPath, "."+Path));
				}
			}

			/// <summary>
			/// Last time this entry modified
			/// </summary>
			public DateTime LastModifyTime {
				get;
				set;
			}

			/// <summary>
			/// Conversion to string
			/// </summary>
			/// <returns>String data</returns>
			public override string ToString() {
				return Name;
			}

		}

		/// <summary>
		/// Class for storing project entries
		/// </summary>
		public class Entry : EntryBase {

			/// <summary>
			/// Entry name without extension
			/// </summary>
			public string NameWithoutExt {
				get {
					return System.IO.Path.GetFileNameWithoutExtension(Path);
				}
			}

			/// <summary>
			/// Preview for this entry
			/// </summary>
			public Preview Icon {
				get {
					if (preview == null) {
						preview = Preview.Get(this);
					}
					return preview;
				}
			}

			/// <summary>
			/// Internal preview field
			/// </summary>
			Preview preview;

			/// <summary>
			/// Entry constructor
			/// </summary>
			/// <param name="path">File path</param>
			public Entry(string path, DateTime changed, EntryBase parent) {
				Path = path;
				LastModifyTime = changed;
				Parent = parent;
			}

		}

		/// <summary>
		/// Class for folders in project
		/// </summary>
		public class Folder : EntryBase {

			/// <summary>
			/// Child entries
			/// </summary>
			public Entry[] Entries {
				get;
				private set;
			}

			/// <summary>
			/// Child folders
			/// </summary>
			public Folder[] Folders {
				get;
				private set;
			}

			/// <summary>
			/// Folder constructor
			/// </summary>
			/// <param name="folders">Collection of nested folders</param>
			/// <param name="entries">Collection of nested entries</param>
			public Folder(string path, DateTime changed, EntryBase parent) {
				Path = path;
				LastModifyTime = changed;
				Entries = new Entry[0];
				Folders = new Folder[0];
				Parent = parent;
			}

			/// <summary>
			/// Changing siblings
			/// </summary>
			/// <param name="folders">Collection of nested folders</param>
			/// <param name="entries">Collection of nested entries</param>
			public void SetChildren(IEnumerable<Folder> folders, IEnumerable<Entry> entries) {
				if (folders != null) {
					Folders = folders.ToArray();
				} else {
					Folders = new Folder[0];
				}
				if (entries != null) {
					Entries = entries.ToArray();
				} else {
					Entries = new Entry[0];
				}
			}

		}

		/// <summary>
		/// Event for entry
		/// </summary>
		public enum EntryEvent {
			Created,
			Modified,
			Deleted
		}

		/// <summary>
		/// Arguments for entry event
		/// </summary>
		public class EntryEventArgs : EventArgs {
			public EntryBase Entry;
			public EntryEvent Type;
		}

		/// <summary>
		/// Collection of events
		/// </summary>
		public class MultipleEntryEventArgs : EventArgs {
			public EntryEventArgs[] Events;
		}

		/// <summary>
		/// Watcher for project
		/// </summary>
		class ProjectWatcher {

			/// <summary>
			/// Debounce delay
			/// </summary>
			const int DEBOUNCE_DELAY = 300;

			/// <summary>
			/// Entries last events
			/// </summary>
			Dictionary<DebouncedFile, long> entries;

			/// <summary>
			/// Changes list
			/// </summary>
			ConcurrentQueue<FileChange> changeList;

			/// <summary>
			/// Internal backing thread
			/// </summary>
			Thread watchingThread;

			/// <summary>
			/// Starting watcher
			/// </summary>
			public ProjectWatcher() {
				changeList = new ConcurrentQueue<FileChange>();
				watchingThread = new Thread(ThreadedWatch);
				watchingThread.IsBackground = true;
				watchingThread.Priority = ThreadPriority.BelowNormal;
				watchingThread.Start();
			}

			/// <summary>
			/// Updating data
			/// </summary>
			/// <returns></returns>
			public FileChange[] Update() {
				List<FileChange> evl = new List<FileChange>();
				while (!changeList.IsEmpty) {
					FileChange fc = null;
					if (changeList.TryDequeue(out fc)) {
						evl.Add(fc);
					}
				}
				return evl.ToArray();
			}

			/// <summary>
			/// Stopping watcher
			/// </summary>
			public void Stop() {
				if (watchingThread != null) {
					watchingThread.Abort();
					watchingThread = null;
				}
			}

			/// <summary>
			/// Threaded watching
			/// </summary>
			void ThreadedWatch() {
				entries = new Dictionary<DebouncedFile, long>();
				FileSystemWatcher fw = new FileSystemWatcher();
				fw.InternalBufferSize = 8192;
				fw.Path = rootPath;
				fw.IncludeSubdirectories = true;
				fw.EnableRaisingEvents = true;
				fw.Created += fw_Event;
				fw.Changed += fw_Event;
				fw.Renamed += fw_Event;
				fw.Deleted += fw_Event;
				try {
					while (true) {
						long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
						List<DebouncedFile> dlist = new List<DebouncedFile>();
						foreach (var d in entries) {
							if (d.Value < now) {
								dlist.Add(d.Key);
							}
						}
						foreach (DebouncedFile df in dlist) {
							changeList.Enqueue(new FileChange() {
								Event = df.Event,
								Path = df.Path
							});
							entries.Remove(df);
						}
						Thread.Sleep(5);
					}
				} catch (ThreadAbortException e) { }
				fw.EnableRaisingEvents = false;
			}

			/// <summary>
			/// Handling event
			/// </summary>
			/// <param name="sender">Watcher</param>
			/// <param name="e">Information</param>
			void fw_Event(object sender, FileSystemEventArgs e) {

				// Decoding data
				List<DebouncedFile> events = new List<DebouncedFile>();
				switch (e.ChangeType) {
					case WatcherChangeTypes.Changed:
						events.Add(new DebouncedFile() {
							Path = e.FullPath,
							Event = EntryEvent.Modified
						});
						break;
					case WatcherChangeTypes.Created:
						events.Add(new DebouncedFile() {
							Path = e.FullPath,
							Event = EntryEvent.Created
						});
						break;
					case WatcherChangeTypes.Deleted:
						events.Add(new DebouncedFile() {
							Path = e.FullPath,
							Event = EntryEvent.Deleted
						});
						break;
					case WatcherChangeTypes.Renamed:
						events.Add(new DebouncedFile() {
							Path = (e as RenamedEventArgs).OldFullPath,
							Event = EntryEvent.Deleted
						});
						events.Add(new DebouncedFile() {
							Path = e.FullPath,
							Event = EntryEvent.Created
						});
						break;
				}

				// Reading data
				long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				foreach (DebouncedFile df in events) {
					bool found = false;
					foreach (KeyValuePair<DebouncedFile, long> d in entries) {
						if (d.Key.Event == df.Event && d.Key.Path == df.Path) {
							entries[d.Key] = now + DEBOUNCE_DELAY;
							found = true;
							break;
						}
					}
					if (!found) {
						entries.Add(df, now + DEBOUNCE_DELAY);
					}
				}
			}

			/// <summary>
			/// File change data
			/// </summary>
			public class FileChange {

				/// <summary>
				/// Path to entry
				/// </summary>
				public string Path {
					get;
					set;
				}

				/// <summary>
				/// Event type
				/// </summary>
				public EntryEvent Event {
					get;
					set;
				}
			}

			/// <summary>
			/// Debounced entry
			/// </summary>
			class DebouncedFile {
				
				/// <summary>
				/// File path
				/// </summary>
				public string Path {
					get;
					set;
				}

				/// <summary>
				/// Entry event type
				/// </summary>
				public EntryEvent Event {
					get;
					set;
				}
			}
		} 
	}
}
