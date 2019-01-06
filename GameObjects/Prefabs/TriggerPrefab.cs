using System.IO;
using Cubed.World;
using OpenTK;

namespace Cubed.Prefabs {

	/// <summary>
	/// Custom trigger
	/// </summary>
	public abstract class TriggerPrefab : GamePrefab {

		/// <summary>
		/// Trigger bounds
		/// </summary>
		public Vector3 Bounds {
			get;
			set;
		}

		public TriggerPrefab() {
			Bounds = Vector3.One;
		}

		/// <summary>
		/// Assigning to scene - do nothing
		/// </summary>
		/// <param name="scene"></param>
		public override void Assign(Scene scene) {}

		/// <summary>
		/// Removing from scene - same
		/// </summary>
		/// <param name="scene"></param>
		public override void Unassign(Scene scene) {}

		/// <summary>
		/// Save trigger data
		/// </summary>
		public override void Save(BinaryWriter f) {
			base.Save(f);
			f.Write(Bounds.X);
			f.Write(Bounds.Y);
			f.Write(Bounds.Z);
			SaveTriggerParams(f);
		}

		/// <summary>
		/// Read trigger data
		/// </summary>
		public override void Load(BinaryReader f) {
			base.Load(f);
			Vector3 bnd = Vector3.Zero;
			bnd.X = f.ReadSingle();
			bnd.Y = f.ReadSingle();
			bnd.Z = f.ReadSingle();
			LoadTriggerParams(f);
		}

		/// <summary>
		/// Saving trigger data
		/// </summary>
		protected abstract void SaveTriggerParams(BinaryWriter f);

		/// <summary>
		/// Reading trigger data
		/// </summary>
		/// <param name="f"></param>
		protected abstract void LoadTriggerParams(BinaryReader f);
		
	}
}
