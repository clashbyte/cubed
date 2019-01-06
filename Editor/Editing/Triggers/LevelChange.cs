using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Data.Editor.Attributes;
using Cubed.Data.EditorGlue.Attributes;
using Cubed.Forms.Resources;
using Cubed.Graphics;

namespace Cubed.Editing.Triggers {

	[InspectorIcon("LevelChange")]
	[InspectorName("LevelChange")]
	[InspectorDescription("LevelChangeDesc")]
	[TargetPrefab(typeof(Prefabs.Triggers.LevelChangeTrigger))]
	public class LevelChange : TriggerObject {

		/// <summary>
		/// Next map
		/// </summary>
		[HintedFilePicker(".map")]
		public string NextMap {
			get {
				return (Prefab as Prefabs.Triggers.LevelChangeTrigger).MapName;
			}
			set {
				(Prefab as Prefabs.Triggers.LevelChangeTrigger).MapName = value;
			}
		}

		/// <summary>
		/// Hidden texture
		/// </summary>
		static Texture texture;

		/// <summary>
		/// Color for trigger
		/// </summary>
		protected override Color GizmoColor {
			get {
				return Color.Red;
			}
		}

		/// <summary>
		/// Icon for level change
		/// </summary>
		protected override Texture GizmoIcon {
			get {
				Texture t = texture;
				if (t == null || t.IsReleased) {
					t = new Texture(InspectorIcons.LevelChange) {
						Filtering = Texture.FilterMode.Enabled
					};
					texture = t;
				}
				return t;
			}
		}

	}
}
