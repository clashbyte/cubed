using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Editor.Attributes;
using Cubed.Editing;
using Cubed.Editing.Triggers;
using Cubed.UI.Controls;
using Cubed.UI.Graphics;

namespace Cubed.Forms.Editors.Map {
	partial class MapEditor {

		/// <summary>
		/// Things you can spawn
		/// </summary>
		static readonly Type[] logicTypes = new Type[] {
			typeof(PlayerSpawn),
			typeof(MapLight),
			typeof(MapSprite),
			typeof(MapSound),
			typeof(LevelChange),
			typeof(MapDoor),
		};

		/// <summary>
		/// Open logics tool
		/// </summary>
		void LogicToolOpen() {
			if (entityList.Entries.Count == 0) {
				foreach (Type t in logicTypes) {
					object[] attributes = t.GetCustomAttributes(true);
					
					string name = "";
					UIIcon icon = null;
					foreach (Attribute attr in attributes) {
						if (attr is InspectorNameAttribute) {
							name = (attr as InspectorNameAttribute).Name;
						} else if (attr is InspectorIconAttribute) {
							icon = (attr as InspectorIconAttribute).Icon;
						}
					}

					if (icon != null) {
						NSDirectoryInspector.Entry entry = new NSDirectoryInspector.Entry() {
							Name = name,
							IsDraggable = true,
							MainIcon = icon,
							Tag = t
						};
						entityList.Entries.Add(entry);
					}
				}
			}
			listContainer.Panel2Collapsed = false;
			SelectToolOpen();
		}

		/// <summary>
		/// Close logics tool
		/// </summary>
		void LogicToolClose() {
			listContainer.Panel2Collapsed = true;
			SelectToolClose();
		}

		/// <summary>
		/// Update logics tool
		/// </summary>
		void LogicToolUpdate() {
			SelectToolUpdate();
		}

	}
}
