using System.Drawing;
using Cubed.Graphics;

namespace Cubed.Data.Shaders {
	
	/// <summary>
	/// Map shadowing shader
	/// </summary>
	internal class MapShadowShader : ShaderBase {

		/// <summary>
		/// Shader singletone
		/// </summary>
		public static MapShadowShader Shader {
			get {
				if (sh == null) {
					sh = new MapShadowShader();
				}
				return sh;
			}
		}
		static MapShadowShader sh;
		
		/// <summary>
		/// Фрагментный шейдер
		/// </summary>
		protected override string FragmentProgram {
			get {
				return ShaderSources.MapShadowFragment;
			}
		}

		/// <summary>
		/// Вершинный шейдер
		/// </summary>
		protected override string VertexProgram {
			get {
				return ShaderSources.MapShadowVertex;
			}
		}

		/// <summary>
		/// Расположение буффера вершин
		/// </summary>
		public int VertexBufferLocation {
			get {
				return vertexAttrib.Handle;
			}
		}

		// Скрытые параметры
		VertexAttribute vertexAttrib;

		// Конструктор
		protected MapShadowShader() : base() {
			if (vertexAttrib == null) {
				vertexAttrib = new VertexAttribute("inPosition");
				attribs.Add(vertexAttrib);
			}
		}

		/// <summary>
		/// Установка шейдера
		/// </summary>
		public override void Bind() {
			base.Bind();
		}

	}
}
