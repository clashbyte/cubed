using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cubed.Data.Files;

namespace Cubed.Data.Editor {

	/// <summary>
	/// Class for clipboard
	/// </summary>
	[Serializable]
	public class ClipboardContent {

		/// <summary>
		/// Internal format for clipboard
		/// </summary>
		const string FORMAT = "CubedData";

		/// <summary>
		/// Clipboard content
		/// </summary>
		public Chunk Content {
			get {
				if (rawData == null) {
					return null;
				}
				return ChunkedFile.ReadFromBytes(rawData);
			}
			set {
				if (value != null) {
					rawData = ChunkedFile.WriteAsBytes(value);
				} else {
					rawData = null;
				}
			}
		}

		/// <summary>
		/// Chunk raw content
		/// </summary>
		byte[] rawData;


		/// <summary>
		/// Constructor
		/// </summary>
		public ClipboardContent() {
			rawData = null;
		}

		/// <summary>
		/// Reading data from clipboard
		/// </summary>
		/// <returns>Chunk</returns>
		public static Chunk Read() {
			if (Clipboard.ContainsData(FORMAT)) {
				ClipboardContent cc = (ClipboardContent)Clipboard.GetData(FORMAT);
				if (cc != null) {
					return cc.Content;
				}
			}
			return null;
		}

		/// <summary>
		/// Checking clipboard for chunk
		/// </summary>
		public static bool HasChunk() {
			return Clipboard.ContainsData(FORMAT);
		}

		/// <summary>
		/// Saving chunk to clipboard
		/// </summary>
		public static void Write(Chunk chunk) {
			ClipboardContent cc = new ClipboardContent();
			cc.Content = chunk;
			
			Clipboard.Clear();
			Clipboard.SetData(FORMAT, cc);
		}

	}
}
