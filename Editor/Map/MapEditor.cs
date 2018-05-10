using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Core;
using Cubed.UI.Basic;
using Cubed.World;

namespace Cubed.Map {

	/// <summary>
	/// Map editor main class
	/// </summary>
	public partial class MapEditor {

		/// <summary>
		/// Item initialization
		/// </summary>
		static bool _initialized = false;

		/// <summary>
		/// Current engine
		/// </summary>
		static Engine engine;

		/// <summary>
		/// Current scene
		/// </summary>
		static Scene scene;

		/// <summary>
		/// Current camera
		/// </summary>
		static Camera camera;

		/// <summary>
		/// Tool button array
		/// </summary>
		static RadioButton[] toolButtons;

		/// <summary>
		/// Editor start
		/// </summary>
		public static void Start() {


			// Starting engine
			engine = new Engine();
			//engine.Title = "Cubed Map Editor";
			engine.UpdateLogic += engine_UpdateLogic;
			//engine.Run();


		}

		/// <summary>
		/// Initialization and updating
		/// </summary>
		static void engine_UpdateLogic(object sender, Engine.UpdateEventArgs e) {
			
		}

		/// <summary>
		/// Initializing
		/// </summary>
		static void Init() {

		}

		/// <summary>
		/// Updating
		/// </summary>
		static void Update() {

		}

	}
}
