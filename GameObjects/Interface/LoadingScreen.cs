using Cubed.Drivers.Rendering;
using Cubed.Gameplay;
using Cubed.Graphics;
using Cubed.UI;
using Cubed.UI.Basic;
using Cubed.UI.Themes;
using OpenTK;
using System;
using System.Drawing;

namespace Cubed.Interface {

	/// <summary>
	/// Loading screen
	/// </summary>
	public class LoadingScreen {

		/// <summary>
		/// Current parent game
		/// </summary>
		public Game Parent {
			get;
			private set;
		}
		
		/// <summary>
		/// Backdrop for map
		/// </summary>
		public Texture Backdrop {
			get {
				return backdrop;
			}
			set {
				if (backdrop != value) {
					backdrop = value;
					
				}
			}
		}

		/// <summary>
		/// Flag for "Any Key"
		/// </summary>
		public bool IsReady {
			get {
				return isReady;
			}
			set {
				if (isReady != value) {
					isReady = value;
					label.Enabled = isReady;
					spinner.Enabled = !isReady;
				}
			}
		}

		/// <summary>
		/// Background
		/// </summary>
		Texture backdrop;

		/// <summary>
		/// Flag for "Any key"
		/// </summary>
		bool isReady;

		/// <summary>
		/// Parent UI
		/// </summary>
		UserInterface ui;

		/// <summary>
		/// Spinner
		/// </summary>
		Quad spinner;

		/// <summary>
		/// Internal label
		/// </summary>
		Label label;

		/// <summary>
		/// Backdrop
		/// </summary>
		Quad background;

		/// <summary>
		/// Full black background
		/// </summary>
		Quad black;

		/// <summary>
		/// Internal angle
		/// </summary>
		float angle;

		/// <summary>
		/// Constructor for loading screen
		/// </summary>
		/// <param name="parent">Game</param>
		public LoadingScreen(Game parent) {
			Parent = parent;
			parent.Engine.MakeCurrent();

			// Loader spinner
			spinner = new Quad() {
				Position = Vector2.One * 10,
				Size = Vector2.One * 30,
				Anchor = Control.AnchorMode.BottomRight,
				Texture = new Texture(Packed.Textures.Spinner) {
					Filtering = Texture.FilterMode.Enabled,
					WrapHorizontal = Texture.WrapMode.Clamp,
					WrapVertical = Texture.WrapMode.Clamp
				},
				Enabled = !isReady
			};

			// Any key label
			label = new Label() {
				Position = Vector2.One * 10,
				Size = new Vector2(30f, 30f),
				Anchor = Control.AnchorMode.BottomRight,
				FontSize = 18f,
				Color = Color.White,
				HorizontalAlign = UserInterface.Align.End,
				VerticalAlign = UserInterface.Align.End,
				Text = "Press Any key...",
				Enabled = isReady
			};

			// Backdrop
			background = new Quad() {
				Position = Vector2.Zero,
				Size = Vector2.One * 100,
				Tint = Color.White
			};

			// Full black item
			black = new Quad() {
				Position = Vector2.Zero,
				Size = Vector2.One * 100,
				Tint = Color.Black
			};
			
			// Interface
			ui = new UserInterface();
			ui.Items.Add(black);
			ui.Items.Add(background);
			ui.Items.Add(spinner);
			ui.Items.Add(label);
		}

		/// <summary>
		/// Setting up
		/// </summary>
		public void Show() {
			Parent.Engine.Interface = ui;
		}

		/// <summary>
		/// Updating loader
		/// </summary>
		public void Update() {

			// Detecting sizes
			Vector2 size = Display.Current.Resolution;
			angle = (angle + 10) % 360;

			// Updating controls
			if (IsReady) {
				int pulse = 200 + (int)(Math.Sin(MathHelper.DegreesToRadians(angle)) * 50);
				label.Color = Color.FromArgb(pulse, Color.White);
			} else {
				float dist = 1.414213f / 2f;
				Vector2[] uv = new Vector2[4];
				for (int i = 0; i < 4; i++) {
					float deg = angle + i * 90;
					uv[i] = new Vector2((float)Math.Sin(MathHelper.DegreesToRadians(deg)) * dist + 0.5f, (float)Math.Cos(MathHelper.DegreesToRadians(deg)) * dist + 0.5f);
				}
				spinner.TexCoords = new Vector2[] {
					uv[1],
					uv[0],
					uv[2],
					uv[3]
				};
			}

			// Backdrop
			if (backdrop != null && backdrop.State == Texture.LoadingState.Complete) {
				background.Texture = backdrop;
				background.Enabled = true;
			} else {
				background.Enabled = false;
			}
			black.Size = size;
		}
	}
}
