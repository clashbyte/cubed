using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
		static FileSystemWatcher watcher;

		/// <summary>
		/// Driver
		/// </summary>
		static FileSystem driver;

		/// <summary>
		/// Path to project folder
		/// </summary>
		static string rootPath;

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
				return true;
			}
			return false;
		}

		/// <summary>
		/// Rescanning project
		/// </summary>
		public static void Rescan() {

			// Accum for entry events
			List<EntryEventArgs> evList = new List<EntryEventArgs>();

			// Reading root
			Folder newFolder = CreateFileTree("/");
			CompareRecursive(Root, newFolder, evList);

			// Calling hooks
			if (EntriesChangedEvent != null && evList.Count > 0) {
				EntriesChangedEvent(null, new MultipleEntryEventArgs() {
					Events = evList.ToArray()
				});
			}

		}

		/// <summary>
		/// Comparing folders
		/// </summary>
		/// <param name="prev"></param>
		/// <param name="next"></param>
		static void CompareRecursive(Folder prev, Folder next, List<EntryEventArgs> evList) {

			// Saving all new folders
			List<string> prevFolders = new List<string>();
			List<string> nextFolders = new List<string>();
			List<Folder> newFolders = new List<Folder>();
			foreach (Folder fld in prev.Folders) {
				prevFolders.Add(fld.Path);
			}
			foreach (Folder fld in next.Folders) {
				nextFolders.Add(fld.Path);
			}

			// Scanning for removed folders
			foreach (Folder fld in prev.Folders) {
				if (!nextFolders.Contains(fld.Path)) {
					evList.AddRange(Notify(fld, EntryEvent.Deleted, true));
				} else {
					newFolders.Add(fld);
				}
			}
			foreach (Folder nfld in next.Folders) {
				if (!prevFolders.Contains(nfld.Path)) {
					nfld.Parent = prev;
					evList.AddRange(Notify(nfld, EntryEvent.Created, true));
					newFolders.Add(nfld);
				}
			}
			foreach (Folder fl in newFolders) {
				foreach (Folder ofl in prev.Folders) {
					if (fl.Path == ofl.Path) {
						CompareRecursive(ofl, fl, evList);
						break;
					}
				}
			}

			// Scanning for files
			List<string> prevFiles = new List<string>();
			List<string> nextFiles = new List<string>();
			List<Entry> newEntries = new List<Entry>();
			foreach (Entry ent in prev.Entries) {
				prevFiles.Add(ent.Path);
			}
			foreach (Entry ent in next.Entries) {
				nextFiles.Add(ent.Path);
			}

			// Scanning for removed folders
			foreach (Entry ent in prev.Entries) {
				if (!nextFiles.Contains(ent.Path)) {
					evList.AddRange(Notify(ent, EntryEvent.Deleted));
				} else {
					foreach (Entry nent in next.Entries) {
						if (nent.Path == ent.Path) {
							if (nent.LastModifyTime > ent.LastModifyTime) {
								ent.LastModifyTime = nent.LastModifyTime;
								evList.AddRange(Notify(ent, EntryEvent.Modified));
							}
							break;
						}
					}
					newEntries.Add(ent);
				}
			}
			foreach (Entry nent in next.Entries) {
				if (!prevFiles.Contains(nent.Path)) {
					nent.Parent = prev;
					evList.AddRange(Notify(nent, EntryEvent.Created));
					newEntries.Add(nent);
				}
			}

			// Setting child entries
			prev.SetChildren(newFolders, newEntries);
		}

		/// <summary>
		/// Notifying single entry
		/// </summary>
		/// <param name="entry">Entry</param>
		/// <param name="type">Type of event</param>
		static EntryEventArgs[] Notify(EntryBase entry, EntryEvent type, bool recursive = false) {
			List<EntryEventArgs> lst = new List<EntryEventArgs>();
			if (recursive && entry is Folder) {
				Folder fld = entry as Folder;
				foreach (Folder ifd in fld.Folders) {
					lst.AddRange(Notify(ifd, type, true));
				}
				foreach (Entry en in fld.Entries) {
					lst.Add(BroadcastEvent(en, type));
				}
			} else if(entry is Entry) {
				if (type == EntryEvent.Modified) {
					Preview.Update(entry as Entry);
				} else if(type == EntryEvent.Deleted) {
					Preview.Remove(entry as Entry);
				}
			}
			lst.Add(BroadcastEvent(entry, type));
			return lst.ToArray();
		}

		/// <summary>
		/// Broadcasting event for single item
		/// </summary>
		/// <param name="entry">Entry</param>
		/// <param name="ev">Event type</param>
		static EntryEventArgs BroadcastEvent(EntryBase entry, EntryEvent ev) {
			EntryEventArgs ea = new EntryEventArgs() {
				Entry = entry,
				Type = ev
			};
			if (EntryChangedEvent != null) {
				EntryChangedEvent(null, ea);
			}
			return ea;
		}

		/// <summary>
		/// Closing the project
		/// </summary>
		public static void Close() {
			if (rootPath != string.Empty) {
				rootPath = "";
				Info = null;
				watcher.Dispose();
				watcher = null;
			}
		}

		/// <summary>
		/// Scanning project tree
		/// </summary>
		private static Folder CreateFileTree(string path) {
			Folder f = ScanSubfolder(path);
			return f;
		}

		/// <summary>
		/// Scanning subfolder for files and other folders
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
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

	}
}
