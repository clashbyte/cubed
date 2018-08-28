using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Cubed.UI.Graphics {

	/// <summary>
	/// Icon for interface
	/// </summary>
	public class UIIcon {

		/// <summary>
		/// Main image
		/// </summary>
		public Image Scan {
			get {
				return img;
			}
			set {
				img = value;
				GenerateShadow();
			}
		}

		/// <summary>
		/// Shadowed version
		/// </summary>
		public Image Shadow {
			get { return shadow; }
		}

		/// <summary>
		/// Internal image and shadow
		/// </summary>
		Image img, shadow;

		/// <summary>
		/// Icon constructor
		/// </summary>
		/// <param name="i">Base image</param>
		public UIIcon(Image i) {
			img = i;
			GenerateShadow();
		}

		/// <summary>
		/// Rendering image to box
		/// </summary>
		/// <param name="to">Icon dimensions</param>
		public void Draw(System.Drawing.Graphics g, Rectangle to, float offset = 2f) {
			if (img == null) {
				return;
			}
			float padX = to.X, padY = to.Y;
			float sizeX = img.Width, sizeY = img.Height;
			float dtX = (float)to.Width / sizeX;
			float dtY = (float)to.Height / sizeY;
			float dtl = (dtX > dtY) ? dtY : dtX;

			sizeX *= dtl;
			sizeY *= dtl;
			padX += (float)to.Width / 2f - sizeX / 2f;
			padY += (float)to.Height / 2f - sizeY / 2f;


			// Saving old params
			CompositingQuality compositingQuality = g.CompositingQuality;
			SmoothingMode smoothingMode = g.SmoothingMode;
			InterpolationMode interpolationMode = g.InterpolationMode;

			g.CompositingQuality = CompositingQuality.HighQuality;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.High;
			
			g.DrawImage(shadow, padX + offset, padY + offset, sizeX, sizeY);
			g.DrawImage(img, padX, padY, sizeX, sizeY);

			g.CompositingQuality = compositingQuality;
			g.SmoothingMode = smoothingMode;
			g.InterpolationMode = interpolationMode;
		}

		/// <summary>
		/// Shadow generation
		/// </summary>
		void GenerateShadow() {
			shadow = null;
			if (img == null) {
				return;
			}
			
			shadow = new Bitmap(img.Width, img.Height);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(shadow)) {
				ImageAttributes attr = new ImageAttributes();
				attr.SetColorMatrix(new ColorMatrix(
					new float[][] {
						new float[] {0,  0,  0,  0, 0},        // red scaling factor of 0
						new float[] {0,  0,  0,  0, 0},        // green scaling factor of 0
						new float[] {0,  0,  0,  0, 0},        // blue scaling factor of 0
						new float[] {0,  0,  0,  0.5f, 0},        // alpha scaling factor of 1
						new float[] {0,  0,  0,  0, 1}
					}
				), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

				g.DrawImage(img,
					new Rectangle(0, 0, img.Width, img.Height),
					0, 0,
					img.Width,
					img.Height,
					GraphicsUnit.Pixel,
					attr
				);
			}
		}

	}
}
