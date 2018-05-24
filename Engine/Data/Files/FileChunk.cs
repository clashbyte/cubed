using System;

namespace Cubed.Data.Files {

	/// <summary>
	/// Base file chunk type
	/// </summary>
	public class FileChunk {

		/// <summary>
		/// Chunk identifier
		/// </summary>
		public string ID {
			get {
				return id;
			}
			set {
				if (value.Length != 4) {
					throw new Exception("Identifier must be 4 characters long");
				}
				id = value.ToUpper();
			}
		}

		/// <summary>
		/// Internal identifier
		/// </summary>
		private string id;

	}
}
