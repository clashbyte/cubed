using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Game.Attributes;

namespace Cubed.Prefabs {
	
	[Prefab(3)]
	public class PlayerStart : GamePrefab {


		public override void Assign(World.Scene scene) {
			//throw new NotImplementedException();
		}

		public override void Unassign(World.Scene scene) {
			//throw new NotImplementedException();
		}

		public override void Save(System.IO.BinaryWriter f) {
			base.Save(f);
		}

		public override void Load(System.IO.BinaryReader f) {
			base.Load(f);
		}
	}
}
