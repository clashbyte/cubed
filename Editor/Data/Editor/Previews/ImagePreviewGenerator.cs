using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Cubed.Data.Editor.Previews {
	
	/// <summary>
	/// Generator for images
	/// </summary>
	public class ImagePreviewGenerator : PreviewGenerator {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path"></param>
		public ImagePreviewGenerator(string path)
			: base(path) {

		}

		/// <summary>
		/// Making previews for basic images
		/// </summary>
		public override void Generate() {
			
			Image src = Image.FromFile(Path);
			if (src == null) {
				Result = null;
				return;
			}

			Result = ScaleForEditor(src);
			src.Dispose();
			ShowSubIcon = false;
		}

	}
}
