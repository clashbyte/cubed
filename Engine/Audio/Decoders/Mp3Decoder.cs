using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Cubed.Core;
using MP3Sharp;

namespace Cubed.Audio.Decoders {

	/// <summary>
	/// Decoder for MP3 files
	/// </summary>
	internal class Mp3Decoder : AudioDecoder {

		/// <summary>
		/// Base stream
		/// </summary>
		Stream baseStream;

		/// <summary>
		/// MP3 decoding stream
		/// </summary>
		MP3Stream defaultDecoder;

		/// <summary>
		/// Reading data
		/// </summary>
		/// <param name="nextData">Flag for next data</param>
		/// <param name="readAll">Flag for one-shot read</param>
		/// <returns>Data</returns>
		public override AudioData Read(int nextData = -1, bool readAll = false) {
			return ReadWithCustomStream(defaultDecoder, nextData, readAll);
		}

		/// <summary>
		/// Reading data
		/// </summary>
		/// <param name="nextData">Flag for next data</param>
		/// <param name="readAll">Flag for one-shot read</param>
		/// <returns>Data</returns>
		public AudioData ReadWithCustomStream(MP3Stream stream, int nextData, bool readAll) {
			int pos = 0;
			if (nextData != -1) {
				pos = nextData;
			}
			baseStream.Position = pos;

			// Decoding method
			byte[] buffer = null;
			int next = -1;
			if (readAll) {

				// Reading whole file
				MemoryStream ms = new MemoryStream();
				byte[] copyBuffer = new byte[65535];
				while (!stream.IsEOF) {
					int total = stream.Read(copyBuffer, 0, copyBuffer.Length);
					ms.Write(copyBuffer, 0, total);
				}

				// Closing buffer
				buffer = ms.ToArray();
				stream.Close();

			} else {

				// Preparing buffer
				int framesToRead = 8;
				bool ended = false;
				MemoryStream ms = new MemoryStream();
				for (int i = 0; i < framesToRead; i++) {

					// Reading current frame
					stream.CleanCache();
					int totalFrames = stream.DecodeFrames(1);
					if (totalFrames == 0) {
						ended = true;
					}

					// Transferring buffer
					int size = stream.CachedBytesLeft;
					if (size > 0) {
						byte[] tempBuffer = new byte[size];
						stream.Read(tempBuffer, 0, size);
						ms.Write(tempBuffer, 0, size);
					}
					if (ended) {
						break;
					}
				}

				// Next handle
				buffer = ms.ToArray();
				next = (int)baseStream.Position;
				if (ended) {
					next = -1;
				}

			}

			// Reading data
			AudioData data = new AudioData();
			data.Data = buffer;
			data.Channels = stream.ChannelCount;
			data.Depth = 16;
			data.Samples = stream.Frequency;
			data.NextData = next;
			return data;
		}

		/// <summary>
		/// Setting up
		/// </summary>
		protected override void Setup() {
			baseStream = GetEngine().Filesystem.GetStream(File);
			defaultDecoder = new MP3Stream(baseStream);

			// Skipping currently read buffer
			defaultDecoder.CleanCache();
		}

		/// <summary>
		/// Returning new decoder
		/// </summary>
		/// <returns></returns>
		public MP3Stream ObtainCustomDecoder() {
			MP3Stream ms = new MP3Stream(baseStream);
			ms.CleanCache();
			return ms;
		}
	}
}
