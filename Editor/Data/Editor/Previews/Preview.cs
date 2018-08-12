using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using Cubed.UI.Graphics;
using Cubed.Data.Projects;

namespace Cubed.Data.Editor.Previews {
	
	/// <summary>
	/// Main preview for file
	/// </summary>
	public class Preview {

		/// <summary>
		/// Event for ready previews
		/// </summary>
		public static event EventHandler<PreviewEventArgs> PreviewReady;

		/// <summary>
		/// Number of images to process in main thread
		/// </summary>
		const int READY_QUOTA = 5;

		/// <summary>
		/// Number of generators
		/// </summary>
		const int WORKER_QUOTA = 4;

		/// <summary>
		/// Pending previews
		/// </summary>
		static ConcurrentQueue<Preview> pending = new ConcurrentQueue<Preview>();

		/// <summary>
		/// Complete previews
		/// </summary>
		static ConcurrentQueue<Preview> ready = new ConcurrentQueue<Preview>();

		/// <summary>
		/// Generation threads
		/// </summary>
		static Thread[] threads;

		/// <summary>
		/// Previews cache
		/// </summary>
		static Dictionary<string, Preview> cache = new Dictionary<string,Preview>();

		/// <summary>
		/// Core icon
		/// </summary>
		public UIIcon Icon {
			get;
			private set;
		}

		/// <summary>
		/// Sub icon
		/// </summary>
		public UIIcon SubIcon {
			get;
			private set;
		}

		/// <summary>
		/// Display sub icon
		/// </summary>
		public bool ShowSubIcon {
			get;
			protected set;
		}

		/// <summary>
		/// File path
		/// </summary>
		public string File {
			get;
			private set;
		}

		/// <summary>
		/// Generator
		/// </summary>
		PreviewGenerator gen;

		/// <summary>
		/// Constructor
		/// </summary>
		Preview(Project.Entry entry) {
			File = entry.FullPath;
			SubIcon = FileTypeManager.GetIcon(entry);
			gen = FileTypeManager.GetPreviewGenerator(entry);
			if (gen != null) {
				Icon = SubIcon;
				pending.Enqueue(this);
			} else {
				Icon = SubIcon;
				SubIcon = null;
				ShowSubIcon = false;
			}
		}

		/// <summary>
		/// Getting preview for file
		/// </summary>
		/// <param name="file">File path</param>
		/// <returns>File data</returns>
		public static Preview Get(Project.Entry entry) {
			
			// Making workers
			if (threads == null) {
				threads = new Thread[WORKER_QUOTA];
				for (int i = 0; i < WORKER_QUOTA; i++) {
					Thread t = new Thread(BackgroundWork);
					t.IsBackground = true;
					t.Priority = ThreadPriority.BelowNormal;
					t.Start();
					threads[i] = t;
				}
			}

			// Checking cache
			if (!cache.ContainsKey(entry.FullPath.ToLower())) {
				cache.Add(entry.FullPath.ToLower(), new Preview(entry));
			}
			return cache[entry.FullPath.ToLower()];
		}

		/// <summary>
		/// Handle pending previews
		/// </summary>
		public static void UpdatePending() {
			for (int i = 0; i < READY_QUOTA; i++) {
				if (ready.IsEmpty) {
					break;
				}
				Preview p = null;
				if (ready.TryDequeue(out p)) {
					if (p.gen.Result != null) {
						p.Icon = new UIIcon(p.gen.Result);
						p.ShowSubIcon = p.gen.ShowSubIcon;
						if (PreviewReady != null) {
							PreviewReady(null, new PreviewEventArgs(p));
						}
					}
				}
			}
		}

		/// <summary>
		/// Working in background
		/// </summary>
		static void BackgroundWork() {
			while (true) {
				if (pending.Count > 0) {
					Preview p = null;
					if (pending.TryDequeue(out p)) {
						p.gen.Generate();
						ready.Enqueue(p);
					}
					Thread.Sleep(0);
				} else {
					Thread.Sleep(1);
				}
			}
		}

		/// <summary>
		/// Event args for preview event
		/// </summary>
		public class PreviewEventArgs : EventArgs {

			/// <summary>
			/// Preview data
			/// </summary>
			public Preview Preview;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="p">Preview</param>
			public PreviewEventArgs(Preview p) {
				Preview = p;
			}
		}
	}
}
