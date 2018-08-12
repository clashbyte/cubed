using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Editor.Previews;
using Cubed.Data.Projects;
using Cubed.Forms.Resources;
using Cubed.UI.Graphics;

namespace Cubed.Data.Editor {
	
	/// <summary>
	/// File type collection
	/// </summary>
	public static class FileTypeManager {

		/// <summary>
		/// Types
		/// </summary>
		static FileType[] types;

		/// <summary>
		/// Constructor
		/// </summary>
		static FileTypeManager() {
			types = new FileType[] {
				new FileType(
					"Image",
					".bmp;.png;.jpg;.jpeg",
					new UIIcon(DirectoryInspectorIcons.NetworkDrive),
					null,
					typeof(ImagePreviewGenerator),
					null
				),
			};
			emptyIcon = new UIIcon(DirectoryInspectorIcons.File);
			emptyName = "???";
		}

		/// <summary>
		/// Unknown name
		/// </summary>
		static string emptyName;

		/// <summary>
		/// Unknown icon
		/// </summary>
		static UIIcon emptyIcon;

		/// <summary>
		/// File record
		/// </summary>
		public class FileType {
			public string Name;
			public string[] Extensions;
			public UIIcon Icon;
			public Type Editor;
			public Type PreviewGenerator;
			public Type LargePreviewer;

			public FileType(string name, string exts, UIIcon icon, Type editor, Type previewGen, Type largePrev) {
				Name = name;
				Extensions = exts.ToLower().Split(';');
				Icon = icon;
				Editor = editor;
				PreviewGenerator = previewGen;
				LargePreviewer = largePrev;
			}
		}

		/// <summary>
		/// Get entry type name
		/// </summary>
		/// <param name="entry">Entry</param>
		/// <returns>Entry type name</returns>
		public static string GetName(Project.Entry entry) {
			FileType ft = FindByEntry(entry);
			if (ft == null) {
				return emptyName;
			}
			return ft.Name;
		}

		/// <summary>
		/// Getting default icon for entry
		/// </summary>
		/// <param name="entry">Entry</param>
		/// <returns></returns>
		public static UIIcon GetIcon(Project.Entry entry) {
			FileType ft = FindByEntry(entry);
			if (ft == null) {
				return emptyIcon;
			}
			return ft.Icon;
		}

		/// <summary>
		/// Pick previewer by entry
		/// </summary>
		/// <param name="entry">Entry</param>
		/// <returns>PreviewGenerator or null</returns>
		public static PreviewGenerator GetPreviewGenerator(Project.Entry entry) {
			FileType ft = FindByEntry(entry);
			if (ft == null || ft.PreviewGenerator == null) {
				return null;
			}
			return Activator.CreateInstance(ft.PreviewGenerator, entry.FullPath) as PreviewGenerator;
		}

		/// <summary>
		/// Searching entry type by extension
		/// </summary>
		/// <param name="entry">Entry</param>
		/// <returns>Type or null</returns>
		static FileType FindByEntry(Project.Entry entry) {
			string n = System.IO.Path.GetExtension(entry.Name).ToLower();
			foreach (FileType ft in types) {
				if (ft.Extensions.Contains(n)) {
					return ft;
				}
			}
			return null;
		}
	}
}
