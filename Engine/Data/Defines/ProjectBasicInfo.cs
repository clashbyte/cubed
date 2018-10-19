using System;
using System.Drawing;
using System.IO;
using Cubed.Data.EditorGlue.Attributes;
using Cubed.Data.Files;

namespace Cubed.Data.Defines {

	/// <summary>
	/// Basical project info
	/// </summary>
	[Serializable]
	public class ProjectBasicInfo {

		/// <summary>
		/// Name
		/// </summary>
		[HintedName("ProjectName")]
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// Author
		/// </summary>
		[HintedName("ProjectAuthor")]
		public string Author {
			get;
			set;
		}

		/// <summary>
		/// Icon for project
		/// </summary>
		[HintedName("ProjectIcon")]
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
			info.Name = System.IO.Path.GetFileName(folder);
			info.Author = Environment.UserName;
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
		/// Saving project
		/// </summary>
		/// <param name="parent">Parent chunk</param>
		internal void Save(ContainerChunk parent) {

			// Basic project info
			KeyValueChunk kc = new KeyValueChunk();
			kc.ID = "PROJ";
			kc.Version = 1;
			kc.Content.Add("Name", Name);
			kc.Content.Add("Author", Author);
			parent.Children.Add(kc);

			// Writing icon
			if (Icon != null) {
				BinaryChunk bc = new BinaryChunk();
				bc.ID = "ICON";
				bc.Version = 1;

				MemoryStream mstream = new MemoryStream();
				Icon.Save(mstream, System.Drawing.Imaging.ImageFormat.Png);
				bc.Content = mstream.ToArray();
				parent.Children.Add(bc);

			}

		}

		/// <summary>
		/// Converting from chunk
		/// </summary>
		/// <param name="cont">Chunk</param>
		/// <returns></returns>
		internal static ProjectBasicInfo FromChunk(ContainerChunk cont, bool fallbackOnError = true) {
			ProjectBasicInfo info = null;
			Image icon = null;

			if (cont != null) {
				foreach (var item in cont.Children) {
					if (item.ID == "PROJ") {
						KeyValueChunk k = item as KeyValueChunk;

						// Reading basic project data
						info = new ProjectBasicInfo();
						info.Name = k.Content["Name"];
						info.Author = k.Content["Author"];

					} else if (item.ID == "ICON") {
						BinaryChunk b = item as BinaryChunk;

						// Reading icon
						icon = Image.FromStream(new MemoryStream(b.Content));
					}
				}
			}

			// Validating data
			if (icon == null || info == null) {
				if (fallbackOnError) {
					ProjectBasicInfo wrong = new ProjectBasicInfo();
					wrong.Name = "Damaged project";
					wrong.Author = "Unknown";
					wrong.Icon = Resources.ProjectErrorIcon;
					return wrong;
				}
				return null;
			}
			info.Icon = icon;
			return info;
		}
	}
}
