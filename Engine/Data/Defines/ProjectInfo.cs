using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.Data.Files;

namespace Cubed.Data.Defines {
	
	/// <summary>
	/// Project information
	/// </summary>
	public class ProjectInfo {

		/// <summary>
		/// Basic project information
		/// </summary>
		public ProjectBasicInfo Basic {
			get;
			private set;
		}

		/// <summary>
		/// Reading project default values
		/// </summary>
		/// <param name="path">Path to project file</param>
		/// <returns>Basic project info</returns>
		public static ProjectInfo Read(string path = null, bool fallback = true) {

			// Opening info
			Chunk chunk = null;
			if (string.IsNullOrEmpty(path)) {
				chunk = ChunkedFile.Read(".cubed", false);
			} else {
				chunk = ChunkedFile.Read(Path.Combine(path, ".cubed"), true);
			}
			return FromChunk(chunk as ContainerChunk, fallback);
		}

		/// <summary>
		/// Converting from chunk
		/// </summary>
		/// <param name="cont">Chunk</param>
		/// <returns></returns>
		static ProjectInfo FromChunk(ContainerChunk cont, bool fallbackOnError = true) {
			ProjectInfo info = new ProjectInfo();
			info.Basic = ProjectBasicInfo.FromChunk(cont, fallbackOnError);


			return info;
		}

	}


}
