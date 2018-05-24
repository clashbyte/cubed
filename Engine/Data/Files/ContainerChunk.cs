using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Data.Files {

	/// <summary>
	/// Chunk that contains all the data
	/// </summary>
	public class ContainerChunk {

		/// <summary>
		/// Main data chunk
		/// </summary>
		public List<FileChunk> Children {
			get;
			private set;
		}

	}
}
