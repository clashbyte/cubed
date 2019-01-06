using Cubed.Core;
using System;
using Cubed.Drivers.Rendering;
using Cubed.Main.Testing;
using Cubed.Drivers.Files;
using Cubed.Gameplay;
using System.Collections.Generic;
using Cubed.Data.Defines;
using System.Drawing;
using Cubed.Graphics;

namespace Cubed.Main {

	/// <summary>
	/// Root app class
	/// </summary>
	class Program {

		/// <summary>
		/// Current engine
		/// </summary>
		static Engine engine;

		/// <summary>
		/// Current game loop
		/// </summary>
		static Game game;

		/// <summary>
		/// Hidden options
		/// </summary>
		static Dictionary<string, string> options;

		/// <summary>
		/// Flag for initialization
		/// </summary>
		static bool initialized;

		/// <summary>
		/// Main method
		/// </summary>
		static void Main(string[] args) {
			#if DEBUG
			Bootstrap(args);
			#else
			try {
				Bootstrap(args);
			} catch(Exception e) {

				// Dropping error to console if debug is enabled
				if (options != null) {
					if (options.ContainsKey("debug")) {
						Console.Error.WriteLine(e);
						return;
					}
				}

				// Showing error window

			}
			#endif
		}

		/// <summary>
		/// Running program
		/// </summary>
		static void Bootstrap(string[] args) {
			
			// Parsing command line arguments
			options = new Dictionary<string, string>();
			for (int i = 0; i < args.Length; i++) {
				string k = args[i].Trim();
				if (k.StartsWith("-")) {
					k = k.Substring(1);
					string v = "";
					if (args.Length > i + 1) {
						v = args[i + 1];
						if (v.StartsWith("-")) {
							v = "";
						} else {
							i++;
						}
					}
					if (options.ContainsKey(k)) {
						options[k] = v;
					} else {
						options.Add(k, v);
					}
				}
			}

			// Making filesystem
			FileSystem fs = null;
			if (options.ContainsKey("unpacked")) {
				if (options.ContainsKey("folder") && System.IO.Directory.Exists(options["folder"])) {
					fs = new FolderFileSystem() {
						RootFolder = options["folder"]
					};
				}
			}

			// Creating engine
			engine = new Engine();
			engine.Filesystem = fs;
			engine.UpdateLogic += LogicStep;

			// Reading basic game info
			engine.MakeCurrent();
			ProjectBasicInfo info = ProjectBasicInfo.Read();

			// Parsing screen settings
			OpenTK.DisplayDevice dd = OpenTK.DisplayDevice.Default;
			int width = dd.Width;
			int height = dd.Height;
			bool fullscreen = true;
			if (options.ContainsKey("windowed")) {
				width -= 200;
				height -= 100;
				fullscreen = false;
			}

			// Starting screen
			WindowDisplay display = new WindowDisplay() {
				Fullscreen = fullscreen,
				Resolution = new OpenTK.Vector2(
					width, height
				),
				Title = info.Name,
				Icon = info.Icon != null ? IconHelper.FromImage(info.Icon) : null
			};
			display.Engine = engine;
			display.Run();
		}

		/// <summary>
		/// Updating game stuff
		/// </summary>
		static void LogicStep(object sender, Engine.UpdateEventArgs e) {
			if (!initialized) {

				// Creating game instance
				game = new Game(engine);
				if (options.ContainsKey("map")) {
					game.LoadLevel(options["map"], false);
				}
				initialized = true;

			}

			// Updating game
			game.Update();
		}
	}
}
