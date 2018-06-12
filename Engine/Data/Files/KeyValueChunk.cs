using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.Data.Files.Attributes;

namespace Cubed.Data.Files {

	/// <summary>
	/// Chunk with string dictionary
	/// </summary>
	[ChunkTag(3)]
	public class KeyValueChunk : Chunk {

		/// <summary>
		/// Content dictionary
		/// </summary>
		public Dictionary<string, string> Content {
			get;
			private set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public KeyValueChunk() {
			Content = new Dictionary<string, string>();
		}

		/// <summary>
		/// Decomposing data
		/// </summary>
		/// <param name="data">Raw data</param>
		protected override void Read(byte[] data) {
			BinaryReader f = new BinaryReader(new MemoryStream(data), System.Text.Encoding.UTF8);
			Content.Clear();
			int total = f.ReadUInt16();
			for (int i = 0; i < total; i++) {
				string k = f.ReadString();
				string v = f.ReadString();
				Content.Add(k, v);
			}
		}

		/// <summary>
		/// Composing data
		/// </summary>
		/// <returns>Raw data</returns>
		protected override byte[] Write() {
			MemoryStream ms = new MemoryStream();
			BinaryWriter f = new BinaryWriter(ms, System.Text.Encoding.UTF8);
			f.Write((ushort)Content.Count);
			foreach (var item in Content) {
				if (!string.IsNullOrEmpty(item.Key)) {
					f.Write(item.Key);
				} else {
					f.Write("");
				}
				if (!string.IsNullOrEmpty(item.Value)) {
					f.Write(item.Value);
				} else {
					f.Write("");
				}
			}
			return ms.ToArray();
		}
	}
}
