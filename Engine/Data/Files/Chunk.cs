using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cubed.Data.Files.Attributes;

namespace Cubed.Data.Files {

	/// <summary>
	/// Base file chunk type
	/// </summary>
	public abstract class Chunk {

		/// <summary>
		/// All tag types dictionary
		/// </summary>
		static Dictionary<ushort, Type> tagTypes = null;

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
		/// Chunk version
		/// </summary>
		public int Version {
			get {
				return version;
			}
			set {
				if (value > 65535 || value < 0) {
					throw new Exception("Wrong version for chunk: " + value);
				}
				version = (ushort)value;
			}
		}

		/// <summary>
		/// Internal identifier
		/// </summary>
		private string id;

		/// <summary>
		/// Chunk version
		/// </summary>
		private ushort version;

		/// <summary>
		/// Reading single chunk
		/// </summary>
		/// <returns>Chunk or null</returns>
		protected static Chunk ReadRaw(byte[] data) {
			MemoryStream stream = new MemoryStream(data);
			BinaryReader f = new BinaryReader(stream, System.Text.Encoding.UTF8);
			CheckTags();

			try {

				// Reading header
				string chID = new string(f.ReadChars(4));
				ushort chVer = f.ReadUInt16();
				ushort chTag = f.ReadUInt16();
				int chLen = (int)f.ReadUInt32();

				// Checking chunk
				if (!tagTypes.ContainsKey(chTag)) {
					return null;
				}

				// Reading data
				byte[] content = new byte[0];
				try {
					content = f.ReadBytes(chLen);
				} catch (Exception) {}
				
				// Creating object
				Chunk chunk = (Chunk)Activator.CreateInstance(tagTypes[chTag]);
				chunk.id = chID;
				chunk.version = chVer;
				chunk.Read(content);
				return chunk;

			} catch (Exception) {
				return null;
			}
		}

		/// <summary>
		/// Writing single chunk
		/// </summary>
		protected static byte[] WriteRaw(Chunk chunk) {
			MemoryStream stream = new MemoryStream();
			BinaryWriter f = new BinaryWriter(stream, System.Text.Encoding.UTF8);

			Type t = chunk.GetType();
			int tag = -1;
			foreach (var item in tagTypes) {
				if (item.Value == t) {
					tag = item.Key;
					break;
				}
			}
			if (tag < 0 || tag > 65535) {
				return new byte[0];
			}

			// Writing header
			f.Write(chunk.id.ToCharArray(0, 4));
			f.Write(chunk.version);
			f.Write((ushort)tag);
			
			// Writing data
			byte[] content = chunk.Write();
			if (content == null) {
				content = new byte[0];
			}
			f.Write((int)content.Length);
			f.Write(content);

			// Complete!
			return stream.ToArray();
		}

		/// <summary>
		/// Decomposing data
		/// </summary>
		/// <param name="data">Content</param>
		protected abstract void Read(byte[] data);

		/// <summary>
		/// Composing data
		/// </summary>
		/// <returns>Array of data</returns>
		protected abstract byte[] Write();

		/// <summary>
		/// Checking and collecting all tags
		/// </summary>
		static void CheckTags() {
			if (tagTypes == null) {
				tagTypes = new Dictionary<ushort, Type>();
				Assembly asm = Assembly.GetExecutingAssembly();
				foreach (Type type in asm.GetTypes()) {
					object[] atts = type.GetCustomAttributes(typeof(ChunkTagAttribute), true);
					foreach (object att in atts) {
						ChunkTagAttribute ta = (ChunkTagAttribute)att;
						if (ta != null) {
							if (tagTypes.ContainsKey(ta.Tag)) {
								throw new Exception("Chunk tag repeated twice on class " + type.Name + " (first got in "+tagTypes[ta.Tag].Name+")");
							}
							tagTypes.Add(ta.Tag, type);
						}
					}
				}
			}
		}
	}
}
