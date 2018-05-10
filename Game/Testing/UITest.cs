using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Core;
using Cubed.Drivers.Files;
using Cubed.UI;
using Cubed.UI.Basic;
using Cubed.UI.Misc;
using Cubed.World;
using OpenTK;

namespace Cubed.Main.Testing {

	/// <summary>
	/// User interface
	/// </summary>
	public class UITest {

		/// <summary>
		/// Engine handle
		/// </summary>
		Engine eng;

		/// <summary>
		/// Init flag
		/// </summary>
		bool init = false;

		/// <summary>
		/// Start the game
		/// </summary>
		public void Run(Engine engine) {

			// Handling logical update
			engine.UpdateLogic += engine_UpdateLogic;
			//engine.MouseLock = false;
			engine.Filesystem = new FolderFileSystem() {
				RootFolder = @"D:\Sharp\Cubed\Project"
			};
			eng = engine;

		}

		void engine_UpdateLogic(object sender, Engine.UpdateEventArgs e) {

			if (!init) {

				// Creating empty scene
				Scene scn = new Scene();
				eng.World = scn;

				RadioGroup rg = new RadioGroup();
				UserInterface ui = new UserInterface();
				/*
				ui.Items.Add(new Label() {
					Position = Vector2.One * 30f,
					Size = new Vector2(300f, 50f),
					Text = "label test",
					Color = Color.White,
					FontSize = 12f
				});
				*/
				Icons[] icons = new Icons[] {
					Icons.mouse_pointer,
					Icons.cube,
					Icons.paint_brush,
					Icons.briefcase,
					Icons.object_group
				};
				for (int i = 0; i < 5; i++) {
					ui.Items.Add(new RadioButton() {
						Position = Vector2.One * 15f + Vector2.UnitY * 40f * i,
						Size = new Vector2(30f, 30f),
						Icon = icons[i],
						Anchor = Control.AnchorMode.BottomRight,
						Group = rg
					});
				}
				eng.Interface = ui;

				init = true;
			}

		}

	}
}
