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
			engine.MouseLock = false;
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
				ui.Items.Add(new ToggleButton() {
					Position = Vector2.One * 15f,
					Size = new Vector2(100f, 30f),
					Text = "Button test",
					Anchor = Control.AnchorMode.BottomRight
				});
				eng.Interface = ui;

				init = true;
			}

		}

	}
}
