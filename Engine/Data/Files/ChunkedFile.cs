using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.Core;

namespace Cubed.Data.Files {

	/// <summary>
	/// Chunk container
	/// </summary>
	public class ChunkedFile : Chunk {

		/// <summary>
		/// Reading file
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Chunk Read(string path, bool realFiles = false) {
			byte[] data = null;
			if (realFiles) {
				data = File.ReadAllBytes(path);
			} else {
				if (Engine.Current == null || Engine.Current.Filesystem == null || !Engine.Current.Filesystem.Exists(path)) {
					return null;
				}
				data = Engine.Current.Filesystem.Get(path);
			}
			
			// Reading chunk
			return Chunk.ReadRaw(data);
		}

		/// <summary>
		/// Writing file (only to filesystem)
		/// </summary>
		/// <param name="path">Path to write</param>
		/// <returns></returns>
		public static bool Write(string path, Chunk chunk, out string error) {
			byte[] data = Chunk.WriteRaw(chunk);
			error = null;
			try {
				File.WriteAllBytes(path, data);
			} catch (Exception e) {
				error = e.ToString();
				return false;
			}
			return true;
		}

		/// <summary>
		/// Reading data
		/// </summary>
		/// <param name="data"></param>
		protected override void Read(byte[] data) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Writing data
		/// </summary>
		/// <returns></returns>
		protected override byte[] Write() {
			throw new NotImplementedException();
		}
	}
}
