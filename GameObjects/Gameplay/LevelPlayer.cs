using System.IO;

namespace Cubed.Gameplay {

	/// <summary>
	/// Class that handles map playing
	/// </summary>
	public class LevelPlayer {

		/// <summary>
		/// Current player state
		/// </summary>
		public PlayerState State {
			get;
			private set;
		}

		/// <summary>
		/// Creating level player
		/// </summary>
		/// <param name="stream">Stream that contains level</param>
		public LevelPlayer(Stream stream) {

		}

		/// <summary>
		/// Start level loading
		/// </summary>
		public void StartLoading() {

		}
		
		/// <summary>
		/// Updating all logics
		/// </summary>
		public void Update() {

		}

		/// <summary>
		/// Current loading state
		/// </summary>
		public enum PlayerState {
			Empty,
			LoadingMap,
			LoadingTextures,
			LoadingEntities,
			LoadingSounds,
			Complete,
			Playing,
			Paused
		}

	}
}
