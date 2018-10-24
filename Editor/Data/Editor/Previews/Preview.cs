using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using Cubed.UI.Graphics;
using Cubed.Data.Projects;
using System.Drawing;
using Cubed.Forms.Resources;

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
		/// Is this preview animated
		/// </summary>
		public bool HasAnimation {
			get {
				return animated;
			}
		}

		/// <summary>
		/// Returning all frames
		/// </summary>
		public Frame[] AnimatedIcon {
			get {
				return animFrames;
			}
		}

		/// <summary>
		/// File path
		/// </summary>
		public string File {
			get;
			private set;
		}

		/// <summary>
		/// Animation flag
		/// </summary>
		bool animated;

		/// <summary>
		/// Animation frames
		/// </summary>
		Frame[] animFrames;

		/// <summary>
		/// Generator
		/// </summary>
		PreviewGenerator gen;

		/// <summary>
		/// Constructor
		/// </summary>
		Preview(Project.Entry entry) {
			File = entry.FullPath;
			Refresh(entry);
		}

		/// <summary>
		/// Creating from image
		/// </summary>
		/// <param name="src">Raw image</param>
		Preview(Image src) {
			File = "";
			SubIcon = null;
			ShowSubIcon = false;
			animated = false;

			Bitmap m = new Bitmap(128, 128);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(m)) {
				g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				if (src.Width < 128 && src.Height < 128) {
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				} else {
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				}

				float deltaX = 128f / (float)src.Width;
				float deltaY = 128f / (float)src.Height;
				float delta = deltaX < deltaY ? deltaX : deltaY;
				float cw = (float)src.Width * delta;
				float ch = (float)src.Height * delta;
				g.DrawImage(src, 64f - cw / 2f, 64f - ch / 2f, cw, ch);
			}
			Icon = new UIIcon(m);
		}

		/// <summary>
		/// Refresh preview
		/// </summary>
		void Refresh(Project.Entry entry) {
			SubIcon = FileTypeManager.GetIcon(entry);
			Icon = SubIcon;
			gen = FileTypeManager.GetPreviewGenerator(entry);
			if (gen != null) {
				pending.Enqueue(this);
			} else {
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
		/// Creating preview for raw image
		/// </summary>
		/// <returns>Preview data</returns>
		public static Preview GetForRawImage(Image image) {
			return new Preview(image);
		}

		/// <summary>
		/// Update single entry
		/// </summary>
		/// <param name="entry">Entry to update</param>
		public static void Update(Project.Entry entry) {
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
			if (cache.ContainsKey(entry.FullPath.ToLower())) {
				cache[entry.FullPath.ToLower()].Refresh(entry);
			}
		}

		/// <summary>
		/// Remove entry from cache
		/// </summary>
		/// <param name="entry"></param>
		public static void Remove(Project.Entry entry) {

			// Checking cache
			if (cache.ContainsKey(entry.FullPath.ToLower())) {
				cache.Remove(entry.FullPath.ToLower());
			}

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
					if (p.gen.Result != null || p.gen.AnimatedResult != null) {
						if (p.gen.AnimatedResult != null) {
							p.animated = true;
							p.animFrames = new Frame[p.gen.AnimatedResult.Length];
							for (int f = 0; f < p.animFrames.Length; f++) {
								Frame frm = new Frame();
								frm.Icon = new UIIcon(p.gen.AnimatedResult[f].Frame);
								frm.Delay = p.gen.AnimatedResult[f].Delay;
								p.animFrames[f] = frm;
							}
							p.Icon = new UIIcon(p.gen.AnimatedResult[0].Frame);
						} else {
							p.Icon = new UIIcon(p.gen.Result);
						}
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

		/// <summary>
		/// Structure for single scan
		/// </summary>
		public class Frame {
			public UIIcon Icon;
			public int Delay;
		}
	}
}
