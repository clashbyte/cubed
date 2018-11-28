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

namespace Cubed.Audio {

	/// <summary>
	/// Internal audio system
	/// </summary>
	public class AudioSystem {

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
			while (true) {

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
			/// Paused flag
			/// </summary>
			bool paused;

			/// <summary>
			/// Playing flag
			/// </summary>
			bool playing;

			/// <summary>
			/// Internal OpenAL source
			/// </summary>
			int alSource;

			/// <summary>
			/// Hidden AL buffers
			/// </summary>
			int[] alBuffers;

			/// <summary>
			/// Current buffer index
			/// </summary>
			int bufferPointer;

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
						if (currentState != ALSourceState.Playing) {
							AL.SourcePlay(alSource);
						}

						// Updating effects
						efx.SourceUpdate(alSource);

					} else if(pl && paused) {
						if (currentState != ALSourceState.Paused) {
							AL.SourcePause(alSource);
						}
					} else {
						if (currentState != ALSourceState.Stopped) {
							AL.SourceStop(alSource);
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
					AL.Source(alSource, ALSourcef.Gain, speed);
					AL.Source(alSource, ALSourcef.Pitch, 1);
					AL.Source(alSource, ALSourceb.Looping, loop && !streaming);
					dirty = false;
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
							AL.Source(alSource, ALSourceb.Looping, false);

							// Creating ring buffer
							prevData = -1;
							for (int i = 0; i < STREAM_BUFFERS; i++) {
								AudioDecoder.AudioData ad = GetNextData();
								prevData = ad.NextData;
								PopulateBuffer(alBuffers[i], ad);
								AL.SourceQueueBuffer(alSource, alBuffers[i]);

								if (prevData == -1) {
									break;
								}
							}

						} else {

							// Enable looping
							AL.Source(alSource, ALSourceb.Looping, loop);

							// Instant loading to single buffer
							prevData = -1;
							PopulateBuffer(alBuffers[0], GetNextData());
							AL.Source(alSource, ALSourcei.Buffer, alBuffers[0]);

						}
						if (playing && !paused) {
							CheckParameters();
							AL.SourcePlay(alSource);
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
				ALFormat format = data.Depth == 16 ? ALFormat.Mono16 : ALFormat.Mono8;
				if (data.Channels == 2) {
					format = data.Depth == 16 ? ALFormat.Stereo16 : ALFormat.Stereo8;
				}
				AL.BufferData<byte>(buffer, format, data.Data, data.Data.Length, data.Samples);
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
	}
}
