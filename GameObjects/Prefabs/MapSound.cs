using System;
using Cubed.Audio;
using Cubed.Components.Audio;
using Cubed.Core;
using Cubed.Data.Game.Attributes;
using Cubed.World;

namespace Cubed.Prefabs {

	/// <summary>
	/// Sound source
	/// </summary>
	[Prefab(4)]
	public class MapSound : GamePrefab {

		/// <summary>
		/// Specified sound track
		/// </summary>
		public AudioTrack Sound {
			get {
				return source.Track;
			}
			set {
				source.Track = value;
			}
		}

		/// <summary>
		/// Volume
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
		/// Sound speed
		/// </summary>
		public float Speed {
			get {
				return source.Pitch;
			}
			set {
				source.Pitch = value;
			}
		}

		/// <summary>
		/// Channel modification
		/// </summary>
		public AudioSystem.SoundChannels Channel {
			get {
				return source.ChannelMod;
			}
			set {
				source.ChannelMod = value;
			}
		}

		/// <summary>
		/// Source entity
		/// </summary>
		Entity entity;

		/// <summary>
		/// Internal sound source
		/// </summary>
		SoundSource source;

		/// <summary>
		/// Constructor
		/// </summary>
		public MapSound() {
			entity = new Entity();
			entity.AddComponent(source = new SoundSource(null));
			entity.Parent = this;
			entity.LocalPosition = OpenTK.Vector3.Zero;
		}

		/// <summary>
		/// Assigning to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Assign(Scene scene) {
			scene.Entities.Add(this);
			scene.Entities.Add(entity);
			source.Play();
		}

		/// <summary>
		/// Unassigning from scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Unassign(Scene scene) {
			scene.Entities.Remove(this);
			scene.Entities.Remove(entity);
			source.Stop();
		}

		/// <summary>
		/// Saving to file
		/// </summary>
		/// <param name="f">Binary writer</param>
		public override void Save(System.IO.BinaryWriter f) {
			base.Save(f);

			// Writing version
			f.Write((byte)1);

			// Link to file
			if (Sound != null) {
				f.Write(Sound.Path);
			} else {
				f.Write("");
			}

			// Params
			f.Write(Volume);
			f.Write(Speed);
			f.Write((byte)Channel);
		}

		/// <summary>
		/// Reading from file
		/// </summary>
		/// <param name="f"></param>
		public override void Load(System.IO.BinaryReader f) {
			base.Load(f);

			// Reading version
			int ver = 0;
			try {
				ver = f.ReadByte();
			} catch (Exception ex) { }

			// Loading file
			if (ver > 0) {
				string path = f.ReadString();
				if (Engine.Current.Filesystem.Exists(path)) {
					Sound = new AudioTrack(path);
				}

				// Switching data
				Volume = f.ReadSingle();
				Speed = f.ReadSingle();
				Channel = (AudioSystem.SoundChannels)f.ReadByte();
			}
		}
		
	}
}
