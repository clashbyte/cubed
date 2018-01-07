using Cubed.Core;
using System;


namespace Cubed.Main {
	class Program {
		
		
		static void Main(string[] args) {
			Engine engine = new Engine();
			engine.MakeCurrent();


			Testing.UITest test = new Testing.UITest();
			test.Run(engine);


			engine.Run();
		}
	}
}
