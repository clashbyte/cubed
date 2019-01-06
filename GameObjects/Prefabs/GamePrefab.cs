using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.Components;
using Cubed.Data.Files;
using Cubed.Data.Game.Attributes;
using Cubed.World;
using OpenTK;

namespace Cubed.Prefabs {

	/// <summary>
	/// Game prefab
	/// </summary>
	public abstract class GamePrefab : Entity {

		/// <summary>
		/// Is entity ready (loading state check)
		/// </summary>
		public virtual bool Ready {
			get {
				return true;
			}
		}

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
		/// Release all resources
		/// </summary>
		public new virtual void Destroy() {
			base.Destroy();
		}

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

		/// <summary>
		/// Convert prefabs to chunk array
		/// </summary>
		/// <param name="prefabs">Prefab list</param>
		/// <returns>Array of chunks</returns>
		public static Chunk[] ToChunkArray(GamePrefab[] prefabs) {
			List<Chunk> chunks = new List<Chunk>();
			foreach (GamePrefab gp in prefabs) {
				MemoryStream entStr = new MemoryStream();
				BinaryWriter bw = new BinaryWriter(entStr);
				PrefabAttribute pa = Attribute.GetCustomAttribute(gp.GetType(), typeof(PrefabAttribute)) as PrefabAttribute;
				if (pa == null) {
					continue;
				}

				// Writing object
				bw.Write(pa.ID);
				gp.Save(bw);

				// Storing object
				chunks.Add(new BinaryChunk() {
					ID = "PRFB",
					Version = 1,
					Content = entStr.ToArray()
				});
			}
			return chunks.ToArray();
		}

		/// <summary>
		/// Convert chunks to prefab array
		/// </summary>
		/// <param name="chunks">Chunk list</param>
		/// <returns>Array of prefabs</returns>
		public static GamePrefab[] FromChunkArray(Chunk[] chunks) {
			List<GamePrefab> prefabs = new List<GamePrefab>();
			foreach (Chunk ch in chunks) {
				BinaryChunk bc = ch as BinaryChunk;
				BinaryReader cr = new BinaryReader(new MemoryStream(bc.Content));

				// Reading type
				Type t = PrefabAttribute.GetPrefab(cr.ReadInt32());
				if (t != null) {
					GamePrefab pref = Activator.CreateInstance(t) as GamePrefab;
					pref.Load(cr);
					prefabs.Add(pref);
				}
			}
			return prefabs.ToArray();
		}
	}
}
