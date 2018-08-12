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

		public override void Generate() {
			Bitmap m = new Bitmap(128, 128);
			Image src = Image.FromFile(Path);
			if (src == null) {
				Result = null;
				return;
			}
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(m)) {
				g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				if (src.Width < 128 && src.Height < 128) {
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				} else {
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				}

				float deltaX = 128f / (float)src.Width;
				float deltaY = 128f / (float)src.Height;
				float delta = deltaX < deltaY ? deltaX : deltaY;
				float cw = (float)src.Width * delta;
				float ch = (float)src.Height * delta;
				g.DrawImage(src, 64f - cw / 2f, 64f - ch / 2f, cw, ch);
			}
			ShowSubIcon = false;
			Result = m;
		}

	}
}
