using Cubed.Core;
using System;
using Cubed.Drivers.Rendering;
using Cubed.Main.Testing;


namespace Cubed.Main {
	class Program {
		
		
		static void Main(string[] args) {
			
			Engine engine = new Engine();
			LevelTest3 lt = new LevelTest3();
			lt.Run(engine);

			OpenTK.DisplayDevice dd = OpenTK.DisplayDevice.Default;


			WindowDisplay display = new WindowDisplay() {
				Fullscreen = false,
				Resolution = new OpenTK.Vector2(
					dd.Width - 200,
					dd.Height - 100
				)
			};
			display.Engine = engine;
			display.Run();
		}
	}
}
