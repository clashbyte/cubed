using System.IO;
using Cubed.Core;
using AudioSynthesis;
using AudioSynthesis.Midi;
using AudioSynthesis.Sequencer;
using AudioSynthesis.Synthesis;
using AudioSynthesis.Bank;

namespace Cubed.Audio.Decoders {

	/// <summary>
	/// Decoder for Midi files - actually a synth file
	/// </summary>
	internal class MidiDecoder : AudioDecoder {

		/// <summary>
		/// External bank file path
		/// </summary>
		public string ExternalBankFile{
			get {
				return bankPath;
			}
			set {
				bankPath = value;
				IResource file = null;

				// Trying to load external bank
				if (bankPath != "" && System.IO.File.Exists(bankPath)) {
					file = new ExternalFile(bankPath, new FileStream(bankPath, FileMode.Open, FileAccess.Read));
				}

				// Fallback to default bank
				if (file == null) {
					if (internalBank == null) {
						internalBank = new InternalBankFile(Resources.DefaultSoundFont);
					}
					file = internalBank;
				}
				bank = new PatchBank(file);
			}
		}

		/// <summary>
		/// Disable caching for midi files
		/// </summary>
		public override bool CachingAllowed {
			get {
				return false;
			}
		}

		/// <summary>
		/// Midi file
		/// </summary>
		MidiFile midi;

		/// <summary>
		/// Sequencer
		/// </summary>
		MidiFileSequencer sequencer;

		/// <summary>
		/// Synth
		/// </summary>
		Synthesizer synth;

		/// <summary>
		/// Patch bank
		/// </summary>
		static PatchBank bank;

		/// <summary>
		/// Path to patch bank
		/// </summary>
		static string bankPath;

		/// <summary>
		/// Internal bank handle
		/// </summary>
		static InternalBankFile internalBank;

		/// <summary>
		/// Reading data
		/// </summary>
		/// <param name="nextData">Flag for next data</param>
		/// <param name="readAll">Flag for one-shot read</param>
		/// <returns>Data</returns>
		public override AudioData Read(int nextData = -1, bool readAll = false) {
			int pos = 0;
			if (nextData != -1) {
				pos = nextData;
			}
			sequencer.Seek(pos, false);
			
			// Reading file
			byte[] buffer = null;
			MemoryStream ms = new MemoryStream();
			byte[] copyBuffer = new byte[synth.RawBufferSize];
			int readTotal = 0;
			while (true) {
				sequencer.FillMidiEventQueue();
				synth.GetNext(copyBuffer);
				if (copyBuffer.Length > 0) {
					ms.Write(copyBuffer, 0, copyBuffer.Length);
				}
				readTotal += copyBuffer.Length;
				if (readTotal >= synth.SampleRate * 2 && !readAll) {
					break;
				}
				if (sequencer.CurrentTime >= sequencer.EndTime) {
					break;
				}
			}
			buffer = ms.ToArray();

			// Computing next
			int next = -1;
			if (sequencer.CurrentTime < sequencer.EndTime) {
				next = sequencer.CurrentTime;
			}

			// Reading data
			AudioData data = new AudioData();
			data.Data = buffer;
			data.Channels = 2;
			data.Depth = 16;
			data.Samples = 44100;
			data.NextData = next;
			return data;
		}

		/// <summary>
		/// Setting up
		/// </summary>
		protected override void Setup() {

			// Getting stream
			Stream raw = GetEngine().Filesystem.GetStream(File);
			midi = new MidiFile(new ExternalFile(File, raw));

			// Setting up synth
			if (bank == null) {
				ExternalBankFile = "";
			}
			synth = new Synthesizer(44100, 2);
			synth.LoadBank(bank);

			sequencer = new MidiFileSequencer(synth);
			sequencer.LoadMidi(midi);
			sequencer.Play();
		}

		/// <summary>
		/// Instant load test
		/// </summary>
		public override bool NeedInstantLoad(string file) {
			// Midi should always be streamed
			return false;
		}

		/// <summary>
		/// Internal class for AudioSynth lib
		/// </summary>
		class ExternalFile : IResource {
			private string fileName;
			private Stream fileStream;
			public ExternalFile(string fileName, Stream fileStream) {
				this.fileName = fileName;
				this.fileStream = fileStream;
			}
			public string GetName() { return fileName; }
			public bool DeleteAllowed() { return true; }
			public bool ReadAllowed() { return true; }
			public bool WriteAllowed() { return false; }
			public void DeleteResource() { }
			public Stream OpenResourceForRead() {
				fileStream.Position = 0;
				return fileStream;
			}
			public Stream OpenResourceForWrite() { return null; }
		}

		/// <summary>
		/// Internal class for AudioSynth lib
		/// </summary>
		class InternalBankFile : IResource {
			static byte[] content;
			private string fileName;
			public InternalBankFile(byte[] data) {
				fileName = "InternalBank.sf2";
				content = data;
			}
			public string GetName() { return fileName; }
			public bool DeleteAllowed() { return true; }
			public bool ReadAllowed() { return true; }
			public bool WriteAllowed() { return false; }
			public void DeleteResource() { }
			public Stream OpenResourceForRead() {
				return new MemoryStream(content);
			}
			public Stream OpenResourceForWrite() { return null; }
		}
	}
}
