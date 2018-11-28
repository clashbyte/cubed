using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Audio;
using Cubed.Components.Audio;
using Cubed.Data.Game.Attributes;
using Cubed.Graphics;
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
		/// Updating entity
		/// </summary>
		public override void Update() {
			//light.TextureAngle += 5f;
		}

		/// <summary>
		/// Saving to file
		/// </summary>
		/// <param name="f">Binary writer</param>
		public override void Save(System.IO.BinaryWriter f) {
			base.Save(f);

			// Writing version
			f.Write((byte)0);

			/*
			// Color
			f.Write(Color.R);
			f.Write(Color.G);
			f.Write(Color.B);

			// Range
			f.Write(Range);

			// Shadows
			f.Write(Shadows);

			// Textures
			f.Write(Texture != null);
			if (Texture != null) {
				f.Write(Texture.Link);
			}
			f.Write(TextureAngle);
			*/

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

			/*
			// Reading ver-1 specific data
			if (ver >= 1) {

				// Basic params
				byte[] colors = f.ReadBytes(3);
				Color = System.Drawing.Color.FromArgb(colors[0], colors[1], colors[2]);
				Range = f.ReadSingle();
				Shadows = f.ReadBoolean();

				// Reading texture
				if (f.ReadBoolean()) {
					Texture = new Texture(f.ReadString());
				}
				TextureAngle = f.ReadSingle();

			}*/


		}
		
	}
}
