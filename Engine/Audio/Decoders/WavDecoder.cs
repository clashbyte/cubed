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
		/// Audio format
		/// </summary>
		int format;

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
			if (depth == 24) {
				data.Data = Convert24BitsTo16(data.Data);
				data.Depth = 16;
			}
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
			int formatSize		= reader.ReadInt32();
			format				= reader.ReadUInt16();
			channels			= reader.ReadUInt16();
			samples				= reader.ReadInt32();
			stream.Position		+= 6;
			depth				= reader.ReadUInt16();
			stream.Position		= 20 + formatSize;

			// Position
			bool foundData = false;
			while (stream.Position < stream.Length - 1) {
				header = new string(reader.ReadChars(4));
				int targetSize = reader.ReadInt32();
				switch (header) {

					case "data":
						startPoint = (int)stream.Position;
						endPoint = startPoint + targetSize;
						stream.Position += targetSize;
						foundData = true;
						break;

					default:
						stream.Position += targetSize;
						System.Diagnostics.Debug.WriteLine("Unknown chunk: " + header);
						break;
				}
			}
			if (!foundData) {
				throw new Exception("Unknown data in Wav container!");
			}
		}

		/// <summary>
		/// Converting WAV data from 24 bits to 16
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns></returns>
		byte[] Convert24BitsTo16(byte[] data) {
			short[] raw = new short[data.Length / 3];
			for (int i = 0; i < data.Length / 3; i++) {
				int idx = i * 3;
				int num = 
					(data[idx + 0] << 0) | 
					(data[idx + 1] << 8) | 
					(data[idx + 2] << 16);
				raw[i] = (short)((float)num / 8388607f * (float)(short.MaxValue - 1));
			}
			byte[] output = new byte[raw.Length * 2];
			Buffer.BlockCopy(raw, 0, output, 0, output.Length);
			return output;
		}
	}
}
