using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Files.Attributes;

namespace Cubed.Data.Files {

	/// <summary>
	/// Chunk with data
	/// </summary>
	[ChunkTag(2)]
	public class BinaryChunk : Chunk {

		/// <summary>
		/// Raw content
		/// </summary>
		public byte[] Content {
			get {
				return content;
			}
			set {
				if (value == null) {
					content = new byte[0];
				} else {
					content = value;
				}
			}
		}

		/// <summary>
		/// Hidden content
		/// </summary>
		byte[] content;

		/// <summary>
		/// Constructor
		/// </summary>
		public BinaryChunk() {
			content = new byte[0];
		}

		/// <summary>
		/// Reading data
		/// </summary>
		/// <param name="data">Content</param>
		protected override void Read(byte[] data) {
			content = data;
		}

		/// <summary>
		/// Writing data
		/// </summary>
		/// <returns>Data</returns>
		protected override byte[] Write() {
			return content;
		}
	}
}
