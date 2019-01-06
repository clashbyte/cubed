using Cubed.Graphics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.World {

	/// <summary>
	/// Class for creating world and lightmap mesh
	/// </summary>
	internal static class MapTriangulator {

		/// <summary>
		/// Texture pixels per single block
		/// </summary>
		public const int TEXELS_PER_BLOCK = 16;

		/// <summary>
		/// Calculating light dimensions
		/// </summary>
		public static void CalculateLightCoords(IEnumerable<FaceGroup> groups, out int texWidth, out int texHeight) {

			// Calculating total
			int total = 0;
			foreach (FaceGroup fg in groups) {
				total += fg.Faces.Count;
			}

			// Calculating size
			int step = TEXELS_PER_BLOCK;
			int blocksX = 0;
			int blocksY = 0;
			while (true) {
				int cx = step / TEXELS_PER_BLOCK;
				int cy = (int)Math.Ceiling((float)total / (float)cx);
				if (cy <= cx) {
					blocksX = cx;
					blocksY = cy;
					break;
				}
				step *= 2;
			}

			// Storing sizes
			texWidth = blocksX * TEXELS_PER_BLOCK;
			texHeight = MathHelper.NextPowerOfTwo(blocksY * TEXELS_PER_BLOCK);
			
			// Division
			float mulX = (float)TEXELS_PER_BLOCK / (float)texWidth;
			float mulY = (float)TEXELS_PER_BLOCK / (float)texHeight;

			float pX = 0;
			float pY = 0;
			foreach (FaceGroup fg in groups) {
				foreach (Face f in fg.Faces) {
					for (int i = 0; i < f.LightCoords.Length; i+=2) {
						f.LightCoords[i + 0] = (f.LightCoords[i + 0] + pX) * mulX;
						f.LightCoords[i + 1] = (f.LightCoords[i + 1] + pY) * mulY;
					}
					pX++;
					if (pX >= blocksX) {
						pX = 0;
						pY++;
					}
				}
			}
		}

		/// <summary>
		/// Calculating merged group
		/// </summary>
		/// <returns>Number of indices</returns>
		public static int TriangulateMerged(IEnumerable<FaceGroup> groups, out float[] coords, out float[] texCoords, out float[] lightCoords, out float[] normals, out ushort[] indices) {

			// Making arrays
			List<float> lCoords = new List<float>();
			List<float> lTexCoords = new List<float>();
			List<float> lLightCoords = new List<float>();
			List<float> lNormals = new List<float>();
			List<ushort> lIndices = new List<ushort>();

			// Pushing faces
			int idx = 0;
			foreach (FaceGroup faceGroup in groups) {
				foreach (Face face in faceGroup.Faces) {

					float[] normalBuffer = new float[face.Coords.Length];
					for (int i = 0; i < face.Indices.Length; i += 3) {
						int px1 = face.Indices[i + 0] * 3;
						int px2 = face.Indices[i + 1] * 3;
						int px3 = face.Indices[i + 2] * 3;

						Vector3 vp1 = new Vector3(face.Coords[px1 + 0], face.Coords[px1 + 1], face.Coords[px1 + 2]);
						Vector3 vp2 = new Vector3(face.Coords[px2 + 0], face.Coords[px2 + 1], face.Coords[px2 + 2]);
						Vector3 vp3 = new Vector3(face.Coords[px3 + 0], face.Coords[px3 + 1], face.Coords[px3 + 2]);
						Vector3 normal = Vector3.Cross(vp2 - vp1, vp3 - vp1).Normalized();

						normalBuffer[px1 + 0] += normal.X;
						normalBuffer[px1 + 1] += normal.Y;
						normalBuffer[px1 + 2] += normal.Z;
						normalBuffer[px2 + 0] += normal.X;
						normalBuffer[px2 + 1] += normal.Y;
						normalBuffer[px2 + 2] += normal.Z;
						normalBuffer[px3 + 0] += normal.X;
						normalBuffer[px3 + 1] += normal.Y;
						normalBuffer[px3 + 2] += normal.Z;
					}
					for (int i = 0; i < normalBuffer.Length; i += 3) {
						float length = (float)Math.Sqrt(Math.Pow(normalBuffer[i], 2) + Math.Pow(normalBuffer[i + 1], 2) + Math.Pow(normalBuffer[i + 2], 2));
						if (length != 0) {
							normalBuffer[i + 0] /= length;
							normalBuffer[i + 1] /= length;
							normalBuffer[i + 2] /= length;
						}
					}

					lCoords.AddRange(face.Coords);
					lTexCoords.AddRange(face.TexCoords);
					lLightCoords.AddRange(face.LightCoords);
					lNormals.AddRange(normalBuffer);
					foreach (ushort i in face.Indices) {
						lIndices.Add((ushort)(idx + i));
					}
					idx += face.Coords.Length / 3;
				}
			}
			
			// Saving
			coords = lCoords.ToArray();
			texCoords = lTexCoords.ToArray();
			lightCoords = lLightCoords.ToArray();
			normals = lNormals.ToArray();
			indices = lIndices.ToArray();
			return indices.Length;
		}

		/// <summary>
		/// Faces group
		/// </summary>
		public class FaceGroup {

			/// <summary>
			/// Faces list
			/// </summary>
			public List<Face> Faces;

			/// <summary>
			/// Face texture
			/// </summary>
			public Texture Texture;

			/// <summary>
			/// Constructor
			/// </summary>
			public FaceGroup() {
				Faces = new List<Face>();
			}

			/// <summary>
			/// Converting face group to vertex array
			/// </summary>
			/// <returns>Number of indices</returns>
			public int Triangulate(out float[] coords, out float[] texCoords, out float[] lightCoords, out float[] normals, out ushort[] indices) {

				// Making arrays
				List<float> lCoords = new List<float>();
				List<float> lTexCoords = new List<float>();
				List<float> lLightCoords = new List<float>();
				List<float> lNormals = new List<float>();
				List<ushort> lIndices = new List<ushort>();

				// Pushing faces
				int idx = 0;
				foreach (Face face in Faces) {
					float[] normalBuffer = new float[face.Coords.Length];
					for (int i = 0; i < face.Indices.Length; i+=3) {
						int px1 = face.Indices[i + 0] * 3;
						int px2 = face.Indices[i + 1] * 3;
						int px3 = face.Indices[i + 2] * 3;

						Vector3 vp1 = new Vector3(face.Coords[px1 + 0], face.Coords[px1 + 1], face.Coords[px1 + 2]);
						Vector3 vp2 = new Vector3(face.Coords[px2 + 0], face.Coords[px2 + 1], face.Coords[px2 + 2]);
						Vector3 vp3 = new Vector3(face.Coords[px3 + 0], face.Coords[px3 + 1], face.Coords[px3 + 2]);
						Vector3 normal = Vector3.Cross(vp2 - vp1, vp3 - vp1).Normalized();

						normalBuffer[px1 + 0] += normal.X;
						normalBuffer[px1 + 1] += normal.Y;
						normalBuffer[px1 + 2] += normal.Z;
						normalBuffer[px2 + 0] += normal.X;
						normalBuffer[px2 + 1] += normal.Y;
						normalBuffer[px2 + 2] += normal.Z;
						normalBuffer[px3 + 0] += normal.X;
						normalBuffer[px3 + 1] += normal.Y;
						normalBuffer[px3 + 2] += normal.Z;
					}
					for (int i = 0; i < normalBuffer.Length; i+=3) {
						float length = (float)Math.Sqrt(Math.Pow(normalBuffer[i], 2) + Math.Pow(normalBuffer[i + 1], 2) + Math.Pow(normalBuffer[i + 2], 2));
						if (length != 0) {
							normalBuffer[i + 0] /= length;
							normalBuffer[i + 1] /= length;
							normalBuffer[i + 2] /= length;
						}
					}
					
					lCoords.AddRange(face.Coords);
					lTexCoords.AddRange(face.TexCoords);
					lLightCoords.AddRange(face.LightCoords);
					lNormals.AddRange(normalBuffer);
					foreach (ushort i in face.Indices) {
						lIndices.Add((ushort)(idx + i));
					}
					idx += face.Coords.Length / 3;
				}

				// Saving
				coords = lCoords.ToArray();
				texCoords = lTexCoords.ToArray();
				lightCoords = lLightCoords.ToArray();
				normals = lNormals.ToArray();
				indices = lIndices.ToArray();
				return indices.Length;
			}

		}

		/// <summary>
		/// Single face
		/// </summary>
		public class Face {

			/// <summary>
			/// Real coordinates
			/// </summary>
			public float[] Coords;

			/// <summary>
			/// Texture coordinates
			/// </summary>
			public float[] TexCoords;

			/// <summary>
			/// Coordinates for light
			/// </summary>
			public float[] LightCoords;

			/// <summary>
			/// Normal directions
			/// </summary>
			public float[] Normals;

			/// <summary>
			/// Indices for this face
			/// </summary>
			public ushort[] Indices;

		}

	}
}
