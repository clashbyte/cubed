using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.Data.Files.Attributes;

namespace Cubed.Data.Files {

	/// <summary>
	/// Chunk that contains all the data
	/// </summary>
	[ChunkTag(1)]
	public class ContainerChunk : Chunk {

		/// <summary>
		/// Main data chunk
		/// </summary>
		public List<Chunk> Children {
			get;
			private set;
		}

		/// <summary>
		/// Decomposing data
		/// </summary>
		/// <param name="data">Raw data</param>
		protected override void Read(byte[] data) {
			BinaryReader f = new BinaryReader(new MemoryStream(data), System.Text.Encoding.UTF8);
			
			// Reading all the children
			int total = f.ReadUInt16();
			Children.Clear();
			for (int i = 0; i < total; i++) {

				// Skip to size
				f.BaseStream.Position += 8;
				int size = f.ReadInt32();
				f.BaseStream.Position -= 12;

				// Reading data
				byte[] chData = f.ReadBytes(size);
				Chunk child = ReadRaw(chData);
				if (child != null) {
					Children.Add(child);
				}
			}
		}

		/// <summary>
		/// Composing child chunks
		/// </summary>
		/// <returns></returns>
		protected override byte[] Write() {
			MemoryStream ms = new MemoryStream();
			BinaryWriter f = new BinaryWriter(ms, System.Text.Encoding.UTF8);

			// Writing all the children
			f.Write((ushort)Children.Count);
			foreach (Chunk c in Children) {
				f.Write(WriteRaw(c));
			}

			return ms.ToArray();
		}

	}
}
