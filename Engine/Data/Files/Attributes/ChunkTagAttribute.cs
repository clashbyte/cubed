using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Data.Files.Attributes {
	
	/// <summary>
	/// Attribute to store unique chunk ID
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	internal class ChunkTagAttribute : Attribute {

		/// <summary>
		/// Custom tag for chunk
		/// </summary>
		public ushort Tag {
			get;
			private set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag</param>
		public ChunkTagAttribute(int tag) {
			Tag = (ushort)tag;
		}

	}
}
