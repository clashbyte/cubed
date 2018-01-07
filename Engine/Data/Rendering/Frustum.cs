using System;
using OpenTK;

namespace Cubed.Data.Rendering {

	/// <summary>
	/// Camera frustum
	/// </summary>
	internal static class Frustum {

		/// <summary>
		/// Sides data
		/// </summary>
		static Vector4[] frustum;

		/// <summary>
		/// Setting frustum up
		/// </summary>
		/// <param name="projMat">Projection matrix</param>
		/// <param name="modlMat">Modelview matrix</param>
		public static void Setup(Matrix4 projMat, Matrix4 modlMat) {

			// Cull matrix
			Matrix4 c = modlMat * projMat;
			if (frustum == null) {
				frustum = new Vector4[6];
			}
			c.Transpose();

			// Right side
			frustum[0] = new Vector4(
				c.Column0.W - c.Column0.X,
				c.Column1.W - c.Column1.X,
				c.Column2.W - c.Column2.X,
				c.Column3.W - c.Column3.X
			);

			// Left side
			frustum[1] = new Vector4(
				c.Column0.W + c.Column0.X,
				c.Column1.W + c.Column1.X,
				c.Column2.W + c.Column2.X,
				c.Column3.W + c.Column3.X
			);

			// Bottom side
			frustum[2] = new Vector4(
				c.Column0.W - c.Column0.Y,
				c.Column1.W - c.Column1.Y,
				c.Column2.W - c.Column2.Y,
				c.Column3.W - c.Column3.Y
			);

			// Top side
			frustum[3] = new Vector4(
				c.Column0.W + c.Column0.Y,
				c.Column1.W + c.Column1.Y,
				c.Column2.W + c.Column2.Y,
				c.Column3.W + c.Column3.Y
			);

			// Far side
			frustum[4] = new Vector4(
				c.Column0.W - c.Column0.Z,
				c.Column1.W - c.Column1.Z,
				c.Column2.W - c.Column2.Z,
				c.Column3.W - c.Column3.Z
			);

			// Near side
			frustum[5] = new Vector4(
				c.Column0.W + c.Column0.Z,
				c.Column1.W + c.Column1.Z,
				c.Column2.W + c.Column2.Z,
				c.Column3.W + c.Column3.Z
			);

			// Normalizing
			for (int i = 0; i < 6; i++) {
				frustum[i] /= frustum[i].Xyz.Length;
			}
		}

		/// <summary>
		/// Sphere visibility check
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="radius">Radius</param>
		public static bool Contains(Vector3 pos, float radius) {
			for (int p = 0; p < 6; p++) {
				if ((frustum[p].X * pos.X + frustum[p].Y * pos.Y + frustum[p].Z * -pos.Z + frustum[p].W) <= -radius) {
					return false;
				}
			}
			return true;
		}
	}

	/// <summary>
	/// New frustum
	/// </summary>
	public static class FrustumOld {
		private static float[] _clipMatrix = new float[16];
		private static float[,] _frustum = new float[6, 4];

		public const int A = 0;
		public const int B = 1;
		public const int C = 2;
		public const int D = 3;

		public enum ClippingPlane : int {
			Right = 0,
			Left = 1,
			Bottom = 2,
			Top = 3,
			Back = 4,
			Front = 5
		}

		/// <summary>
		/// Normalizing plane
		/// </summary>
		/// <param name="frustum"></param>
		/// <param name="side"></param>
		static void NormalizePlane(float[,] frustum, int side) {
			float magnitude = (float)Math.Sqrt((frustum[side, 0] * frustum[side, 0]) + (frustum[side, 1] * frustum[side, 1])
												+ (frustum[side, 2] * frustum[side, 2]));
			frustum[side, 0] /= magnitude;
			frustum[side, 1] /= magnitude;
			frustum[side, 2] /= magnitude;
			frustum[side, 3] /= magnitude;
		}

		/// <summary>
		/// Check for sphere in view
		/// </summary>
		/// <param name="location"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static bool Contains(Vector3 location, float radius) {
			for (int p = 0; p < 6; p++) {
				float d = _frustum[p, 0] * location.X + _frustum[p, 1] * location.Y + _frustum[p, 2] * -location.Z + _frustum[p, 3];
				if (d <= -radius) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Making frustum from camera
		/// </summary>
		/// <param name="projectionMatrix"></param>
		/// <param name="modelViewMatrix"></param>
		public static void Setup(Matrix4 projectionMatrix, Matrix4 modelViewMatrix) {
			_clipMatrix[0] = (modelViewMatrix.M11 * projectionMatrix.M11) + (modelViewMatrix.M12 * projectionMatrix.M21) + (modelViewMatrix.M13 * projectionMatrix.M31) + (modelViewMatrix.M14 * projectionMatrix.M41);
			_clipMatrix[1] = (modelViewMatrix.M11 * projectionMatrix.M12) + (modelViewMatrix.M12 * projectionMatrix.M22) + (modelViewMatrix.M13 * projectionMatrix.M32) + (modelViewMatrix.M14 * projectionMatrix.M42);
			_clipMatrix[2] = (modelViewMatrix.M11 * projectionMatrix.M13) + (modelViewMatrix.M12 * projectionMatrix.M23) + (modelViewMatrix.M13 * projectionMatrix.M33) + (modelViewMatrix.M14 * projectionMatrix.M43);
			_clipMatrix[3] = (modelViewMatrix.M11 * projectionMatrix.M14) + (modelViewMatrix.M12 * projectionMatrix.M24) + (modelViewMatrix.M13 * projectionMatrix.M34) + (modelViewMatrix.M14 * projectionMatrix.M44);

			_clipMatrix[4] = (modelViewMatrix.M21 * projectionMatrix.M11) + (modelViewMatrix.M22 * projectionMatrix.M21) + (modelViewMatrix.M23 * projectionMatrix.M31) + (modelViewMatrix.M24 * projectionMatrix.M41);
			_clipMatrix[5] = (modelViewMatrix.M21 * projectionMatrix.M12) + (modelViewMatrix.M22 * projectionMatrix.M22) + (modelViewMatrix.M23 * projectionMatrix.M32) + (modelViewMatrix.M24 * projectionMatrix.M42);
			_clipMatrix[6] = (modelViewMatrix.M21 * projectionMatrix.M13) + (modelViewMatrix.M22 * projectionMatrix.M23) + (modelViewMatrix.M23 * projectionMatrix.M33) + (modelViewMatrix.M24 * projectionMatrix.M43);
			_clipMatrix[7] = (modelViewMatrix.M21 * projectionMatrix.M14) + (modelViewMatrix.M22 * projectionMatrix.M24) + (modelViewMatrix.M23 * projectionMatrix.M34) + (modelViewMatrix.M24 * projectionMatrix.M44);

			_clipMatrix[8] = (modelViewMatrix.M31 * projectionMatrix.M11) + (modelViewMatrix.M32 * projectionMatrix.M21) + (modelViewMatrix.M33 * projectionMatrix.M31) + (modelViewMatrix.M34 * projectionMatrix.M41);
			_clipMatrix[9] = (modelViewMatrix.M31 * projectionMatrix.M12) + (modelViewMatrix.M32 * projectionMatrix.M22) + (modelViewMatrix.M33 * projectionMatrix.M32) + (modelViewMatrix.M34 * projectionMatrix.M42);
			_clipMatrix[10] = (modelViewMatrix.M31 * projectionMatrix.M13) + (modelViewMatrix.M32 * projectionMatrix.M23) + (modelViewMatrix.M33 * projectionMatrix.M33) + (modelViewMatrix.M34 * projectionMatrix.M43);
			_clipMatrix[11] = (modelViewMatrix.M31 * projectionMatrix.M14) + (modelViewMatrix.M32 * projectionMatrix.M24) + (modelViewMatrix.M33 * projectionMatrix.M34) + (modelViewMatrix.M34 * projectionMatrix.M44);

			_clipMatrix[12] = (modelViewMatrix.M41 * projectionMatrix.M11) + (modelViewMatrix.M42 * projectionMatrix.M21) + (modelViewMatrix.M43 * projectionMatrix.M31) + (modelViewMatrix.M44 * projectionMatrix.M41);
			_clipMatrix[13] = (modelViewMatrix.M41 * projectionMatrix.M12) + (modelViewMatrix.M42 * projectionMatrix.M22) + (modelViewMatrix.M43 * projectionMatrix.M32) + (modelViewMatrix.M44 * projectionMatrix.M42);
			_clipMatrix[14] = (modelViewMatrix.M41 * projectionMatrix.M13) + (modelViewMatrix.M42 * projectionMatrix.M23) + (modelViewMatrix.M43 * projectionMatrix.M33) + (modelViewMatrix.M44 * projectionMatrix.M43);
			_clipMatrix[15] = (modelViewMatrix.M41 * projectionMatrix.M14) + (modelViewMatrix.M42 * projectionMatrix.M24) + (modelViewMatrix.M43 * projectionMatrix.M34) + (modelViewMatrix.M44 * projectionMatrix.M44);

			_frustum[(int)ClippingPlane.Right, 0] = _clipMatrix[3] - _clipMatrix[0];
			_frustum[(int)ClippingPlane.Right, 1] = _clipMatrix[7] - _clipMatrix[4];
			_frustum[(int)ClippingPlane.Right, 2] = _clipMatrix[11] - _clipMatrix[8];
			_frustum[(int)ClippingPlane.Right, 3] = _clipMatrix[15] - _clipMatrix[12];
			NormalizePlane(_frustum, (int)ClippingPlane.Right);

			_frustum[(int)ClippingPlane.Left, 0] = _clipMatrix[3] + _clipMatrix[0];
			_frustum[(int)ClippingPlane.Left, 1] = _clipMatrix[7] + _clipMatrix[4];
			_frustum[(int)ClippingPlane.Left, 2] = _clipMatrix[11] + _clipMatrix[8];
			_frustum[(int)ClippingPlane.Left, 3] = _clipMatrix[15] + _clipMatrix[12];
			NormalizePlane(_frustum, (int)ClippingPlane.Left);

			_frustum[(int)ClippingPlane.Bottom, 0] = _clipMatrix[3] + _clipMatrix[1];
			_frustum[(int)ClippingPlane.Bottom, 1] = _clipMatrix[7] + _clipMatrix[5];
			_frustum[(int)ClippingPlane.Bottom, 2] = _clipMatrix[11] + _clipMatrix[9];
			_frustum[(int)ClippingPlane.Bottom, 3] = _clipMatrix[15] + _clipMatrix[13];
			NormalizePlane(_frustum, (int)ClippingPlane.Bottom);

			_frustum[(int)ClippingPlane.Top, 0] = _clipMatrix[3] - _clipMatrix[1];
			_frustum[(int)ClippingPlane.Top, 1] = _clipMatrix[7] - _clipMatrix[5];
			_frustum[(int)ClippingPlane.Top, 2] = _clipMatrix[11] - _clipMatrix[9];
			_frustum[(int)ClippingPlane.Top, 3] = _clipMatrix[15] - _clipMatrix[13];
			NormalizePlane(_frustum, (int)ClippingPlane.Top);

			_frustum[(int)ClippingPlane.Back, 0] = _clipMatrix[3] - _clipMatrix[2];
			_frustum[(int)ClippingPlane.Back, 1] = _clipMatrix[7] - _clipMatrix[6];
			_frustum[(int)ClippingPlane.Back, 2] = _clipMatrix[11] - _clipMatrix[10];
			_frustum[(int)ClippingPlane.Back, 3] = _clipMatrix[15] - _clipMatrix[14];
			NormalizePlane(_frustum, (int)ClippingPlane.Back);

			_frustum[(int)ClippingPlane.Front, 0] = _clipMatrix[3] + _clipMatrix[2];
			_frustum[(int)ClippingPlane.Front, 1] = _clipMatrix[7] + _clipMatrix[6];
			_frustum[(int)ClippingPlane.Front, 2] = _clipMatrix[11] + _clipMatrix[10];
			_frustum[(int)ClippingPlane.Front, 3] = _clipMatrix[15] + _clipMatrix[14];
			NormalizePlane(_frustum, (int)ClippingPlane.Front);
		}
	}
}
