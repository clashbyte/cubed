using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Data.Rendering;
using Cubed.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.World {

	/// <summary>
	/// Camera for world rendering
	/// </summary>
	public sealed class Camera : Entity {

		/// <summary>
		/// Ortho size
		/// </summary>
		public const float ORTHO_SIZE = 10f;

		/// <summary>
		/// Projection matrix
		/// </summary>
		Matrix4 proj = Matrix4.Identity;

		/// <summary>
		/// Skybox matrix
		/// </summary>
		Matrix4 skymat = Matrix4.Identity;

		/// <summary>
		/// Screen size
		/// </summary>
		Vector2 size = Vector2.One;

		/// <summary>
		/// Camera range
		/// </summary>
		Vector2 range = new Vector2(0.03f, 300f);

		/// <summary>
		/// Projection mode
		/// </summary>
		ProjectionMode mode = ProjectionMode.Perspective;

		/// <summary>
		/// Width aspect
		/// </summary>
		float aspect = 1f;

		/// <summary>
		/// Zooming
		/// </summary>
		float zoom = 1f;

		/// <summary>
		/// Dirty matrices flag
		/// </summary>
		bool needMatrix = false;

		/// <summary>
		/// Camera size
		/// </summary>
		public Vector2 Size {
			get {
				return new Vector2(size.X, size.Y);
			}
			set {
				size = new Vector2(value.X, value.Y);
				needMatrix = true;
			}
		}

		/// <summary>
		/// Camera range
		/// </summary>
		public Vector2 Range {
			get {
				return new Vector2(range.X, range.Y);
			}
			set {
				range = new Vector2(value.X, value.Y);
				needMatrix = true;
			}
		}

		/// <summary>
		/// Camera zoom
		/// </summary>
		public float Zoom {
			get {
				return zoom;
			}
			set {
				zoom = value;
				if (zoom < 0.00001f) {
					zoom = 0.00001f;
				}
				needMatrix = true;
			}
		}

		/// <summary>
		/// Camera projection mode
		/// </summary>
		public ProjectionMode Projection {
			get {
				return mode;
			}
			set {
				mode = value;
				needMatrix = true;
			}
		}

		/// <summary>
		/// Current pass camera
		/// </summary>
		static internal Camera Current {
			get;
			private set;
		}

		/// <summary>
		/// Current inverted matrix
		/// </summary>
		public Matrix4 InvertedMatrix {
			get {
				return invmat;
			}
		}

		/// <summary>
		/// Converting from world to screen coords
		/// </summary>
		/// <param name="point">Point in global coords</param>
		/// <returns>Point in screen coords</returns>
		public Vector2 PointToScreen(Vector3 point) {
			if (needMatrix) {
				RebuildProjection();
			}
			Vector4 tv = Vector4.Transform(new Vector4(point.X, point.Y, -point.Z, 1f), invmat * proj);
			if (tv.W < -float.Epsilon || tv.W > float.Epsilon) {
				tv /= tv.W;
			}
			return new Vector2(
				(tv.X + 1f) * 0.5f * size.X,
				(-tv.Y + 1f) * 0.5f * size.Y
			);
		}

		/// <summary>
		/// Converting screen point to world
		/// </summary>
		/// <param name="sx">X</param>
		/// <param name="sy">Y</param>
		/// <returns>Point in world coords</returns>
		public Vector3 ScreenToPoint(float sx, float sy, float depth = 0f) {
			return ScreenToPoint(new Vector3(sx, sy, depth));
		}

		/// <summary>
		/// Handle free look controls
		/// </summary>
		/// <param name="movement">Movement vectors</param>
		/// <param name="looks">Looking vector</param>
		public void FreeLookControls(Vector3 movement, Vector3 looks) {
			Position += VectorToWorld(movement);
			Vector3 ang = LocalAngles + looks;
			ang.X = Math.Sign(ang.X) * Math.Min(Math.Abs(ang.X), 90);
			ang.Y = ang.Y % 360f;
			ang.Z = 0;
			LocalAngles = ang;
		}

		/// <summary>
		/// Converting screen point to world
		/// </summary>
		/// <param name="p">Point</param>
		/// <returns>Point in world coords</returns>
		internal Vector3 ScreenToPoint(Vector3 p) {
			if (needMatrix) {
				RebuildProjection();
			}
			Vector4 vec;
			vec.X = 2.0f * p.X / (float)size.X - 1;
			vec.Y = -(2.0f * p.Y / (float)size.Y - 1);
			vec.Z = p.Z;
			vec.W = 1.0f;
			vec = Vector4.Transform(vec, (invmat * proj).Inverted());
			if (vec.W > float.Epsilon || vec.W < float.Epsilon) {
				vec.X /= vec.W;
				vec.Y /= vec.W;
				vec.Z /= vec.W;
			}
			return new Vector3(vec.X, vec.Y, -vec.Z);
		}

		/// <summary>
		/// Setting camera up before rendering
		/// </summary>
		internal void Setup() {
			if (needMatrix) {
				RebuildProjection();
			}

			// Data loading
			GL.Viewport(0, 0, (int)size.X, (int)size.Y);
			if (Caps.ShaderPipeline) {
				ShaderSystem.ProjectionMatrix = proj;
			} else {
				GL.MatrixMode(MatrixMode.Projection);
				GL.LoadMatrix(ref proj);
			}
			Current = this;
		}

		/// <summary>
		/// Sending sky matrix
		/// </summary>
		internal void LoadSkyMatrix() {
			if (Caps.ShaderPipeline) {
				ShaderSystem.CameraMatrix = skymat;
			} else {
				GL.MatrixMode(MatrixMode.Modelview);
				GL.LoadMatrix(ref skymat);
			}
		}

		/// <summary>
		/// Send current camera matrix
		/// </summary>
		internal void LoadMatrix() {
			if (Caps.ShaderPipeline) {
				ShaderSystem.CameraMatrix = invmat;
			} else {
				GL.MatrixMode(MatrixMode.Modelview);
				GL.LoadMatrix(ref invmat);
			}

			// Настройка фрустума
			Frustum.Setup(proj, invmat);
		}

		/// <summary>
		/// Rebuild projection matrix
		/// </summary>
		void RebuildProjection() {
			aspect = size.X / size.Y;
			switch (mode) {
				case ProjectionMode.Ortho:
					proj = Matrix4.CreateOrthographic(aspect * ORTHO_SIZE * (1f / zoom), ORTHO_SIZE * (1f / zoom), range.X, range.Y);
					break;
				case ProjectionMode.CanvasOrtho:
					proj = Matrix4.CreateOrthographicOffCenter(0, size.X, size.Y, 0, range.X, range.Y);
					break;
				case ProjectionMode.Perspective:
					proj = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 3f * (1f / zoom), aspect, range.X, range.Y);
					break;
			}
			needMatrix = false;
		}

		/// <summary>
		/// Rebuild position matrix
		/// </summary>
		protected override void RebuildMatrix() {
			base.RebuildMatrix();
			skymat = invmat.ClearTranslation();
		}

		/// <summary>
		/// Camera projection modes
		/// </summary>
		public enum ProjectionMode {

			/// <summary>
			/// Orthogonal
			/// </summary>
			Ortho,

			/// <summary>
			/// Orthogonal with 1:1 pixel accuracy
			/// </summary>
			CanvasOrtho,

			/// <summary>
			/// Perspective (default)
			/// </summary>
			Perspective

		}
	}
}
