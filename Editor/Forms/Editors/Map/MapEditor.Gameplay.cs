using Cubed.Data.Files;
using Cubed.Editing;
using Cubed.Editing.Gizmos;
using Cubed.Forms.Common;
using Cubed.Gameplay;
using Cubed.UI;

namespace Cubed.Forms.Editors.Map {
	
	partial class MapEditor {

		/// <summary>
		/// Flag for play mode
		/// </summary>
		bool isPlayModeEnabled;

		/// <summary>
		/// Root game object
		/// </summary>
		Game game;

		/// <summary>
		/// Previous pre-game UI
		/// </summary>
		UserInterface lastInterface;

		/// <summary>
		/// Map saved before playing
		/// </summary>
		Chunk prePlayMap;

		/// <summary>
		/// Enter play mode
		/// </summary>
		void StartPlayMode() {
			// Stopping all sounds
			lastInterface = engine.Interface;
			engine.World = null;
			engine.Interface = null;

			// Releasing entities
			prePlayMap = SaveToChunk();
			foreach (EditableObject eo in sceneSelectedObjects) {
				Gizmo[] gizmos = eo.ControlGizmos;
				eo.Deselect(scene);
				foreach (Gizmo gz in eo.ControlGizmos) {
					gz.Unassign(scene);
					gz.Destroy();
				}
			}
			foreach (EditableObject eo in sceneObjects) {
				eo.Destroy(scene);
			}
			sceneObjects.Clear();
			sceneSelectedObjects.Clear();
			InspectingObject = null;
			makePrefabButton.Enabled = false;
			makePrefabButton.Invalidate();
			MainForm.UpdateEditingMenu();

			// Saving map to temp stream
			game = new Game(engine);
			game.LoadLevel(ChunkedFile.WriteAsBytes(prePlayMap), true);

			// Enable play mode flag
			isPlayModeEnabled = true;
		}

		/// <summary>
		/// Exit play mode
		/// </summary>
		void StopPlayMode() {
			// Resuming all sounds
			game.Cleanup();
			LoadFromChunk(prePlayMap, true);
			prePlayMap = null;
			engine.World = scene;
			engine.Interface = lastInterface;
			isPlayModeEnabled = false;
		}

		/// <summary>
		/// Updating play mode
		/// </summary>
		void UpdatePlayMode() {
			if (game != null) {
				engine.MakeCurrent();
				game.Update();
			}
		}


		
	}
}
