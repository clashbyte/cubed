using System.Drawing;
using Cubed.Graphics;

namespace Cubed.Data.Shaders {

	/// <summary>
	/// Shader for sprite
	/// </summary>
	internal class SpriteShader : ShaderBase {

		/// <summary>
		/// Shader singletone
		/// </summary>
		public static SpriteShader Shader {
			get {
				if (sh == null) {
					sh = new SpriteShader();
				}
				return sh;
			}
		}
		static SpriteShader sh;
		
		/// <summary>
		/// Фрагментный шейдер
		/// </summary>
		protected override string FragmentProgram {
			get {
				return ShaderSources.SpriteFragment;
			}
		}

		/// <summary>
		/// Вершинный шейдер
		/// </summary>
		protected override string VertexProgram {
			get {
				return ShaderSources.SpriteVertex;
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

		/// <summary>
		/// Расположение буффера текстурных координат
		/// </summary>
		public int TexCoordBufferLocation {
			get {
				return texCoordAttrib.Handle;
			}
		}

		/// <summary>
		/// Основной цвет
		/// </summary>
		public Color DiffuseColor {
			get {
				return diffuseColor.Color;
			}
			set {
				diffuseColor.Color = value;
			}
		}

		/// <summary>
		/// Основной цвет
		/// </summary>
		public bool AlphaTestPass {
			get {
				return alphaPass.Value;
			}
			set {
				alphaPass.Value = value;
			}
		}

		// Скрытые параметры
		TextureUniform textureUniform;
		ColorUniform diffuseColor;
		BoolUniform alphaPass;
		MatrixUniform textureMatrix;
		VertexAttribute vertexAttrib;
		VertexAttribute texCoordAttrib;

		// Конструктор
		protected SpriteShader()
			: base() {
			if (vertexAttrib == null) {
				vertexAttrib = new VertexAttribute("inPosition");
				attribs.Add(vertexAttrib);
			}
			if (texCoordAttrib == null) {
				texCoordAttrib = new VertexAttribute("inTexCoord");
				attribs.Add(texCoordAttrib);
			}
			if (textureUniform == null) {
				textureUniform = new TextureUniform("texture");
				uniforms.Add(textureUniform);
			}
			if (diffuseColor == null) {
				diffuseColor = new ColorUniform("diffuseColor");
				uniforms.Add(diffuseColor);
			}
			if (alphaPass == null) {
				alphaPass = new BoolUniform("discardPass");
				uniforms.Add(alphaPass);
			}
			if (textureMatrix == null) {
				textureMatrix = new MatrixUniform("textureMatrix");
				uniforms.Add(textureMatrix);
			}
		}

		/// <summary>
		/// Установка шейдера
		/// </summary>
		public override void Bind() {
			textureMatrix.Matrix = ShaderSystem.TextureMatrix;
			alphaPass.Value = ShaderSystem.IsAlphaTest;
			base.Bind();
		}
	}
}
