using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Input;
using Cubed.UI.Themes;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.UI {

	/// <summary>
	/// User interface (host for controls)
	/// </summary>
	public class UserInterface {

		/// <summary>
		/// Items of the interface
		/// </summary>
		public List<Control> Items {
			get;
			private set;
		}

		/// <summary>
		/// Interface theme
		/// </summary>
		public InterfaceTheme Theme {
			get;
			set;
		}

		/// <summary>
		/// Screen position
		/// </summary>
		Vector2 screenPos;

		/// <summary>
		/// Screen size
		/// </summary>
		Vector2 screenSize;

		/// <summary>
		/// User interface constructor
		/// </summary>
		public UserInterface() {
			Items = new List<Control>();
			Theme = new DefaultTheme();
		}



		/// <summary>
		/// Setup for rendering
		/// </summary>
		/// <param name="position">Position</param>
		/// <param name="size">Size</param>
		internal void Setup(Vector2 position, Vector2 size) {
			
			// Creating matrices
			screenPos = position;
			screenSize = size;
			Matrix4 proj = Matrix4.CreateOrthographicOffCenter(position.X, position.X + size.X, position.Y + size.Y, position.Y, -1, 3);
			
			// Setting up projection
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref proj);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

		}

		/// <summary>
		/// Logical update of interface
		/// </summary>
		internal void Update(float delta, InputState state) {
			foreach (Control c in Items) {
				if (c.Enabled) {
					c.RebuildPositions(screenPos, screenSize);
					c.Update(state, delta);
				}
			}
		}
		
		/// <summary>
		/// Render whole interface
		/// </summary>
		internal void Render() {
			InterfaceTheme.Current = Theme;
			foreach (Control c in Items) {
				if (c.Enabled) {
					c.Render();
				}
			}
		}

		/// <summary>
		/// Resizing of the host window
		/// </summary>
		/// <param name="newSize"></param>
		internal void Resize(Vector2 newSize) {

		}


		/// <summary>
		/// Text align
		/// </summary>
		public enum Align {
			Start,
			Middle,
			End
		}

	}
}
