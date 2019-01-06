using Cubed.Audio;
using Cubed.Core;

namespace Cubed.Components.Audio {

	/// <summary>
	/// Sound emitting component
	/// </summary>
	public class SoundSource : EntityComponent, ILateUpdatable {

		/// <summary>
		/// Audio track to play
		/// </summary>
        public AudioTrack Track {
            get {
                return source.Track;
            }
            set {
				source.Track = value;
            }
        }

		/// <summary>
		/// Source volume
		/// </summary>
		public float Volume {
			get {
				return source.Volume;
			}
			set {
				source.Volume = value;
			}
		}

		/// <summary>
		/// Source playing speed
		/// </summary>
		public float Pitch {
			get {
				return source.Speed;
			}
			set {
				source.Speed = value;
			}
		}

		/// <summary>
		/// Source group
		/// </summary>
		public AudioSystem.SoundGroup Group {
			get {
				return source.Group;
			}
			set {
				source.Group = value;
			}
		}

		/// <summary>
		/// Sound looping
		/// </summary>
		public bool Loop {
			get {
				return source.Loop;
			}
			set {
				source.Loop = value;
			}
		}

		/// <summary>
		/// Disabling 3D mode
		/// </summary>
		public bool Disable3D {
			get {
				return source.Disable3D;
			}
			set {
				source.Disable3D = value;
			}
		}

		/// <summary>
		/// Channel processing mode
		/// </summary>
		public AudioSystem.SoundChannels ChannelMod {
			get {
				return source.Channels;
			}
			set {
				source.Channels = value;
				
			}
		}
		
		/// <summary>
		/// Hidden source
		/// </summary>
		AudioSystem.Source source;
		
		/// <summary>
		/// Track constructor
		/// </summary>
		/// <param name="track">Track file</param>
		public SoundSource(AudioTrack track) {
			
			// Creating source
			source = Engine.Current.AudioSystem.QuerySource();
			if (track != null) {
				source.Track = track;
			}
		}

		/// <summary>
		/// Current updating
		/// </summary>
		public void LateUpdate() {
			if (!source.Disable3D && Parent != null) {
				source.Position = Parent.Position;
			}
		}
		
		/// <summary>
		/// Play sound
		/// </summary>
		public void Play() {
			source.Play();
		}

		/// <summary>
		/// Pause sound
		/// </summary>
		public void Pause() {
			source.Pause();
		}

		/// <summary>
		/// Stop sound
		/// </summary>
		public void Stop() {
			source.Stop();
		}

		/// <summary>
		/// Removing channel
		/// </summary>
		internal override void Destroy() {
			base.Destroy();
			source.Parent.ReleaseSource(source);
			source = null;
		}

	}
}
