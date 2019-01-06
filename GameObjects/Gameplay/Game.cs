using Cubed.Core;
using Cubed.Interface;
using System.IO;

namespace Cubed.Gameplay {

	/// <summary>
	/// Class that handles all game states
	/// </summary>
	public class Game {

		/// <summary>
		/// Current engine
		/// </summary>
		public Engine Engine {
			get;
			private set;
		}

		/// <summary>
		/// Current game instance
		/// </summary>
		public static Game Current {
			get;
			private set;
		}

		/// <summary>
		/// Current player state
		/// </summary>
		public PlayerState State {
			get {
				return state;
			}
			set {
				if (state != value) {
					SwitchState(value, state);
					state = value;
				}
			}
		}
		
		/// <summary>
		/// Hidden loading screen
		/// </summary>
		public LoadingScreen Loading {
			get;
			private set;
		}

		/// <summary>
		/// Current hidden state
		/// </summary>
		PlayerState state;
		
		/// <summary>
		/// Map player
		/// </summary>
		MapPlayer player;

		/// <summary>
		/// Creating gameplay
		/// </summary>
		public Game(Engine engine) {
			Engine = engine;
			State = PlayerState.Startup;
		}

		/// <summary>
		/// Loading level from name
		/// </summary>
		public void LoadLevel(string name, bool instantPlay) {
			Current = this;
			if (Engine.Filesystem.Exists(name+".map")) {
				LoadLevel(Engine.Filesystem.Get(name+".map"), instantPlay);
			}
		}

		/// <summary>
		/// Loading specified level
		/// </summary>
		public void LoadLevel(byte[] level, bool instantPlay) {
			Current = this;
			CleanPreviousMap();

			// Starting map loader/player
			player = new MapPlayer(this, level, null);
			player.ImmediatePlay = instantPlay;

		}

		/// <summary>
		/// Updating all logics
		/// </summary>
		public void Update() {
			Current = this;
			switch (state) {
				case PlayerState.Startup:
					Loading.Update();
					break;
				case PlayerState.Splash:

					break;
				case PlayerState.MainMenu:

					break;
				case PlayerState.LoadingMap:
					Loading.Update();
					player.UpdateLoading();
					break;
				case PlayerState.Gameplay:
					player.UpdateGame();
					break;
				case PlayerState.MapFinished:
					break;
				default:
					break;
			}

			// Testing editor mode
			if (Input.Controls.KeyHit(OpenTK.Input.Key.F11)) {
				LoadLevel("test2", false);
			}
			// Closing 
			if (Input.Controls.KeyHit(OpenTK.Input.Key.F12)) {
				Drivers.Rendering.Display.Current.Close();
			}
		}

		/// <summary>
		/// Clearing up resources
		/// </summary>
		public void Cleanup() {
			CleanPreviousMap();
		}

		/// <summary>
		/// Switching state
		/// </summary>
		void SwitchState(PlayerState state, PlayerState prevState) {
			switch (state) {

				// Startup splash
				case PlayerState.Startup:
					if (Loading == null) {
						Loading = new LoadingScreen(this);
					}
					Loading.IsReady = false;
					Loading.Backdrop = null;
					Loading.Show();
					break;

				// Main game splash
				case PlayerState.Splash:
					

					break;

				// Main menu
				case PlayerState.MainMenu:
					CleanPreviousMap();

					break;

				// Loading screen
				case PlayerState.LoadingMap:
					if (Loading == null) {
						Loading = new LoadingScreen(this);
					}
					Loading.IsReady = false;
					Loading.Backdrop = null;
					Loading.Show();
					
					break;

				// All gameplay
				case PlayerState.Gameplay:
					player.Show();
					break;

				// Map finished
				case PlayerState.MapFinished:

					break;

			}
		}

		/// <summary>
		/// Removing previous map
		/// </summary>
		void CleanPreviousMap() {
			if (player != null) {
				player.Cleanup();
				player = null;
			}
		}

		/// <summary>
		/// Current loading state
		/// </summary>
		public enum PlayerState {
			None,
			Startup,
			Splash,
			MainMenu,
			LoadingMap,
			Gameplay,
			MapFinished,
		}

	}
}
