using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Audio.Decoders;
using Cubed.Core;

namespace Cubed.Audio {

	/// <summary>
	/// Sound file
	/// </summary>
	public class AudioTrack {

		/// <summary>
		/// File path
		/// </summary>
		public string Path {
			get;
			private set;
		}

		/// <summary>
		/// Sound loaded in instant mode
		/// </summary>
		public bool Instant {
			get;
			private set;
		}
		
		/// <summary>
		/// Cache entry
		/// </summary>
		internal AudioCache.CacheEntry Entry {
			get;
			private set;
		}

		/// <summary>
		/// Loading audio track
		/// </summary>
		/// <param name="path">File name</param>
		/// <param name="instant">Force instant loading</param>
		public AudioTrack(string path, bool instant = false) {
			Path = path;
			Instant = instant;
			Entry = Engine.Current.AudioSystem.Cache.Get(path, instant);
		}
		
	}
}
