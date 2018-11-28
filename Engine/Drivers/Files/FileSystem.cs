using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubed.Drivers.Files {
	
	/// <summary>
	/// Base filesystem
	/// </summary>
	public abstract class FileSystem {

		/// <summary>
		/// Check for file existense
		/// </summary>
		/// <param name="file">File name</param>
		/// <returns>True if file exists in filesystem</returns>
		public abstract bool Exists(string file);

		/// <summary>
		/// Get file size
		/// </summary>
		/// <param name="file">File name</param>
		/// <returns>Size in bytes</returns>
		public abstract int Size(string file);

		/// <summary>
		/// Get file content
		/// </summary>
		/// <param name="file">File name</param>
		/// <returns>File content</returns>
		public abstract byte[] Get(string file);

		/// <summary>
		/// Create file stream
		/// </summary>
		/// <param name="file">File name</param>
		/// <returns>File stream</returns>
		public abstract Stream GetStream(string file);

	}
}
