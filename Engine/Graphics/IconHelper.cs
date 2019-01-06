using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Cubed.Graphics {

	/// <summary>
	/// Icon helper class
	/// </summary>
	public static class IconHelper {

		/// <summary>
		/// Creating icon from image
		/// </summary>
		/// <param name="image">Image</param>
		/// <returns>Resulting icon</returns>
		public static Icon FromImage(Image image) {

			// Processing image
			var widthScale = 64f / (float)image.Width;
			var heightScale = 64f / (float)image.Height;
			var scale = Math.Min(widthScale, heightScale);
			Size scaleSize = new Size(
				(int)Math.Round((image.Width * scale)),
				(int)Math.Round((image.Height * scale))
			);

			// Creating new image
			Image newImage = new Bitmap(scaleSize.Width, scaleSize.Height);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImage)) {
				g.DrawImage(image, 0, 0, scaleSize.Width, scaleSize.Height);
			}
			image = newImage;

			// Icon data
			MemoryStream iconData = new MemoryStream();
			using (MemoryStream imgStream = new MemoryStream()) {
				image.Save(imgStream, ImageFormat.Png);

				// Writing data to icon file
				BinaryWriter f = new BinaryWriter(iconData);
				f.Write((short)0);	// Reserved two bytes
				f.Write((short)1);  // Image type - icon
				f.Write((short)1);  // Number of images

				// Image entry
				f.Write((byte)image.Width);		// Width
				f.Write((byte)image.Height);	// Height
				f.Write((short)0);              // Num of colors and reserved
				f.Write((short)0);              // Color planes
				f.Write((short)32);             // BPP
				f.Write((int)imgStream.Length);	// Data size
				f.Write((int)6 + 16);			// Offset
				f.Write(imgStream.ToArray());   // Raw PNG data
				f.Flush();
			}

			// Reading icon data
			iconData.Position = 0;
			return new Icon(iconData);
		}
	}
}
