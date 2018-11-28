using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.Graphics {

	/// <summary>
	/// Shader backend system
	/// </summary>
	internal static class ShaderSystem {
		
		/// <summary>
		/// Current projection matrix
		/// </summary>
		public static Matrix4 ProjectionMatrix;

		/// <summary>
		/// Current camera matrix
		/// </summary>
		public static Matrix4 CameraMatrix;

		/// <summary>
		/// Current entity matrix
		/// </summary>
		public static Matrix4 EntityMatrix;

		/// <summary>
		/// Current texture matrix
		/// </summary>
		public static Matrix4 TextureMatrix;

		/// <summary>
		/// Is alphatest on this pass enabled
		/// </summary>
		public static bool IsAlphaTest;

		/// <summary>
		/// Checking buffer for existense
		/// </summary>
		public static void CheckVertexBuffer(ref int buffer, float[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw) {
			if (!GL.IsBuffer(buffer)) {
				buffer = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * 4), data, hint);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			}
		}

		/// <summary>
		/// Checking index buffer for existense
		/// </summary>
		public static void CheckIndexBuffer(ref int buffer, ushort[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw) {
			if (!GL.IsBuffer(buffer)) {
				buffer = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer);
				GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(data.Length * 2), data, hint);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			}
		}

		/// <summary>
		/// Checking errors
		/// </summary>
		public static void CheckErrors() {
			ErrorCode err = GL.GetError();
			if (err != ErrorCode.NoError) {
				System.Diagnostics.Debugger.Break();
			}
		}
	}
}
