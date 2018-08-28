using System.Drawing;
using Cubed.Graphics;

namespace Cubed.Data.Shaders {
	
	/// <summary>
	/// Map shader
	/// </summary>
	internal class MapShader : ShaderBase {

		/// <summary>
		/// Shader singletone
		/// </summary>
		public static MapShader Shader {
			get {
				if (sh == null) {
					sh = new MapShader();
				}
				return sh;
			}
		}
		static MapShader sh;
		
		/// <summary>
		/// Фрагментный шейдер
		/// </summary>
		protected override string FragmentProgram {
			get {
				return ShaderSources.MapFragment;
			}
		}

		/// <summary>
		/// Вершинный шейдер
		/// </summary>
		protected override string VertexProgram {
			get {
				return ShaderSources.MapVertex;
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
		/// Расположение буффера текстурных координат
		/// </summary>
		public int LightTexCoordBufferLocation {
			get {
				return lightCoordAttrib.Handle;
			}
		}

		/// <summary>
		/// Основной цвет
		/// </summary>
		public Color AmbientColor {
			get {
				return ambientColor.Color;
			}
			set {
				ambientColor.Color = value;
			}
		}

		/// <summary>
		/// Fog enabled
		/// </summary>
		public bool FogEnabled {
			get {
				return fogEnabled.Value;
			}
			set {
				fogEnabled.Value = value;
			}
		}

		/// <summary>
		/// Fog color
		/// </summary>
		public Color FogColor {
			get {
				return fogColor.Color;
			}
			set {
				fogColor.Color = value;
			}
		}

		/// <summary>
		/// Fog near
		/// </summary>
		public float FogNear {
			get {
				return fogNear.Value;
			}
			set {
				fogNear.Value = value;
			}
		}

		/// <summary>
		/// Fog fab
		/// </summary>
		public float FogFar {
			get {
				return fogFar.Value;
			}
			set {
				fogFar.Value = value;
			}
		}

		// Скрытые параметры
		TextureUniform textureUniform;
		TextureUniform lightmapUniform;
		ColorUniform ambientColor;
		ColorUniform fogColor;
		MatrixUniform textureMatrix;
		BoolUniform fogEnabled;
		FloatUniform fogNear;
		FloatUniform fogFar;
		VertexAttribute vertexAttrib;
		VertexAttribute texCoordAttrib;
		VertexAttribute lightCoordAttrib;

		// Конструктор
		protected MapShader() : base() {
			if (vertexAttrib == null) {
				vertexAttrib = new VertexAttribute("inPosition");
				attribs.Add(vertexAttrib);
			}
			if (texCoordAttrib == null) {
				texCoordAttrib = new VertexAttribute("inTexCoord");
				attribs.Add(texCoordAttrib);
			}
			if (lightCoordAttrib == null) {
				lightCoordAttrib = new VertexAttribute("inLightCoord");
				attribs.Add(lightCoordAttrib);
			}
			if (textureUniform == null) {
				textureUniform = new TextureUniform("texture");
				textureUniform.Layer = 0;
				uniforms.Add(textureUniform);
			}
			if (lightmapUniform == null) {
				lightmapUniform = new TextureUniform("lightmap");
				lightmapUniform.Layer = 1;
				uniforms.Add(lightmapUniform);
			}
			if (ambientColor == null) {
				ambientColor = new ColorUniform("ambient");
				uniforms.Add(ambientColor);
			}
			if (textureMatrix == null) {
				textureMatrix = new MatrixUniform("textureMatrix");
				uniforms.Add(textureMatrix);
			}
			if (fogEnabled == null) {
				fogEnabled = new BoolUniform("fog");
				uniforms.Add(fogEnabled);
			}
			if (fogColor == null) {
				fogColor = new ColorUniform("fogColor");
				uniforms.Add(fogColor);
			}
			if (fogNear == null) {
				fogNear = new FloatUniform("fogNear");
				uniforms.Add(fogNear);
			}
			if (fogFar == null) {
				fogFar = new FloatUniform("fogFar");
				uniforms.Add(fogFar);
			}
		}

		/// <summary>
		/// Установка шейдера
		/// </summary>
		public override void Bind() {
			textureMatrix.Matrix = ShaderSystem.TextureMatrix;
			base.Bind();
		}

	}
}
