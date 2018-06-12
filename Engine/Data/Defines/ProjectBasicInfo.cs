using System;
using System.Drawing;
using System.IO;
using Cubed.Data.Files;

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
		/// Icon for project
		/// </summary>
		public Image Icon {
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
			info.Splash = new Bitmap(1, 1);
			info.Icon = Resources.ProjectIcon;
			return info;
		}

		/// <summary>
		/// Reading project default values
		/// </summary>
		/// <param name="path">Path to project file</param>
		/// <returns>Basic project info</returns>
		public static ProjectBasicInfo Read(string path = null, bool fallback = true) {
			
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
		static ProjectBasicInfo FromChunk(ContainerChunk cont, bool fallbackOnError = true) {
			ProjectBasicInfo info = null;
			Image splash = null, icon = null;

			if (cont != null) {
				foreach (var item in cont.Children) {
					if (item.ID == "PROJ") {
						KeyValueChunk k = item as KeyValueChunk;

						// Reading basic project data
						info = new ProjectBasicInfo();
						info.Name = k.Content["Name"];
						info.Author = k.Content["Author"];

					} else if (item.ID == "IMGS") {
						BinaryChunk b = item as BinaryChunk;

						// Reading splash
						splash = Image.FromStream(new MemoryStream(b.Content));

					} else if (item.ID == "ICON") {
						BinaryChunk b = item as BinaryChunk;

						// Reading icon
						icon = Image.FromStream(new MemoryStream(b.Content));
					}
				}
			}

			// Validating data
			if (icon == null || info == null || splash == null) {
				if (fallbackOnError) {
					ProjectBasicInfo wrong = new ProjectBasicInfo();
					wrong.Name = "Damaged project";
					wrong.Author = "Unknown";
					wrong.Splash = new Bitmap(1, 1);
					wrong.Icon = Resources.ProjectErrorIcon;
					return wrong;
				}
				return null;
			}
			info.Splash = splash;
			info.Icon = icon;
			return info;
		}
	}
}
