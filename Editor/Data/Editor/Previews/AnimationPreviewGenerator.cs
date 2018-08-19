using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Cubed.Data.Editor.Previews {
	
	/// <summary>
	/// Generator for images
	/// </summary>
	public class AnimationPreviewGenerator : PreviewGenerator {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path"></param>
		public AnimationPreviewGenerator(string path)
			: base(path) {

		}

		/// <summary>
		/// Making previews for basic images
		/// </summary>
		public override void Generate() {
			string ext = System.IO.Path.GetExtension(Path).ToLower();
			Image src = Image.FromFile(Path);
			if (src == null) {
				return;
			}
			switch (ext) {
				
				// GIF file
				case ".gif":
					AnimatedResult = ReadGIF(src);
					break;
				
			}

			// Cleanup
			src.Dispose();
			ShowSubIcon = false;
		}

		/// <summary>
		/// Reading GIF file
		/// </summary>
		/// <param name="src">Source image</param>
		/// <returns>Array of frames</returns>
		AnimationFrame[] ReadGIF(Image src) {
			List<AnimationFrame> frames = new List<AnimationFrame>();
			if (ImageAnimator.CanAnimate(src)) {
				FrameDimension frameDimension = new FrameDimension(src.FrameDimensionsList[0]);
				int frameCount = src.GetFrameCount(frameDimension);
				int delay = 0;
				for (int f = 0; f < frameCount; f++) {
					delay = BitConverter.ToInt32(src.GetPropertyItem(20736).Value, f * 4) * 10;
					src.SelectActiveFrame(frameDimension, f);
					frames.Add(new AnimationFrame() {
						Frame = ScaleForEditor((Image)src.Clone()),
						Delay = delay
					});
				}
			} else {
				frames.Add(new AnimationFrame() {
					Frame = ScaleForEditor((Image)src.Clone()),
					Delay = 1000
				});
			}
			return frames.ToArray();
		}

	}
}
