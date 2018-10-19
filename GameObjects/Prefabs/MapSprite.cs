using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using Cubed.Data.Game.Attributes;
using Cubed.Graphics;
using Cubed.World;
using OpenTK;

namespace Cubed.Prefabs {

	/// <summary>
	/// Sprite on map
	/// </summary>
	[Prefab(1)]
	public class MapSprite : GamePrefab {

		/// <summary>
		/// Core texture
		/// </summary>
		public Texture Texture {
			get {
				return sprite.Texture;
			}
			set {
				sprite.Texture = value;
				
			}
		}

		/// <summary>
		/// Affecting by light attribute
		/// </summary>
		public bool AffectedByLight {
			get {
				return sprite.AffectedByLight;
			}
			set {
				sprite.AffectedByLight = value;
			}
		}

		/// <summary>
		/// Affecting by fog attribute
		/// </summary>
		public bool AffectedByFog {
			get {
				return sprite.AffectedByFog;
			}
			set {
				sprite.AffectedByFog = value;
			}
		}

		/// <summary>
		/// Tint color
		/// </summary>
		public Color Tint {
			get {
				return sprite.Tint;
			}
			set {
				sprite.Tint = value;
			}
		}

		/// <summary>
		/// Texture offset
		/// </summary>
		public Vector2 Offset {
			get {
				return sprite.Offset;
			}
			set {
				sprite.Offset = value;
			}
		}

		/// <summary>
		/// Texture scale
		/// </summary>
		public Vector2 Scale {
			get {
				return sprite.Scale;
			}
			set {
				sprite.Scale = value;
			}
		}

		/// <summary>
		/// Collision flag
		/// </summary>
		public bool Solid {
			get {
				return boundEnabled;
			}
			set {
				boundEnabled = value;
				UpdateBounds();
			}
		}

		/// <summary>
		/// Bounds offset
		/// </summary>
		public Vector3 BoundOffset {
			get {
				return boundPosition;
			}
			set {
				boundPosition = value;
				UpdateBounds();
			}
		}

		/// <summary>
		/// Bounds offset
		/// </summary>
		public Vector3 BoundSize {
			get {
				return boundSize;
			}
			set {
				boundSize = value;
				UpdateBounds();
			}
		}

		/// <summary>
		/// Enabled bounding box
		/// </summary>
		bool boundEnabled;

		/// <summary>
		/// Bounding box position
		/// </summary>
		Vector3 boundPosition = Vector3.Zero;

		/// <summary>
		/// Bounding box size
		/// </summary>
		Vector3 boundSize = Vector3.One;

		/// <summary>
		/// Bounding entity
		/// </summary>
		Entity bound;

		/// <summary>
		/// Internal sprite
		/// </summary>
		SpriteComponent sprite;

		/// <summary>
		/// Map sprite
		/// </summary>
		public MapSprite() {
			sprite = new SpriteComponent() {
				AffectedByLight = true,
				AffectedByFog = true,
				Facing = SpriteComponent.FacingMode.Y
			};
			AddComponent(sprite);
			bound = new Entity() {
				Parent = this
			};
			bound.Position = Position;
		}

		/// <summary>
		/// Adding to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Assign(World.Scene scene) {
			scene.Entities.Add(this);
			scene.Entities.Add(bound);
		}

		/// <summary>
		/// Removing from scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Unassign(World.Scene scene) {
			scene.Entities.Remove(this);
			scene.Entities.Remove(bound);
		}

		/// <summary>
		/// Saving sprite
		/// </summary>
		/// <param name="f"></param>
		public override void Save(System.IO.BinaryWriter f) {
			base.Save(f);

			// Writing version
			f.Write((byte)2);

			// Writing data
			f.Write(Texture != null);
			if (Texture != null) {
				f.Write(Texture.Link);
			}

			// Basic settings
			f.Write(Tint.R);
			f.Write(Tint.G);
			f.Write(Tint.B);
			f.Write(Tint.A);
			f.Write(AffectedByLight);
			f.Write(Offset.X);
			f.Write(Offset.Y);
			f.Write(Scale.X);
			f.Write(Scale.Y);
			
			// Extended settinfs
			f.Write(AffectedByFog);
			f.Write(Solid);
			f.Write(BoundOffset.X);
			f.Write(BoundOffset.Y);
			f.Write(BoundOffset.Z);
			f.Write(BoundSize.X);
			f.Write(BoundSize.Y);
			f.Write(BoundSize.Z);

		}

		/// <summary>
		/// Loading sprite
		/// </summary>
		/// <param name="f"></param>
		public override void Load(System.IO.BinaryReader f) {
			base.Load(f);

			// Reading version
			int ver = 0;
			try {
				ver = f.ReadByte();
			} catch (Exception ex) { }

			if (ver >= 1) {

				// Reading texture
				if (f.ReadBoolean()) {
					Texture = new Texture(f.ReadString());
				}

				// Default params
				byte[] colors = f.ReadBytes(4);
				Tint = System.Drawing.Color.FromArgb(colors[3], colors[0], colors[1], colors[2]);
				AffectedByLight = f.ReadBoolean();

				// Dimensions
				Vector2 pos = Vector2.Zero, scale = Vector2.One;
				pos.X = f.ReadSingle();
				pos.Y = f.ReadSingle();
				scale.X = f.ReadSingle();
				scale.Y = f.ReadSingle();
				Offset = pos;
				Scale = scale;

			}

			if (ver >= 2) {

				// Extended config
				AffectedByFog = f.ReadBoolean();

				Vector3 boundPos = Vector3.Zero, boundSize = Vector3.Zero;
				Solid = f.ReadBoolean();
				boundPos.X = f.ReadSingle();
				boundPos.Y = f.ReadSingle();
				boundPos.Z = f.ReadSingle();
				boundSize.X = f.ReadSingle();
				boundSize.Y = f.ReadSingle();
				boundSize.Z = f.ReadSingle();
				BoundOffset = boundPos;
				BoundSize = boundSize;


			}
		}

		/// <summary>
		/// Updating bounding box
		/// </summary>
		void UpdateBounds() {
			if (boundEnabled) {
				bound.LocalPosition = boundPosition;
				bound.BoxCollider = new Collider() {
					Size = boundSize
				};
			} else {
				bound.BoxCollider = null;
			}
		}
	}
}
