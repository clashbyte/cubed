using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Game.Attributes;

namespace Cubed.Prefabs {
	
	/// <summary>
	/// Player spawn point
	/// </summary>
	[Prefab(3)]
	public class PlayerStart : GamePrefab {

		/// <summary>
		/// Player starting angle
		/// </summary>
		public float Angle {
			get;
			set;
		}

		/// <summary>
		/// Assigning to world - do nothing
		/// </summary>
		public override void Assign(World.Scene scene) {}

		/// <summary>
		/// Removing from world
		/// </summary>
		public override void Unassign(World.Scene scene) {}

		/// <summary>
		/// Saving to file
		/// </summary>
		/// <param name="f">Writer</param>
		public override void Save(System.IO.BinaryWriter f) {
			base.Save(f);

			// Writing first revision
			f.Write((byte)1);
			f.Write(Angle);

		}

		/// <summary>
		/// Reading from file
		/// </summary>
		/// <param name="f">Reader</param>
		public override void Load(System.IO.BinaryReader f) {
			base.Load(f);
			try {
				int ver = f.ReadByte();

				// Reading angle
				if (ver >= 1) {
					Angle = f.ReadSingle();
				}

			} catch (Exception) { }
		}
	}
}
