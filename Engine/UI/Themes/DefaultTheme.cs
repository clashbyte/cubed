using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Graphics;

namespace Cubed.UI.Themes {
	
	/// <summary>
	/// Basic interface theme
	/// </summary>
	public sealed class DefaultTheme : InterfaceTheme {

		/// <summary>
		/// Font size for buttons
		/// </summary>
		private const float BUTTON_FONT_SIZE = 14;

		/// <summary>
		/// Accent color
		/// </summary>
		private readonly Color accentColor = Color.Orange;

		/// <summary>
		/// Small font
		/// </summary>
		static Font smallFont;

		/// <summary>
		/// Large font
		/// </summary>
		static Font largeFont;

		/// <summary>
		/// Icons font
		/// </summary>
		static Font iconsFont;

		/// <summary>
		/// Drawing label
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="info"></param>
		public override void DrawLabel(float x, float y, float w, float h, LabelData info) {
			
			Font f = info.Font;
			if (f == null) {
				if (info.FontSize > 15) {
					CheckLargeFont();
					f = largeFont;
				} else {
					CheckSmallFont();
					f = smallFont;
				}
			}

			float tw = f.Width(info.Text, info.FontSize);
			float th = info.FontSize;
			float dx = x;
			float dy = y;
			switch (info.HorizontalAlign) {
				case UserInterface.Align.Middle:
					dx = x + w / 2f - tw / 2f;
					break;
				case UserInterface.Align.End:
					dx = x + w - tw;
					break;
			}
			switch (info.VerticalAlign) {
				case UserInterface.Align.Middle:
					dy = y + h / 2f - th / 2f;
					break;
				case UserInterface.Align.End:
					dy = y + h - th;
					break;
			}
			f.Render(info.Text, dx, dy, info.FontSize, info.FontColor);
		}

		/// <summary>
		/// Draw button
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="info"></param>
		public override void DrawButton(float x, float y, float w, float h, ButtonData info) {
			
			// Drawing outline
			Color textColor = Color.White;
			if (info.State == ButtonRenderState.Off || info.State == ButtonRenderState.Hover) {
				textColor = info.State == ButtonRenderState.Hover ? Color.White : Color.LightGray;
				Drawing.LineBox(x, y, w, h, textColor, 2f);
			} else {
				textColor = Color.Black;
				Drawing.FilledBox(x, y, w, h, info.State == ButtonRenderState.Selected ? accentColor : Color.White);
			}

			// Making outline padding
			x += 5;
			y += 5;
			w -= 10;
			h -= 10;

			// Render icon or image
			if (info.Image != null) {
				// Render image


			} else if (info.Icon != Icons.None) {
				// Render icon
				string icon = new String(new char[] {
					(char)info.Icon	
				});
				if (info.Vertical) {

				} else {
					// Drawing icon
					CheckIconFont();
					float tw = iconsFont.Width(icon, h);
					iconsFont.Render(icon, x + h / 2f - tw / 2f + 1f, y, h * 0.9f, textColor);

					// Padding
					x += h + 5;
					w -= h + 5;
				}
			}


			// At last - rendering text
			if (info.Text != "" && info.Text != null) {
				CheckSmallFont();
				float tw = smallFont.Width(info.Text, BUTTON_FONT_SIZE);
				float th = smallFont.Height(info.Text, BUTTON_FONT_SIZE);
				smallFont.Render(info.Text, (int)(x + w / 2f - tw / 2f), (int)(y + h / 2f - th / 2f), BUTTON_FONT_SIZE, textColor);
			}
		}


		/// <summary>
		/// Check and load small font
		/// </summary>
		void CheckSmallFont() {
			if (smallFont == null) {
				smallFont = new Font(Resources.SmallFontData, new Graphics.Texture(Resources.SmallFontAtlas) {
					Filtering = Graphics.Texture.FilterMode.Enabled
				});
			}
		}

		/// <summary>
		/// Check and load large font
		/// </summary>
		void CheckLargeFont() {
			if (largeFont == null) {
				largeFont = new Font(Resources.DefaultFontData, new Graphics.Texture(Resources.DefaultFontAtlas) {
					Filtering = Graphics.Texture.FilterMode.Enabled
				});
			}
		}

		/// <summary>
		/// Check and load icons
		/// </summary>
		void CheckIconFont() {
			if (iconsFont == null) {
				iconsFont = new Font(Resources.IconsFontData, new Graphics.Texture(Resources.IconsFontAtlas) {
					Filtering = Graphics.Texture.FilterMode.Enabled
				});
			}
		}

	}
}
