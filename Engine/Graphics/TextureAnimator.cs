using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Graphics {
	
	/// <summary>
	/// Texture animator
	/// </summary>
	public class TextureAnimator {

		/// <summary>
		/// Current texture frame
		/// </summary>
		public int CurrentFrame {
			get {
				return currentFrame;
			}
		}

		/// <summary>
		/// Starting time
		/// </summary>
		long startTime = 0;

		/// <summary>
		/// Playing speed
		/// </summary>
		bool playing = false;

		/// <summary>
		/// Playing speed
		/// </summary>
		float speed = 0f;

		/// <summary>
		/// Current frame
		/// </summary>
		int currentFrame = 0;

		/// <summary>
		/// Parent texture
		/// </summary>
		Texture parent;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="p">Parent texture</param>
		internal TextureAnimator(Texture p) {

		}

		/// <summary>
		/// Updating animations and texture
		/// </summary>
		internal void Update() {

		}
		
	}
}
