using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Drivers.Files;

namespace Cubed.Drivers.Textures {
	
	/// <summary>
	/// Texture decoder
	/// </summary>
	internal abstract class TextureLoader {

		/// <summary>
		/// Image decoders
		/// </summary>
		static Dictionary<string, Type> loaders = new Dictionary<string, Type>() {
			{".png", typeof(BasicImageLoader)},
			{".jpg", typeof(BasicImageLoader)},
			{".jpeg", typeof(BasicImageLoader)},
			{".bmp", typeof(BasicImageLoader)},
			{".gif", typeof(GifImageLoader)},
		};

		/// <summary>
		/// Decoding file
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static Scan[] Load(string file, FileSystem fileSystem) {
			string ext = System.IO.Path.GetExtension(file).ToLower();
			if (loaders.ContainsKey(ext)) {
				Type t = loaders[ext];
				TextureLoader ld = Activator.CreateInstance(t) as TextureLoader;
				if (ld != null) {
					return ld.Decode(file, fileSystem);
				}
			}
			return new Scan[0];
		}

		/// <summary>
		/// Decoding image
		/// </summary>
		/// <param name="file">File path</param>
		/// <returns></returns>
		protected abstract Scan[] Decode(string file, FileSystem fileSystem);

		/// <summary>
		/// Single texture scan
		/// </summary>
		public class Scan {
			public Image Texture;
			public int Delay;
		}
	}
}
