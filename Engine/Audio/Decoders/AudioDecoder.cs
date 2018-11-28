using Cubed.Core;

namespace Cubed.Audio.Decoders {

	/// <summary>
	/// Base for all audio decoders
	/// </summary>
	internal abstract class AudioDecoder {

		/// <summary>
		/// Decoding file
		/// </summary>
		public string File {
			get {
				return file;
			}
			set {
				file = value;
				Setup();
			}
		}

		/// <summary>
		/// Overriding current engine
		/// </summary>
		public Engine EngineOverride {
			get;
			set;
		}

		/// <summary>
		/// Allow caching
		/// </summary>
		public virtual bool CachingAllowed {
			get {
				return true;
			}
		}

		/// <summary>
		/// Internal file link
		/// </summary>
		string file;

		/// <summary>
		/// Empty decoder
		/// </summary>
		public AudioDecoder() {
			
		}

		/// <summary>
		/// Setting up decoding
		/// </summary>
		protected abstract void Setup();

		/// <summary>
		/// Reading data
		/// </summary>
		/// <returns>Audio data</returns>
		public abstract AudioData Read(int nextData = -1, bool readAll = false);

		/// <summary>
		/// Check if sound file needs to be loaded one-shot
		/// </summary>
		/// <param name="file">File path</param>
		/// <returns>True for instant load</returns>
		public virtual bool NeedInstantLoad(string file) {
			return GetEngine().Filesystem.Size(file) < 512000;
		}

		/// <summary>
		/// Getting overriden engine
		/// </summary>
		/// <returns>Engine instance</returns>
		protected Engine GetEngine() {
			if (EngineOverride != null) {
				return EngineOverride;
			}
			return Engine.Current;
		}

		/// <summary>
		/// Structure for audio data
		/// </summary>
		public class AudioData {
			public byte[] Data;
			public int Samples;
			public int Depth;
			public int Channels;
			public int NextData;
		}
	}
}
