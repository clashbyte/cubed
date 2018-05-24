using System;
using System.Drawing;

namespace Cubed.Data.Defines {

	/// <summary>
	/// Basical project info
	/// </summary>
	public class ProjectBasicInfo {

		/// <summary>
		/// Name
		/// </summary>
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// Author
		/// </summary>
		public string Author {
			get;
			set;
		}

		/// <summary>
		/// Splash image
		/// </summary>
		public Image Splash {
			get;
			set;
		}

		/// <summary>
		/// Generating basic info for folder
		/// </summary>
		/// <param name="folder">Folder name</param>
		/// <returns>Info data</returns>
		public static ProjectBasicInfo GetDefaultInfo(string folder) {
			ProjectBasicInfo info = new ProjectBasicInfo();
			info.Name = folder;
			info.Author = Environment.UserName;
			info.Splash = Resources.ProjectIcon;
			return info;
		}
	}
}
