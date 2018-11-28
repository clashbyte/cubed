using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubed.Drivers.Files {

	/// <summary>
	/// File system for folder
	/// </summary>
	public class FolderFileSystem : FileSystem {

		/// <summary>
		/// Root folder for filesystem
		/// </summary>
		public string RootFolder {
			get;
			set;
		}

		/// <summary>
		/// Check for file existense
		/// </summary>
		/// <param name="file">File name</param>
		/// <returns>True if file exists</returns>
		public override bool Exists(string file) {
			return File.Exists(calcPath(file));
		}

		/// <summary>
		/// Get file size
		/// </summary>
		/// <param name="file">File name</param>
		/// <returns>Size in bytes</returns>
		public override int Size(string file) {
			if (Exists(file)) {
				return (int)(new FileInfo(calcPath(file))).Length;
			}
			return 0;
		}

		/// <summary>
		/// Read file content
		/// </summary>
		/// <param name="file">File name</param>
		/// <returns>Byte array of data or null</returns>
		public override byte[] Get(string file) {
			if(Exists(file)) {
				return File.ReadAllBytes(calcPath(file));
			}
			return null;
		}

		/// <summary>
		/// Reading file stream
		/// </summary>
		/// <param name="file">File</param>
		/// <returns>Stream or null</returns>
		public override Stream GetStream(string file) {
			if (Exists(file)) {
				return new FileStream(calcPath(file), FileMode.Open, FileAccess.Read);
			}
			return null;
		}

		/// <summary>
		/// Calculate path
		/// </summary>
		/// <param name="file">Specified path</param>
		/// <returns>Full path to file</returns>
		string calcPath(string file) {
			if (file.StartsWith("/") || file.StartsWith("\\")) {
				file = file.Substring(1);
			}
			return Path.Combine(RootFolder, file);
		}

	}

}
