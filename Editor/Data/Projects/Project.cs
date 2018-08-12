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
				if (watcher != null) {
					watcher.Dispose();
					watcher = null;
				}
				watcher = new FileSystemWatcher(folder) {
					IncludeSubdirectories = true,
					EnableRaisingEvents = true
				};
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

			// Reading root
			Folder newFolder = CreateFileTree("/");
			CompareRecursive(Root, newFolder);

		}

		/// <summary>
		/// Comparing folders
		/// </summary>
		/// <param name="prev"></param>
		/// <param name="next"></param>
		static void CompareRecursive(Folder prev, Folder next) {

			// Saving all new folders
			List<string> prevFolders = new List<string>();
			List<string> nextFolders = new List<string>();
			List<Folder> newFolders = new List<Folder>();
			foreach (Folder fld in next.Folders) {
				prevFolders.Add(fld.Path);
			}
			foreach (Folder fld in prev.Folders) {
				nextFolders.Add(fld.Path);
			}

			// Scanning for removed folders
			foreach (Folder fld in prev.Folders) {
				if (!nextFolders.Contains(fld.Path)) {
					Notify(fld, EntryEvent.Deleted);
				} else {
					newFolders.Add(fld);
				}
			}
			foreach (Folder nfld in next.Folders) {
				if (!prevFolders.Contains(nfld.Path)) {
					nfld.Parent = prev;
					Notify(nfld, EntryEvent.Created);
					newFolders.Add(nfld);
				}
			}
			foreach (Folder fl in newFolders) {

			}

			// Scanning for files
			List<string> prevFiles = new List<string>();
			List<string> nextFiles = new List<string>();
			


		}

		/// <summary>
		/// Notifying single entry
		/// </summary>
		/// <param name="entry">Entry</param>
		/// <param name="type">Type of event</param>
		static void Notify(EntryBase entry, EntryEvent type, bool recursive = false) {
			if (recursive && entry is Folder) {
				Folder fld = entry as Folder;
				foreach (Folder ifd in fld.Folders) {
					Notify(ifd, type, true);
				}
				foreach (Entry en in fld.Entries) {
					BroadcastEvent(en, type);
				}
			}
			BroadcastEvent(entry, type);
		}

		/// <summary>
		/// Broadcasting event for single item
		/// </summary>
		/// <param name="entry">Entry</param>
		/// <param name="ev">Event type</param>
		static void BroadcastEvent(EntryBase entry, EntryEvent ev) {
			System.Diagnostics.Debug.WriteLine(entry.Path+" - "+ev);
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
				protected set;
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

	}
}
