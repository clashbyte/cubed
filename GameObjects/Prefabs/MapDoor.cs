using System;
using System.IO;
using Cubed.Audio;
using Cubed.Components.Audio;
using Cubed.Core;
using Cubed.Data.Game.Attributes;
using Cubed.Graphics;
using Cubed.Maths;
using Cubed.Prefabs.Classes;
using Cubed.World;
using OpenTK;

namespace Cubed.Prefabs {

	/// <summary>
	/// Prefab for door
	/// </summary>
	[Prefab(5)]
	public class MapDoor : GamePrefab, IPlayerUsable {

		/// <summary>
		/// Checking for loading ready
		/// </summary>
		public override bool Ready {
			get {
				if (FrontTexture != null && FrontTexture.State != Texture.LoadingState.Complete) {
					return false;
				}
				if (RearTexture != null && RearTexture.State != Texture.LoadingState.Complete) {
					return false;
				}
				if (SideTexture != null && SideTexture.State != Texture.LoadingState.Complete) {
					return false;
				}
				if (door == null) {
					return false;
				}
				return true;
				//return !door.NeedRebuild && !door.StaticInvalid && !door.DynamicInvalid;
			}
		}

		/// <summary>
		/// Front texture
		/// </summary>
		public Texture FrontTexture {
			get {
				return door.FrontTexture;
			}
			set {
				door.FrontTexture = value;
			}
		}

		/// <summary>
		/// Rear texture
		/// </summary>
		public Texture RearTexture {
			get {
				return door.RearTexture;
			}
			set {
				door.RearTexture = value;
			}
		}

		/// <summary>
		/// Side texture
		/// </summary>
		public Texture SideTexture {
			get {
				return door.SideTexture;
			}
			set {
				door.SideTexture = value;
			}
		}

		/// <summary>
		/// Sound for open
		/// </summary>
		public AudioTrack OpenSound {
			get {
				return tracks[0];
			}
			set {
				tracks[0] = value;
			}
		}

		/// <summary>
		/// Sound for closing
		/// </summary>
		public AudioTrack CloseSound {
			get {
				return tracks[1];
			}
			set {
				tracks[1] = value;
			}
		}

		/// <summary>
		/// Sound for locked door
		/// </summary>
		public AudioTrack LockedSound {
			get {
				return tracks[2];
			}
			set {
				tracks[2] = value;
			}
		}

		/// <summary>
		/// Open flag
		/// </summary>
		public bool Open {
			get;
			set;
		}

		/// <summary>
		/// Door opening type
		/// </summary>
		public Door.DoorType Type {
			get {
				return door.OpenType;
			}
			set {
				door.OpenType = value;
			}
		}

		/// <summary>
		/// Rotation of this door
		/// </summary>
		public float Angle {
			get {
				return angle;
			}
			set {
				angle = value;
				if (door != null) {
					door.LocalAngles = Vector3.UnitY * angle;
				}
			}
		}

		/// <summary>
		/// Total width
		/// </summary>
		public int Width {
			get {
				return door.Width;
			}
			set {
				door.Width = value;
			}
		}

		/// <summary>
		/// Total height
		/// </summary>
		public int Height {
			get {
				return door.Height;
			}
			set {
				door.Height = value;
			}
		}

		/// <summary>
		/// Thickness
		/// </summary>
		public float Thickness {
			get {
				return door.Thickness;
			}
			set {
				door.Thickness = value;
			}
		}

		/// <summary>
		/// Open-close speed
		/// </summary>
		public float Speed {
			get;
			set;
		}

		/// <summary>
		/// Check if player can use it
		/// </summary>
		bool IPlayerUsable.CanUse {
			get {
				return !Open;
			}
		}

		/// <summary>
		/// Internal door object
		/// </summary>
		Door door;

		/// <summary>
		/// Current opening state
		/// </summary>
		float delta;

		/// <summary>
		/// Current door angle
		/// </summary>
		float angle;

		/// <summary>
		/// Target side
		/// </summary>
		int openSide;

		/// <summary>
		/// Tracks 
		/// </summary>
		AudioTrack[] tracks;

		/// <summary>
		/// Sound source
		/// </summary>
		SoundSource soundSource;

		/// <summary>
		/// Constructor
		/// </summary>
		public MapDoor() {
			door = new Door();
			tracks = new AudioTrack[3];
			openSide = 1;
			Speed = 0.3f;

			soundSource = new SoundSource(null) {
				Loop = false,
			};
			door.AddComponent(soundSource);
		}

		/// <summary>
		/// Adding door to scene
		/// </summary>
		public override void Assign(Scene scene) {
			if (door == null) {
				door = new Door();
			}
			scene.Entities.Add(door);
			scene.Entities.Add(this);

			door.Parent = this;
			door.Position = Position;
			door.LocalAngles = Vector3.UnitY * angle;
			door.ParentMap = scene.Map;
		}

		/// <summary>
		/// Removing door from scene
		/// </summary>
		public override void Unassign(Scene scene) {
			if (door != null) {
				scene.Entities.Remove(door);
				door.Destroy();
				door = null;
			}
			scene.Entities.Remove(this);
		}

		/// <summary>
		/// Updating door
		/// </summary>
		public override void Update() {
			base.Update();
			float add = Math.Min(0.01f + Speed * 0.1f, 1f);
			if (!Open) {
				if (delta != 0) {
					if (Math.Abs(delta) <= add) {
						delta = 0;
					} else {
						delta = Math.Max(Math.Min(delta - add * openSide, 1f), -1f); 
					}
				}
			} else {
				delta = Math.Max(Math.Min(delta + add * openSide, 1f), -1f);
			}
			door.Value = delta;
		}

		/// <summary>
		/// Destroying door
		/// </summary>
		public override void Destroy() {
			if (door != null) {
				door.Destroy();
			}
			base.Destroy();
		}

		/// <summary>
		/// Writing door to stream
		/// </summary>
		/// <param name="f">Writer</param>
		public override void Save(BinaryWriter f) {
			base.Save(f);

			// Writing revision
			f.Write((byte)2);

			// Writing textures
			if (FrontTexture != null) {
				f.Write(FrontTexture.Link);
			} else {
				f.Write("");
			}
			if (RearTexture != null) {
				f.Write(RearTexture.Link);
			} else {
				f.Write("");
			}
			if (SideTexture != null) {
				f.Write(SideTexture.Link);
			} else {
				f.Write("");
			}

			// Writing sounds
			for (int i = 0; i < 3; i++) {
				if (tracks[i] != null) {
					f.Write(tracks[i].Path);
				} else {
					f.Write("");
				}
			}

			// Writing params
			f.Write((byte)door.OpenType);
			f.Write(angle);
			f.Write((ushort)Width);
			f.Write((ushort)Height);
			f.Write(Thickness);
			f.Write(Open);
		}

		/// <summary>
		/// Reading data
		/// </summary>
		/// <param name="f">Reader</param>
		public override void Load(BinaryReader f) {
			base.Load(f);

			// Reading version
			int ver = 0;
			try {
				ver = f.ReadByte();
			} catch(Exception) { }

			// Basic parameters
			if (ver >= 1) {

				// Textures
				string txFront = f.ReadString();
				string txBack = f.ReadString();
				string txSide = f.ReadString();
				if (!string.IsNullOrEmpty(txFront) && Engine.Current.Filesystem.Exists(txFront)) {
					FrontTexture = new Texture(txFront, Texture.LoadingMode.Queued);
				}
				if (!string.IsNullOrEmpty(txBack) && Engine.Current.Filesystem.Exists(txBack)) {
					RearTexture = new Texture(txBack, Texture.LoadingMode.Queued);
				}
				if (!string.IsNullOrEmpty(txSide) && Engine.Current.Filesystem.Exists(txSide)) {
					SideTexture = new Texture(txSide, Texture.LoadingMode.Queued);
				}

				// Sounds (in ver 2)
				if (ver >= 2) {
					for (int i = 0; i < 3; i++) {
						string snd = f.ReadString();
						if (!string.IsNullOrEmpty(snd)) {
							tracks[i] = new AudioTrack(snd);
						} else {
							tracks[i] = null;
						}
					}
				}

				// Parameters
				door.OpenType = (Door.DoorType)f.ReadByte();
				Angle = f.ReadSingle();
				Width = f.ReadUInt16();
				Height = f.ReadUInt16();
				Thickness = f.ReadSingle();
				Open = f.ReadBoolean();
				delta = Open ? 1 : 0;
				if (door != null) {
					door.Value = delta;
				}

			}
		}

		/// <summary>
		/// Picking object for usage
		/// </summary>
		/// <param name="origin">Ray origin</param>
		/// <param name="direction">Ray direction</param>
		/// <param name="distance">Hit distance</param>
		/// <returns>True if ray hit this object</returns>
		bool IPlayerUsable.PickForUsing(Vector3 origin, Vector3 direction, out float distance) {
			float rad = MathHelper.DegreesToRadians(Angle);
			float sin = (float)Math.Sin(rad);
			float cos = (float)Math.Cos(rad);
			Vector3 size = new Vector3(
				(float)Width * cos - Thickness * sin,
				Height,
				(float)Width * sin + Thickness * cos
			) * 0.5f;
			return Intersections.RayIntersectsBox(origin, direction, Position - size, Position + size, out distance);
		}

		/// <summary>
		/// Use by player
		/// </summary>
		/// <param name="hit">Hit direction</param>
		void IPlayerUsable.Use(Vector3 hit) {
			openSide = door.PointToLocal(hit).Z > 0 ? -1 : 1;
			Open = true;

			soundSource.Track = tracks[0];
			soundSource.Loop = false;
			soundSource.Play();
		}
	}
}
