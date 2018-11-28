using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Cubed.Core;
using NVorbis;

namespace Cubed.Audio.Decoders {

	/// <summary>
	/// Decoder for OGG files
	/// </summary>
	internal class OggDecoder : AudioDecoder {
		
		/// <summary>
		/// File stream
		/// </summary>
		Stream stream;

		/// <summary>
		/// Reader class
		/// </summary>
		VorbisReader reader;

		/// <summary>
		/// Starting point for audio data
		/// </summary>
		int startPoint;

		/// <summary>
		/// Max file size
		/// </summary>
		int maxSize;

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
			reader.DecodedPosition = pos;

			// Decoding method
			float[] samples = null;
			byte[] buffer = null;
			int next = -1;
			if (readAll) {
				MemoryStream ms = new MemoryStream();
				float[] copyBuffer = new float[65535];
				while (true) {
					int total = reader.ReadSamples(copyBuffer, 0, copyBuffer.Length);
					if (total == 0) {
						break;
					}
					float[] tempData = new float[total];
					Array.Copy(copyBuffer, tempData, total);
					byte[] converted = ConvertBuffer(tempData);
					ms.Write(converted, 0, converted.Length);
				}
				buffer = ms.ToArray();
			} else {
				int size = 32768;
				float[] tempBuffer = new float[size];
				int total = reader.ReadSamples(tempBuffer, 0, size);

				samples = new float[total];
				Array.Copy(tempBuffer, samples, total);
				buffer = ConvertBuffer(samples);

				next = (int)reader.DecodedPosition;
				if (reader.TotalSamples <= next || total == 0) {
					next = -1;
				}
			}

			// Reading data
			AudioData data = new AudioData();
			data.Data = buffer;
			data.Channels = reader.Channels;
			data.Depth = 16;
			data.Samples = reader.SampleRate;
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
			reader = new VorbisReader(stream, true);
			startPoint = 0;
		}

		/// <summary>
		/// Converting float array to byte-based short array
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		byte[] ConvertBuffer(float[] source) {
			short[] temp = new short[source.Length];
			for (int i = 0; i < source.Length; i++) {
				temp[i] = (short)(source[i] * 32767f);
			}
			byte[] result = new byte[temp.Length * sizeof(short)];
			Buffer.BlockCopy(temp, 0, result, 0, result.Length);
			return result;
		}
	}
}
