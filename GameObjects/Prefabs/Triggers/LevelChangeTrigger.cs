using Cubed.Data.Game.Attributes;
using System.ComponentModel;
using System.IO;

namespace Cubed.Prefabs.Triggers {

	/// <summary>
	/// Level change trigger
	/// </summary>
	[Prefab(100)]
	public class LevelChangeTrigger : TriggerPrefab {

		/// <summary>
		/// Map change target
		/// </summary>
		[DefaultValue("")]
		public string MapName {
			get;
			set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public LevelChangeTrigger() {
			MapName = "";
		}

		/// <summary>
		/// Writing params
		/// </summary>
		protected override void LoadTriggerParams(BinaryReader f) {
			int ver = f.ReadByte();
			MapName = f.ReadString();
		}

		/// <summary>
		/// Reading params
		/// </summary>
		protected override void SaveTriggerParams(BinaryWriter f) {
			f.Write((byte)1);
			f.Write(MapName);
		}
	}
}
