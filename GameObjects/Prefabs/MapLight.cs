using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Data.Game.Attributes;
using Cubed.Graphics;
using Cubed.World;

namespace Cubed.Prefabs {

	/// <summary>
	/// Map light entity
	/// </summary>
	[Prefab(2)]
	public class MapLight : GamePrefab {

		/// <summary>
		/// Light color
		/// </summary>
		public Color Color {
			get {
				return light.Color;
			}
			set {
				light.Color = Color.FromArgb(value.R, value.G, value.B);
			}
		}
		
		/// <summary>
		/// Light range
		/// </summary>
		public float Range {
			get {
				return light.Range;
			}
			set {
				light.Range = Math.Max(value, 0.1f);
			}
		}

		/// <summary>
		/// Light texture
		/// </summary>
		public Texture Texture {
			get {
				return light.Texture;
			}
			set {
				light.Texture = value;
			}
		}

		/// <summary>
		/// Texture angle
		/// </summary>
		public float TextureAngle {
			get {
				return light.TextureAngle;
			}
			set {
				light.TextureAngle = value;
			}
		}

		/// <summary>
		/// Shadows
		/// </summary>
		public bool Shadows {
			get {
				return light.Shadows;
			}
			set {
				light.Shadows = value;
			}
		}

		/// <summary>
		/// Light entity
		/// </summary>
		Light light;

		/// <summary>
		/// Constructor
		/// </summary>
		public MapLight() {
			light = new Light() {
				Color = Color.White,
				Range = 5
			};
			light.Parent = this;
		}

		/// <summary>
		/// Assigning to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Assign(Scene scene) {
			scene.Entities.Add(this);
			scene.Entities.Add(light);
		}

		/// <summary>
		/// Unassigning from scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Unassign(Scene scene) {
			scene.Entities.Remove(this);
			scene.Entities.Remove(light);
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
			f.Write((byte)1);

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

			}


		}
		
	}
}
