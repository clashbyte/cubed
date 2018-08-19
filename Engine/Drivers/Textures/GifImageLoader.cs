using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.Core;
using Cubed.Drivers.Files;

namespace Cubed.Drivers.Textures {

	/// <summary>
	/// Basic decoder for GIF
	/// </summary>
	internal class GifImageLoader : TextureLoader {

		/// <summary>
		/// Decode image
		/// </summary>
		/// <param name="file">File path</param>
		/// <returns>Array of scans</returns>
		protected override Scan[] Decode(string file, FileSystem fileSystem) {
			if (fileSystem.Exists(file)) {
				Image img = Image.FromStream(new MemoryStream(fileSystem.Get(file)));
				if (img != null) {
					List<Scan> frames = new List<Scan>();
					if (ImageAnimator.CanAnimate(img)) {
						FrameDimension frameDimension = new FrameDimension(img.FrameDimensionsList[0]);
						int frameCount = img.GetFrameCount(frameDimension);
						int delay = 0;
						for (int f = 0; f < frameCount; f++) {
							delay = BitConverter.ToInt32(img.GetPropertyItem(20736).Value, f * 4) * 10;
							img.SelectActiveFrame(frameDimension, f);
							frames.Add(new Scan() {
								Texture = (Image)img.Clone(),
								Delay = delay
							});
						}
					} else {
						frames.Add(new Scan() {
							Texture = (Image)img.Clone(),
							Delay = int.MaxValue
						});
					}
					return frames.ToArray();
				}
			}
			return new Scan[0];
		}
	}
}
