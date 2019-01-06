using Cubed.Components.Audio;
using Cubed.Core;
using Cubed.World;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Cubed.Audio.Decoders;
using System;

namespace Cubed.Audio {

	/// <summary>
	/// Internal audio system
	/// </summary>
	public class AudioSystem {

		/// <summary>
		/// All music volume
		/// </summary>
		public float MusicVolume {
			get {
				return musicVol;
			}
			set {
				musicVol = Math.Min(Math.Max(value, 0f), 1f);
			}
		}

		/// <summary>
		/// Sounds volume
		/// </summary>
		public float SoundsVolume {
			get {
				return soundVol;
			}
			set {
				soundVol = Math.Min(Math.Max(value, 0f), 1f);
			}
		}

		/// <summary>
		/// Audio cache
		/// </summary>
		internal AudioCache Cache {
			get;
			private set;
		}

		/// <summary>
		/// Background audio worker
		/// </summary>
		Thread audioThread;

		/// <summary>
		/// Current engine
		/// </summary>
		Engine engine;

		/// <summary>
		/// Current context
		/// </summary>
		AudioContext context;

		/// <summary>
		/// Internal EFX processor
		/// </summary>
		AudioEffects efx;

		/// <summary>
		/// All sources
		/// </summary>
		List<Source> sources;
		
		/// <summary>
		/// Queues for generating and removing sources
		/// </summary>
		ConcurrentQueue<Source> sourcesToCreate, sourcesToDestroy;

		/// <summary>
		/// Internal lock
		/// </summary>
		object _lock;

		/// <summary>
		/// Flag for destroying state
		/// </summary>
		bool destroying;

		/// <summary>
		/// Internal sound volume value
		/// </summary>
		float soundVol;

		/// <summary>
		/// Music volume value
		/// </summary>
		float musicVol;

		/// <summary>
		/// Check for OpenAL
		/// </summary>
		/// <returns>True if library exists</returns>
		public static bool OpenALExists() {
			try {
				AudioContext ac = new AudioContext();
				ac.MakeCurrent();
				ac.Dispose();
			} catch (DllNotFoundException) {
				return false;
			}
			return true;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="eng">Internal engine</param>
		internal AudioSystem(Engine eng) {
			engine = eng;
			sources = new List<Source>();
			sourcesToCreate = new ConcurrentQueue<Source>();
			sourcesToDestroy = new ConcurrentQueue<Source>();
			Cache = new AudioCache(eng);
			_lock = new object();

			soundVol = 1f;
			musicVol = 1f;

			// Creating thread
			audioThread = new Thread(ThreadedWork);
			audioThread.IsBackground = true;
			audioThread.Priority = ThreadPriority.Normal;
			audioThread.Start();
		}

		/// <summary>
		/// Updating audio in background
		/// </summary>
		void ThreadedWork() {

			// Initializing context
			string device = AudioContext.DefaultDevice;
			context = new AudioContext(device, 0, 0);
			context.MakeCurrent();
			efx = new AudioEffects(this);

			System.Diagnostics.Debug.WriteLine("[Engine] Running on AL " + AudioContext.DefaultDevice);
			
			// Endless loop
			while (!destroying) {

				// Setting up camera
				if (engine != null && engine.World != null && engine.World.Camera != null) {
					Camera cam = engine.World.Camera;
					Vector3 pos = cam.Position;
					Vector3 dir = cam.VectorToWorld(Vector3.UnitZ);
					Vector3 dup = cam.VectorToWorld(Vector3.UnitY);
					pos.Z *= -1;
					dir.Z *= -1;
					dup.Z *= -1;

					AL.Listener(ALListener3f.Position, ref pos);
					AL.Listener(ALListenerfv.Orientation, ref dir, ref dup);
				}

				// Updating effects
				efx.Update();

				// Generating all needed sources
				while (!sourcesToCreate.IsEmpty) {
					Source source = null;
					if (sourcesToCreate.TryDequeue(out source)) {
						source.Init();
					}
				}

				// Updating all the sources
				lock (_lock) {
					foreach (Source source in sources) {
						source.Update(efx);
					}
				}

				// Releasing needed sources
				while (!sourcesToDestroy.IsEmpty) {
					Source source = null;
					if (sourcesToDestroy.TryDequeue(out source)) {
						source.Release();
					}
				}

				// Sleeping
				Thread.Sleep(0);
			}

			// Releasing all sources
			lock (_lock) {
				foreach (Source source in sources) {
					source.Release();
				}
			}
		}

		/// <summary>
		/// Taking new source entry
		/// </summary>
		internal Source QuerySource() {
			Source source = new Source(this);
			lock (_lock) { 
				sources.Add(source);
			}
			sourcesToCreate.Enqueue(source);
			return source;
		}

		/// <summary>
		/// Releasing source entry
		/// </summary>
		/// <param name="source">Source</param>
		internal void ReleaseSource(Source source) {
			lock (_lock) {
				if (sources.Contains(source)) {
					sources.Remove(source);
				}
			}
			if (!sourcesToDestroy.Contains(source)) {
				sourcesToDestroy.Enqueue(source);
			}
		}

		/// <summary>
		/// Destroying system
		/// </summary>
		internal void Destroy() {
			// Waiting for main thread
			destroying = true;
			audioThread.Join();

			// Releasing cache
			Cache.Destroy();
			Cache = null;
		}

		/// <summary>
		/// Internal source
		/// </summary>
		internal class Source {

			/// <summary>
			/// Number of buffers for streaming
			/// </summary>
			const int STREAM_BUFFERS = 4;

			/// <summary>
			/// Parent audio system
			/// </summary>
			public AudioSystem Parent {
				get;
				private set;
			}

			/// <summary>
			/// Sound position
			/// </summary>
			public Vector3 Position {
				get {
					return position;
				}
				set {
					if (position != value) {
						dirtyPos = true;
					}
					position = value;
				}
			}

			/// <summary>
			/// Current playing track
			/// </summary>
			public AudioTrack Track {
				get {
					return track;
				}
				set {
					SetTrack(value);
				}
			}

			/// <summary>
			/// Flag for 2D sound
			/// </summary>
			public bool Disable3D {
				get {
					return no3D;
				}
				set {
					if (no3D != value) {
						dirty = true;
						dirtyPos = true;
					}
					no3D = value;
				}
			}

			/// <summary>
			/// Looping flag
			/// </summary>
			public bool Loop {
				get {
					return loop;
				}
				set {
					if (loop != value) {
						dirty = true;
					}
					loop = value;
				}
			}

			/// <summary>
			/// Volume
			/// </summary>
			public float Volume {
				get {
					return volume;
				}
				set {
					if (volume != value) {
						dirty = true;
					}
					volume = value;
				}
			}

			/// <summary>
			/// Playback speed
			/// </summary>
			public float Speed {
				get {
					return speed;
				}
				set {
					if (speed != value) {
						dirty = true;
					}
					speed = value;
				}
			}

			/// <summary>
			/// Sound channel mode
			/// </summary>
			public SoundChannels Channels {
				get {
					return channels;
				}
				set {
					if (channels != value) {
						channels = value;
						dirtyChannels = true;
					}
				}
			}

			/// <summary>
			/// Sound playback channel
			/// </summary>
			public SoundGroup Group {
				get;
				set;
			}

			/// <summary>
			/// Current track
			/// </summary>
			AudioTrack track;

			/// <summary>
			/// Flag for ready track
			/// </summary>
			bool trackReady;

			/// <summary>
			/// Flag for sound update
			/// </summary>
			bool dirty;

			/// <summary>
			/// Position
			/// </summary>
			bool dirtyPos;

			/// <summary>
			/// Flag for recheck audio transform
			/// </summary>
			bool dirtyChannels;

			/// <summary>
			/// Flag for streaming
			/// </summary>
			bool streaming;

			/// <summary>
			/// Current source position
			/// </summary>
			Vector3 position;

			/// <summary>
			/// Flag for no depth
			/// </summary>
			bool no3D;

			/// <summary>
			/// Looping flag
			/// </summary>
			bool loop;

			/// <summary>
			/// Source volume
			/// </summary>
			float volume;

			/// <summary>
			/// Source speed
			/// </summary>
			float speed;

			/// <summary>
			/// Swapping channels
			/// </summary>
			SoundChannels channels;

			/// <summary>
			/// Paused flag
			/// </summary>
			bool paused;

			/// <summary>
			/// Playing flag
			/// </summary>
			bool playing;

			/// <summary>
			/// Current state: 0-stopped, 1-paused, 2-playing
			/// </summary>
			int state;

			/// <summary>
			/// Internal OpenAL source
			/// </summary>
			int alSource;

			/// <summary>
			/// Hidden AL buffers
			/// </summary>
			int[] alBuffers;

			/// <summary>
			/// Previous data
			/// </summary>
			int prevData;

			/// <summary>
			/// Cleanup flag
			/// </summary>
			bool needCleanup;

			/// <summary>
			/// Custom decoder
			/// </summary>
			MP3Sharp.MP3Stream mp3Decoder;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="parent">Parental audio system</param>
			/// <param name="streaming">Generate streaming buffers</param>
			public Source(AudioSystem parent) {
				Parent = parent;

				// Default parameters
				loop = true;
				no3D = false;
				playing = false;
				paused = false;
				volume = 1;
				speed = 1;
			}

			/// <summary>
			/// Initializing sources
			/// </summary>
			public void Init() {
				dirty = true;
				dirtyPos = true;
				alSource = AL.GenSource();
				state = 0;
				CheckParameters();
			}
			
			/// <summary>
			/// Updating source logic
			/// </summary>
			public void Update(AudioEffects efx) {

				// Returning if source is not ready
				if (alSource == 0 || !AL.IsSource(alSource)) {
					return;
				}
				
				// Updating sound logics
				ALSourceState currentState = AL.GetSourceState(alSource);
				if (!trackReady) {
					if (currentState == ALSourceState.Playing || currentState == ALSourceState.Paused) {
						AL.SourceStop(alSource);
					}
					if (track == null || track.Entry == null) {
						AL.SourceStop(alSource);
						state = 0;
						trackReady = true;
					} else {
						CheckTrackReady();
					}
				} else {

					bool pl = playing;
					if (track == null || track.Entry == null) {
						pl = false;
					}

					if (pl && !paused) {
						if (state != 2) {
							AL.SourcePlay(alSource);
							state = 2;
						}

						// Updating effects
						efx.SourceUpdate(alSource);

					} else if(pl && paused) {
						if (state != 1) {
							AL.SourcePause(alSource);
							state = 1;
						}
					} else {
						if (state != 0) {
							AL.SourceStop(alSource);
							state = 0;
						}
					}

					// Update streaming
					if (streaming && pl && !paused) {
						if (prevData != -1 || loop) {
							int processed = 0;
							AL.GetSource(alSource, ALGetSourcei.BuffersProcessed, out processed);
							while (processed > 0) {
								processed--;
								int buffer = AL.SourceUnqueueBuffer(alSource);

								// Skipping loop
								if (prevData == -1 && !loop) {
									continue;
								}

								// Reading data
								AudioDecoder.AudioData audioData = GetNextData();

								// Populating buffer
								PopulateBuffer(buffer, audioData);
								AL.SourceQueueBuffer(alSource, buffer);
							}
							if (AL.GetSourceState(alSource) != ALSourceState.Playing) {
								AL.SourcePlay(alSource);
							}
						}
					}
				}

				// Checking parameters
				CheckParameters();

			}

			/// <summary>
			/// Releasing source
			/// </summary>
			public void Release() { 
				if (alSource != 0 && AL.IsSource(alSource)) {
					AL.SourceStop(alSource);
					AL.DeleteSource(alSource);
				}
				ReleaseBuffers();
			}

			/// <summary>
			/// Play sound
			/// </summary>
			public void Play() {
				if (!playing || paused) {
					playing = true;
					paused = false;
				}
			}

			/// <summary>
			/// Pause sound
			/// </summary>
			public void Pause() {
				if (playing && !paused) {
					paused = true;
				}
			}

			/// <summary>
			/// Stop sound
			/// </summary>
			public void Stop() {
				if (playing) {
					paused = false;
					playing = false;

					// Reassign track
					SetTrack(track, true);
				}
			}

			/// <summary>
			/// Applying params
			/// </summary>
			void CheckParameters() {
				float vol = volume;
				if (Group == SoundGroup.Ambient || Group == SoundGroup.Music) {
					vol *= Parent.musicVol;
				} else {
					vol *= Parent.soundVol;
				}
				AL.Source(alSource, ALSourcef.Gain, vol);
				if (dirtyPos) {
					Vector3 zero = Vector3.Zero;
					Vector3 pos = no3D ? Vector3.Zero : position;
					pos.Z *= -1f;
					AL.Source(alSource, ALSource3f.Position, ref pos);
					AL.Source(alSource, ALSource3f.Velocity, ref zero);
					dirtyPos = false;
				}
				if (dirty) {
					AL.Source(alSource, ALSourceb.SourceRelative, no3D);
					AL.Source(alSource, ALSourcef.Pitch, speed);
					AL.Source(alSource, ALSourceb.Looping, loop && !streaming);
					dirty = false;
				}
				if (dirtyChannels) {
					SetTrack(track, true);
					dirtyChannels = false;
				}
			}

			/// <summary>
			/// Setting current track
			/// </summary>
			void SetTrack(AudioTrack tr, bool force = false) {
				if (track != tr || force) {
					trackReady = false;
					streaming = false;
					track = tr;
					needCleanup = true;
				}
			}

			/// <summary>
			/// Releasing buffers
			/// </summary>
			void ReleaseBuffers() {
				if (alSource != 0) {
					int queued = 0;
					AL.GetSource(alSource, ALGetSourcei.BuffersQueued, out queued);
					if (queued > 0) {
						AL.SourceUnqueueBuffers(alSource, queued);
					}
				}
				if (alBuffers != null) {
					foreach (int buffer in alBuffers) {
						if (buffer != 0 && AL.IsBuffer(buffer)) {
							AL.DeleteBuffer(buffer);
						}
					}
					alBuffers = null;
				}
				if (mp3Decoder != null) {
					mp3Decoder = null;
				}
			}

			/// <summary>
			/// Checking track for completion
			/// </summary>
			void CheckTrackReady() {
				if (needCleanup) {
					AL.Source(alSource, ALSourceb.Looping, false);
					AL.SourceStop(alSource);
					ReleaseBuffers();
					needCleanup = false;
				}
				if (track != null && track.Entry != null) {
					if (track.Entry.State == AudioCache.EntryState.Complete || track.Entry.State == AudioCache.EntryState.Streaming) {
						streaming = track.Entry.State == AudioCache.EntryState.Streaming;
						alBuffers = AL.GenBuffers(!streaming ? 1 : STREAM_BUFFERS);

						if (streaming) {

							// Picking decoder
							if (track.Entry.Decoder is Mp3Decoder) {
								mp3Decoder = (track.Entry.Decoder as Mp3Decoder).ObtainCustomDecoder();
							}

							// Disable looping
							dirty = true;
							CheckParameters();

							// Creating ring buffer
							prevData = -1;
							for (int i = 0; i < STREAM_BUFFERS; i++) {
								AudioDecoder.AudioData ad = GetNextData();
								PopulateBuffer(alBuffers[i], ad);
								AL.SourceQueueBuffer(alSource, alBuffers[i]);

								if (prevData == -1) {
									break;
								}
							}

						} else {

							// Enable looping
							dirty = true;
							CheckParameters();

							// Instant loading to single buffer
							prevData = -1;
							PopulateBuffer(alBuffers[0], GetNextData());
							AL.Source(alSource, ALSourcei.Buffer, alBuffers[0]);

						}
						state = paused ? 1 : 0;
						if (playing && !paused) {
							dirty = true;
							CheckParameters();
							AL.SourcePlay(alSource);
							state = 2;
						}
						trackReady = true;
					}
				}
			}

			/// <summary>
			/// Sending data to a buffer
			/// </summary>
			/// <param name="buffer">Buffer to populate</param>
			/// <param name="data">Data</param>
			void PopulateBuffer(int buffer, AudioDecoder.AudioData data) {

				// Processing data
				int depth = data.Depth;
				int chnum = data.Channels;
				byte[] sdata = data.Data;
				if (data.Channels == 2 && channels != SoundChannels.Auto) {
					if (channels == SoundChannels.Swap) {
						byte[] ndata = new byte[sdata.Length];
						int pos = 0;
						while (pos < ndata.Length) {
							if (depth == 16) {
								ndata[pos + 0] = sdata[pos + 2];
								ndata[pos + 1] = sdata[pos + 3];
								ndata[pos + 2] = sdata[pos + 0];
								ndata[pos + 3] = sdata[pos + 1];
							} else {
								ndata[pos + 0] = sdata[pos + 1];
								ndata[pos + 1] = sdata[pos + 0];
							}
							pos += depth / 4;
						}
						sdata = ndata;
					} else if(channels == SoundChannels.MixedMono) {
						byte[] ndata = new byte[sdata.Length / 2];
						float[] leftData = new float[ndata.Length / (depth / 8)];
						float[] rightData = new float[leftData.Length];
						if (depth == 16) {
							for (int i = 0; i < sdata.Length; i += 4) {
								int pos = i / 4;
								leftData[pos] = (float)BitConverter.ToInt16(sdata, i) / 32768f + 0.5f;
								rightData[pos] = (float)BitConverter.ToInt16(sdata, i + 2) / 32768f + 0.5f;
								leftData[pos] = Math.Min(Math.Max(leftData[pos], 0), 1);
								rightData[pos] = Math.Min(Math.Max(rightData[pos], 0), 1);
							}
						} else {
							for (int i = 0; i < sdata.Length; i += 2) {
								int pos = i / 2;
								leftData[pos] = (float)sdata[i + 0] / 255f;
								rightData[pos] = (float)sdata[i + 1] / 255f;
							}
						}
						for (int i = 0; i < leftData.Length; i++) {
							float a = leftData[i];
							float b = rightData[i];
							float mix = 0;
							if (a < 0.5f || b < 0.5f) {
								mix = a * b * 2f;
							} else {
								mix = 2f * (a + b) - a * b * 2f - 1f;
							}
							if (depth == 16) {
								int mx = (int)(mix * 65535f) - 32768;
								int ps = i * 2;
								ndata[ps + 0] = (byte)((mx) & 0xff);
								ndata[ps + 1] = (byte)((mx >> 8) & 0xff);
							} else {
								ndata[i] = (byte)(mix * 255f);
							}
						}
						sdata = ndata;
						chnum = 1;
					} else {
						byte[] ndata = new byte[sdata.Length / 2];
						int pos = channels == SoundChannels.RightMono ? depth / 8 : 0;
						int npos = 0;
						while (npos < ndata.Length) {
							if (depth == 16) {
								ndata[npos + 0] = sdata[pos + 0];
								ndata[npos + 1] = sdata[pos + 1];
							} else {
								ndata[npos] = sdata[pos];
							}
							pos += depth / 4;
							npos += depth / 8;
						}
						sdata = ndata;
						chnum = 1;
					}
				}

				// Sending to AL
				ALFormat format = depth == 16 ? ALFormat.Mono16 : ALFormat.Mono8;
				if (chnum == 2) {
					format = depth == 16 ? ALFormat.Stereo16 : ALFormat.Stereo8;
				}
				AL.BufferData<byte>(buffer, format, sdata, sdata.Length, data.Samples);
			}

			/// <summary>
			/// Getting new data
			/// </summary>
			/// <returns>Decoded data</returns>
			AudioDecoder.AudioData GetNextData() {
				AudioDecoder.AudioData data = null;
				if (mp3Decoder != null) {
					data = (track.Entry.Decoder as Mp3Decoder).ReadWithCustomStream(mp3Decoder, prevData, false);
				} else {
					data = track.Entry.GetData(prevData);
				}
				prevData = data.NextData;
				return data;
			}
		}

		/// <summary>
		/// Sound channel
		/// </summary>
		public enum SoundGroup {

			/// <summary>
			/// Music channel
			/// </summary>
			Music,

			/// <summary>
			/// Ambient sounds
			/// </summary>
			Ambient,

			/// <summary>
			/// Map generic sounds
			/// </summary>
			MapSound,

			/// <summary>
			/// Player sounds
			/// </summary>
			Player,

			/// <summary>
			/// Enemy sounds
			/// </summary>
			Enemy,

			/// <summary>
			/// Effect sounds
			/// </summary>
			Effect,

			/// <summary>
			/// Gunshots
			/// </summary>
			Weapon,

			/// <summary>
			/// Interface
			/// </summary>
			UI

		}
		
		/// <summary>
		/// Sound channel mode
		/// </summary>
		public enum SoundChannels : int {

			/// <summary>
			/// Automatic
			/// </summary>
			Auto		= 0,

			/// <summary>
			/// Force left channel mono
			/// </summary>
			LeftMono	= 1,

			/// <summary>
			/// Force right channel mono
			/// </summary>
			RightMono	= 2,

			/// <summary>
			/// Force mixed down mono
			/// </summary>
			MixedMono	= 3,

			/// <summary>
			/// Swapping channels
			/// </summary>
			Swap		= 4
		}
	}
}
