using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Audio;
using Cubed.Audio.Decoders;
using Cubed.Core;
using OpenTK.Audio.OpenAL;

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
		
		
		
	}
}
