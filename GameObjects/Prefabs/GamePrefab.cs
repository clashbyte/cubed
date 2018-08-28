using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.World;
using OpenTK;

namespace Cubed.Prefabs {

	/// <summary>
	/// Game prefab
	/// </summary>
	public abstract class GamePrefab : Entity {

		/// <summary>
		/// Assigning to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public abstract void Assign(Scene scene);

		/// <summary>
		/// Removing from scene
		/// </summary>
		/// <param name="scene">Scene to create</param>
		public abstract void Unassign(Scene scene);

		/// <summary>
		/// Saving file
		/// </summary>
		public virtual void Save(BinaryWriter f) {
			f.Write(Position.X);
			f.Write(Position.Y);
			f.Write(Position.Z);
			f.Write(Angles.X);
			f.Write(Angles.Y);
			f.Write(Angles.Z);
		}

		/// <summary>
		/// Saving file
		/// </summary>
		public virtual void Load(BinaryReader f) {
			Vector3 pos = new Vector3(), rot = new Vector3();
			pos.X = f.ReadSingle();
			pos.Y = f.ReadSingle();
			pos.Z = f.ReadSingle();
			rot.X = f.ReadSingle();
			rot.Y = f.ReadSingle();
			rot.Z = f.ReadSingle();
			Position = pos;
			Angles = rot;
		}
	}
}
