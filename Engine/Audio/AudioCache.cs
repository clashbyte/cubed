using Cubed.Audio.Decoders;
using Cubed.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cubed.Audio {

	/// <summary>
	/// Internal audio cache
	/// </summary>
	internal class AudioCache {
		
		/// <summary>
		/// File reading threads
		/// </summary>
		const int LOADING_THREADS = 2;
		
		/// <summary>
		/// Cached sounds
		/// </summary>
		ConcurrentDictionary<string, CacheEntry> sounds = new ConcurrentDictionary<string, CacheEntry>();

		/// <summary>
		/// Sounds for reading from disk
		/// </summary>
		ConcurrentQueue<CacheEntry> loadQueue = new ConcurrentQueue<CacheEntry>();

		/// <summary>
		/// Background loading threads
		/// </summary>
		Thread[] loadingThreads;

		/// <summary>
		/// Current engine
		/// </summary>
		Engine engine;

		/// <summary>
		/// Audio cache constructor
		/// </summary>
		public AudioCache(Engine eng) {
			engine = eng;
		}

		/// <summary>
		/// Loading single sound
		/// </summary>
		internal CacheEntry Get(string name, bool instant) {
			CheckThreads();
			name = name.ToLower();
			if (!sounds.ContainsKey(name)) {
				CacheEntry en = new CacheEntry(name);
				if (instant) {
					en.LoadNow(engine);
				} else {
					en.AddToLoadingQueue(engine);
				}
				sounds.TryAdd(name, en);
			}
			return sounds[name];
		}

		/// <summary>
		/// Thread checking
		/// </summary>
		void CheckThreads() {
			if (loadingThreads == null) {
				loadingThreads = new Thread[LOADING_THREADS];
				for (int i = 0; i < LOADING_THREADS; i++) {
					Thread t = new Thread(ThreadedLoading);
					t.Priority = ThreadPriority.BelowNormal;
					t.IsBackground = true;
					t.Start();
				}
			}
		}

		/// <summary>
		/// Threaded texture background loading job
		/// </summary>
		void ThreadedLoading() {
			while (true) {
				if (!loadQueue.IsEmpty) {
					CacheEntry t = null;
					if (loadQueue.TryDequeue(out t)) {
						t.Read(engine);
					}
					Thread.Sleep(1);
				} else {
					Thread.Sleep(50);
				}
			}
		}

		/// <summary>
		/// Loaded sound entry
		/// </summary>
		public class CacheEntry {
			
			/// <summary>
			/// Path to sound
			/// </summary>
			public string FileName {
				get;
				private set;
			}

			/// <summary>
			/// Current state
			/// </summary>
			public EntryState State {
				get;
				private set;
			}

			/// <summary>
			/// Internal decoder
			/// </summary>
			internal AudioDecoder Decoder {
				get;
				private set;
			}

			/// <summary>
			/// Fully loaded data
			/// </summary>
			AudioDecoder.AudioData fullData;

			/// <summary>
			/// Internal threading lock
			/// </summary>
			object _lock;

			/// <summary>
			/// Entry creation
			/// </summary>
			public CacheEntry(string file) {
				FileName = file;
				State = EntryState.Empty;
				_lock = new object();
			}

			/// <summary>
			/// Instant texture loading
			/// </summary>
			public void LoadNow(Engine eng) {
				Read(eng);
			}

			/// <summary>
			/// Adding to queue
			/// </summary>
			public void AddToLoadingQueue(Engine eng) {
				if (State != EntryState.Loading && !eng.AudioSystem.Cache.loadQueue.Contains(this)) {
					eng.AudioSystem.Cache.loadQueue.Enqueue(this);
					eng.AudioSystem.Cache.CheckThreads();
				}
			}

			/// <summary>
			/// Reading sound
			/// </summary>
			/// <param name="engine">Current engine</param>
			internal void Read(Engine engine) {
				State = EntryState.Loading;
				if (engine.Filesystem.Exists(FileName)) {
					Decoder = PickDecoder(FileName);
					Decoder.EngineOverride = engine;
					lock(_lock) {
						Decoder.File = FileName;
					}

					bool fullLoad = Decoder.NeedInstantLoad(FileName);
					if (fullLoad) {

						// Reading full file
						lock(_lock) {
							fullData = Decoder.Read(-1, true);
						}
						State = EntryState.Complete;

					} else {

						// File is streamable
						State = EntryState.Streaming;

					}
				} else {
					State = EntryState.Empty;
				}
			}

			/// <summary>
			/// Decoding single frame
			/// </summary>
			/// <param name="pointer">Pointer for next data</param>
			/// <returns>Data or null</returns>
			internal AudioDecoder.AudioData GetData(int pointer = -1) {
				if (fullData != null) {
					return fullData;
				}
				if (Decoder != null) {
					AudioDecoder.AudioData data = null;
					lock (_lock) {
						data = Decoder.Read(pointer);
					}
					return data;
				}
				return null;
			}

			/// <summary>
			/// Decoding MP3-specific frame
			/// </summary>
			/// <param name="stream">Custom MP3 stream</param>
			/// <param name="pointer">Data pointer</param>
			/// <returns>Data or null</returns>
			internal AudioDecoder.AudioData GetMP3StreamedData(MP3Sharp.MP3Stream stream, int pointer = -1) {
				if (Decoder != null && Decoder is Mp3Decoder) {
					AudioDecoder.AudioData data = null;
					lock (_lock) {
						data = (Decoder as Mp3Decoder).ReadWithCustomStream(stream, pointer, false);
					}
					return data;
				}
				return null;
			}

		}

		/// <summary>
		/// Getting decoder for audio type
		/// </summary>
		/// <returns>Decoder</returns>
		static AudioDecoder PickDecoder(string file) {
			string ext = System.IO.Path.GetExtension(file).ToLower();
			switch (ext) {

				// WAV files
				case ".wav":
					return new WavDecoder();

				// MP3 files
				case ".mp3":
					return new Mp3Decoder();

				// OGG files
				case ".ogg":
					return new OggDecoder();

				// MID files
				case ".mid":
					return new MidiDecoder();

				// No suitable decoder
				default:
					throw new Exception("Unknown sound format: " + file);
			}
		}

		/// <summary>
		/// Current entry status
		/// </summary>
		public enum EntryState {
			Empty,
			Loading,
			Complete,
			Streaming
		}
	}
}
