using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.Data.Defines;
using Cubed.Drivers.Files;

namespace Cubed.Data.Projects {

	/// <summary>
	/// Class for project handling
	/// </summary>
	public static class Project {

		/// <summary>
		/// Driver
		/// </summary>
		static FileSystem driver;

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

		
	}
}
