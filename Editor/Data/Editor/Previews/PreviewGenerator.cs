using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Cubed.Data.Editor.Previews {

	/// <summary>
	/// Generator for preview
	/// </summary>
	public abstract class PreviewGenerator {

		/// <summary>
		/// Path for file
		/// </summary>
		public string Path {
			get;
			private set;
		}

		/// <summary>
		/// Resulting image
		/// </summary>
		public Image Result {
			get;
			protected set;
		}

		/// <summary>
		/// Resulting animated image
		/// </summary>
		public AnimationFrame[] AnimatedResult {
			get;
			protected set;
		}

		/// <summary>
		/// Flag to display subicon
		/// </summary>
		public bool ShowSubIcon {
			get;
			protected set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to file</param>
		public PreviewGenerator(string path) {
			Path = path;
		}

		/// <summary>
		/// Empty constructor
		/// </summary>
		public PreviewGenerator() { }

		/// <summary>
		/// Background work
		/// </summary>
		public abstract void Generate();

		/// <summary>
		/// Image frame for animated previews
		/// </summary>
		public class AnimationFrame {

			/// <summary>
			/// Current pic
			/// </summary>
			public Image Frame;

			/// <summary>
			/// Delay in milliseconds
			/// </summary>
			public int Delay;

		}

		/// <summary>
		/// Scaling for editor
		/// </summary>
		/// <param name="src">Source image</param>
		/// <returns>Resulting image</returns>
		protected Image ScaleForEditor(Image src) {
			Bitmap m = new Bitmap(128, 128);
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
			return m;
		}
	}
}
