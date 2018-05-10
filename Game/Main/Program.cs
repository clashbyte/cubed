using Cubed.Core;
using System;
using Cubed.Drivers.Rendering;
using Cubed.Main.Testing;


namespace Cubed.Main {
	class Program {
		
		
		static void Main(string[] args) {
			
			
			Engine engine = new Engine();
			LevelTest2 lt = new LevelTest2();
			lt.Run(engine);

			WindowDisplay display = new WindowDisplay();
			display.Engine = engine;
			display.Run();
		}
	}
}
