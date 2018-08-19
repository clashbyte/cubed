using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Projects;
using Cubed.Forms.Common;
using Cubed.Graphics;

namespace Cubed.Forms.Editors.Map {
	partial class MapEditor {

		/// <summary>
		/// Texture cache
		/// </summary>
		Dictionary<Project.Entry, Texture> cachedTextures;

		/// <summary>
		/// Animators
		/// </summary>
		Dictionary<Texture, TextureAnimator> textureAnimators;

		/// <summary>
		/// Reading current texture
		/// </summary>
		/// <param name="empty">Empty placeholder texture</param>
		/// <returns>Texture</returns>
		Texture GetCurrentTexture(Texture empty) {
			if (MainForm.SelectedEntry is Project.Entry) {
				Texture tx = null;
				Project.Entry pe = MainForm.SelectedEntry as Project.Entry;
				if (cachedTextures.ContainsKey(pe)) {
					return cachedTextures[pe];
				}
				string ext = System.IO.Path.GetExtension(pe.Name).ToLower();
				switch (ext) {

					case ".png":
					case ".jpg":
					case ".bmp":
					case ".jpeg":
						tx = new Texture(pe.Path, Texture.LoadingMode.Queued);
						break;

					case ".gif":
						tx = new Texture(pe.Path, Texture.LoadingMode.Queued);
						if (!textureAnimators.ContainsKey(tx)) {
							textureAnimators.Add(tx, new TextureAnimator(tx));
						}
						break;



				}
				if (tx != null) {
					cachedTextures.Add(pe, tx);
					return tx;
				}
			}
			return empty;
		}

		/// <summary>
		/// Updating animators
		/// </summary>
		void UpdateTextureAnimators() {
			foreach (TextureAnimator ta in textureAnimators.Values) {
				ta.Update();
			}
		}

		/// <summary>
		/// Simple texture animator
		/// </summary>
		class TextureAnimator {

			/// <summary>
			/// Current texture
			/// </summary>
			public Texture Tex {
				get;
				private set;
			}

			/// <summary>
			/// Starting anim time
			/// </summary>
			long startTime;

			/// <summary>
			/// Current frame
			/// </summary>
			int currentFrame;

			/// <summary>
			/// Constuctor
			/// </summary>
			public TextureAnimator(Texture tex) {
				Tex = tex;
				currentFrame = 0;
				startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				Update();
			}

			/// <summary>
			/// Updating logics
			/// </summary>
			public void Update() {
				if (Tex.TotalFrames > 1) {
					long time = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - startTime) % Tex.AnimationLength;
					for (int i = 0; i < Tex.TotalFrames; i++) {
						if (time <= Tex.GetFrameDelay(i)) {
							Tex.CurrentFrame = i;
							break;
						}
						time -= Tex.GetFrameDelay(i);
					}
				}
			}
		}

	}
}
