using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubed.Graphics {
	
	/// <summary>
	/// Class for mesh handling
	/// </summary>
	internal class MeshData {

		/// <summary>
		/// Vertex data
		/// </summary>
		public float[] Vertices {
			get {
				return vertices;
			}
			set {
				vertices = value;
				dirty = true;
			}
		}


		public float[] Normals {
			get {
				return normals;
			}
			set {
				
			}
		}


		// Raw array data
		float[] vertices;
		float[] normals;
		float[] texCoords;
		ushort[] indices; 

		// Buffers
		int vertexBuffer;
		int normalBuffer;
		int texCoordBuffer;
		int indexBuffer;

		// Flag for rebuild
		bool dirty;


		// Count of primitives
		public MeshData() {

		}

	}
}
