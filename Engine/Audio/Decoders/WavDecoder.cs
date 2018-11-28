using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Cubed.Core;

namespace Cubed.Audio.Decoders {

	/// <summary>
	/// Decoder for Wav files
	/// </summary>
	internal class WavDecoder : AudioDecoder {
		
		/// <summary>
		/// File stream
		/// </summary>
		Stream stream;

		/// <summary>
		/// Binary reader
		/// </summary>
		BinaryReader reader;

		/// <summary>
		/// Number of channels
		/// </summary>
		int channels;

		/// <summary>
		/// Sample rate
		/// </summary>
		int samples;

		/// <summary>
		/// Sound depth
		/// </summary>
		int depth;

		/// <summary>
		/// Starting point for audio data
		/// </summary>
		int startPoint;

		/// <summary>
		/// Ending point for audio data
		/// </summary>
		int endPoint;

		/// <summary>
		/// Reading data
		/// </summary>
		/// <param name="nextData">Flag for next data</param>
		/// <param name="readAll">Flag for one-shot read</param>
		/// <returns>Data</returns>
		public override AudioData Read(int nextData = -1, bool readAll = false) {
			int pos = startPoint;
			if (nextData != -1) {
				pos = nextData;
			}

			// Determining position
			int size = depth * channels * 1024;
			if (readAll) {
				size = endPoint;
			}
			int next = pos + size;
			if (pos + size > endPoint) {
				size = endPoint - pos;
				next = -1;
			}
			stream.Position = pos;

			// Reading data
			AudioData data = new AudioData();
			data.Data = reader.ReadBytes(size);
			data.Channels = channels;
			data.Depth = depth;
			data.Samples = samples;
			data.NextData = next;
			return data;
		}

		/// <summary>
		/// Setting up
		/// </summary>
		protected override void Setup() {
			if (stream != null) {
				stream.Close();
			}
			stream = GetEngine().Filesystem.GetStream(File);
			reader = new BinaryReader(stream);

			// Header
			reader.BaseStream.Position = 0;
			string header = new string(reader.ReadChars(4));
			if (header != "RIFF") {
				throw new Exception("Unknown file in Wav container!");
			}

			// Sound parameters
			stream.Position = 12;
			header = new string(reader.ReadChars(4));
			if (header != "fmt ") {
				throw new Exception("Unknown format in Wav container!");
			}
			int formatSize = reader.ReadInt32();
			stream.Position += 2;
			channels = reader.ReadUInt16();
			samples = reader.ReadInt32();
			stream.Position += 6;
			depth = reader.ReadUInt16();

			// Position
			stream.Position = 20 + formatSize;
			header = new string(reader.ReadChars(4));
			if (header != "data") {
				throw new Exception("Unknown data in Wav container!");
			}
			int targetSize = reader.ReadInt32();
			startPoint = (int)stream.Position;
			endPoint = startPoint + targetSize;
		}
	}
}
