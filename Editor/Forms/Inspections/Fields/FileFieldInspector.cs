using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Graphics;
using Cubed.Data.Projects;
using Cubed.Forms.Common;
using Cubed.Data.EditorGlue.Attributes;
using Cubed.Data.Editor.Previews;

namespace Cubed.Forms.Inspections.Fields {
	public partial class FileFieldInspector : FieldInspector {

		/// <summary>
		/// Custom updates
		/// </summary>
		bool customUpdate = false;

		/// <summary>
		/// Current mode
		/// </summary>
		PickerMode mode;

		/// <summary>
		/// Current file
		/// </summary>
		Project.Entry currentFile;

		/// <summary>
		/// Constructor
		/// </summary>
		public FileFieldInspector() {
			customUpdate = true;
			InitializeComponent();
			customUpdate = false;
		}

		/// <summary>
		/// Detecting type
		/// </summary>
		public override void DetectType() {

			// Detecting picker type
			HintedFilePickerAttribute fpick = (HintedFilePickerAttribute)Attribute.GetCustomAttribute(Info, typeof(HintedFilePickerAttribute));
			if (fpick != null) {
				mode = PickerMode.StringFile;
				fileDropper.AllowedTypes = fpick.Extensions;
			} else {
				if (Info.PropertyType == typeof(Texture)) {
					mode = PickerMode.Texture;
					fileDropper.AllowedTypes = ".png|.jpg|.jpeg|.png|.gif|.bmp|.anim";
				} else if(Info.PropertyType == typeof(Image)) {
					mode = PickerMode.RawImage;
					fileDropper.AllowedTypes = ".png|.jpg|.jpeg|.png|.gif|.bmp";
				}
			}
		}

		/// <summary>
		/// Reading value
		/// </summary>
		public override void UpdateValue() {
			string path = "";
			Project.Entry en = null;
			object obj = ReadValue();

			switch (mode) {
				case PickerMode.Texture:
					if (obj is Texture) {
						path = (obj as Texture).Link;
					}
					break;
				case PickerMode.Model:
					break;
				case PickerMode.Sound:
					break;
				case PickerMode.StringFile:
					path = obj as string;
					break;
				default:
					break;
			}

			// Path
			if (mode == PickerMode.RawImage) {
				if (obj != null) {
					en = new Project.Entry("", DateTime.Now, null);
					en.Icon = Preview.GetForRawImage(obj as Image);
				}
			} else {
				if (path != "") {
					en = Project.GetFile(path) as Project.Entry;
				}
			}
			currentFile = en;
			customUpdate = true;
			fileDropper.File = currentFile;
			customUpdate = false;
		}
		
		/// <summary>
		/// File changed in dropper
		/// </summary>
		private void fileDropper_FileChanged(object sender, EventArgs e) {
			object value = null;
			if (MainForm.CurrentEngine != null) {
				MainForm.CurrentEngine.MakeCurrent();
			}
			if (!customUpdate) {
				switch (mode) {
					case PickerMode.Texture:
						if (fileDropper.File != null) {
							value = new Texture(fileDropper.File.Path, Texture.LoadingMode.Queued);
						}
						break;
					case PickerMode.Model:
						break;
					case PickerMode.Sound:
						break;
					case PickerMode.StringFile:
						if (fileDropper.File != null) {
							value = fileDropper.File.Path;
						}
						break;
					case PickerMode.RawImage:
						if (fileDropper.File != null) {
							if (fileDropper.File.Path != "") {
								value = Image.FromFile(fileDropper.File.FullPath);
							} else {
								value = ReadValue();
								return;
							}
						}
						break;
					default:
						break;
				}
				SetValue(value);
			}
		}

		/// <summary>
		/// Current picker mode
		/// </summary>
		enum PickerMode {
			Texture,
			Model,
			Sound,
			StringFile,
			RawImage,
			Other
		}
	}
}
