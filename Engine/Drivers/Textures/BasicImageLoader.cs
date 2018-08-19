using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.Core;
using Cubed.Drivers.Files;

namespace Cubed.Drivers.Textures {

	/// <summary>
	/// Basic decoder for PNG, JPG and BMP
	/// </summary>
	internal class BasicImageLoader : TextureLoader {

		/// <summary>
		/// Decode image
		/// </summary>
		/// <param name="file">File path</param>
		/// <returns>Array of scans</returns>
		protected override Scan[] Decode(string file, FileSystem fileSystem) {
			if (fileSystem.Exists(file)) {
				Image img = Image.FromStream(new MemoryStream(fileSystem.Get(file)));
				if (img != null) {
					return new Scan[] {
						new Scan() {
							Texture = img,
							Delay = int.MaxValue
						}
					};
				}
			}
			return new Scan[0];
		}
	}
}
