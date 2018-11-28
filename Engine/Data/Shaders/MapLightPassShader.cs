using System.Drawing;
using Cubed.Graphics;

namespace Cubed.Data.Shaders {
	
	/// <summary>
	/// Map shadowing shader
	/// </summary>
	internal class MapLightPassShader : ShaderBase {

		/// <summary>
		/// Shader singletone
		/// </summary>
		public static MapLightPassShader Shader {
			get {
				if (sh == null) {
					sh = new MapLightPassShader();
				}
				return sh;
			}
		}
		static MapLightPassShader sh;
		
		/// <summary>
		/// Фрагментный шейдер
		/// </summary>
		protected override string FragmentProgram {
			get {
				return ShaderSources.MapLightPassFragment;
			}
		}

		/// <summary>
		/// Вершинный шейдер
		/// </summary>
		protected override string VertexProgram {
			get {
				return ShaderSources.MapLightPassVertex;
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
		/// Расположение буффера текстур
		/// </summary>
		public int TexBufferLocation {
			get {
				return textureAttrib.Handle;
			}
		}

		/// <summary>
		/// Расположение буффера текстур
		/// </summary>
		public int NormalBufferLocation {
			get {
				return normalAttrib.Handle;
			}
		}

		/// <summary>
		/// Основной цвет
		/// </summary>
		public Color LightColor {
			get {
				return lightColor.Color;
			}
			set {
				lightColor.Color = value;
			}
		}

		/// <summary>
		/// Основной цвет
		/// </summary>
		public OpenTK.Vector3 LightPos {
			get {
				return lightPos.Vector;
			}
			set {
				lightPos.Vector = value;
			}
		}

		/// <summary>
		/// Основной цвет
		/// </summary>
		public float LightRange {
			get {
				return lightRange.Value;
			}
			set {
				lightRange.Value = value;
			}
		}

		/// <summary>
		/// Light angle
		/// </summary>
		public float LightRotation {
			get {
				return angle;
			}
			set {
				angle = value;
				lightRotation.Matrix = OpenTK.Matrix4.CreateRotationY(OpenTK.MathHelper.DegreesToRadians(-angle));
			}
		}

		// Скрытые параметры
		VertexAttribute vertexAttrib;
		VertexAttribute textureAttrib;
		VertexAttribute normalAttrib;
		ColorUniform lightColor;
		FloatUniform lightRange;
		VectorUniform lightPos;
		TextureUniform lightBuffer, lightTextureBuffer;
		MatrixUniform lightRotation;
		float angle = 0;

		// Конструктор
		protected MapLightPassShader() : base() {
			if (vertexAttrib == null) {
				vertexAttrib = new VertexAttribute("inPosition");
				attribs.Add(vertexAttrib);
			}
			if (textureAttrib == null) {
				textureAttrib = new VertexAttribute("inFlatPosition");
				attribs.Add(textureAttrib);
			}
			if (normalAttrib == null) {
				normalAttrib = new VertexAttribute("inNormal");
				attribs.Add(normalAttrib);
			}
			if (lightBuffer == null) {
				lightBuffer = new TextureUniform("depthBuffer");
				uniforms.Add(lightBuffer);
			}
			if (lightTextureBuffer == null) {
				lightTextureBuffer = new TextureUniform("lightTexture");
				lightTextureBuffer.Layer = 1;
				uniforms.Add(lightTextureBuffer);
			}
			if (lightColor == null) {
				lightColor = new ColorUniform("lightColor");
				uniforms.Add(lightColor);
			}
			if (lightRange == null) {
				lightRange = new FloatUniform("lightRange");
				uniforms.Add(lightRange);
			}
			if (lightPos == null) {
				lightPos = new VectorUniform("lightPos");
				uniforms.Add(lightPos);
			}
			if (lightRotation == null) {
				lightRotation = new MatrixUniform("lightRotation");
				lightRotation.Matrix = OpenTK.Matrix4.Identity;
				uniforms.Add(lightRotation);
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
