using System;
using System.Collections.Generic;
using System.Drawing;
using Cubed.Data.Shaders;
using Cubed.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.World {
	
	/// <summary>
	/// Light entity
	/// </summary>
	public class Light : Entity {

		/// <summary>
		/// Light color
		/// </summary>
		public Color Color {
			get {
				return color;
			}
			set {
				color = value;
				changed = true;
			}
		}

		/// <summary>
		/// Light range
		/// </summary>
		public float Range {
			get {
				return range;
			}
			set {
				if (range != value) {
					changed = true;
					range = Math.Abs(value);
				}
			}
		}

		/// <summary>
		/// Light texture
		/// </summary>
		public Texture Texture {
			get {
				return texture;
			}
			set {
				if (texture != value) {
					textureWasReady = false;
					changed = true;
					texture = value;
				}
			}
		}

		/// <summary>
		/// Current texture angle
		/// </summary>
		public float TextureAngle {
			get {
				return textureAngle;
			}
			set {
				if (textureAngle != value) {
					textureAngle = value;
					changed = true;
				}
			}
		}

		/// <summary>
		/// Shadows
		/// </summary>
		public bool Shadows {
			get {
				return hasShadows;
			}
			set {
				if (hasShadows != value) {
					hasShadows = value;
					changed = true;
				}
			}
		}

		/// <summary>
		/// Render in static layer
		/// </summary>
		public bool Static {
			get {
				return isStatic;
			}
			set {
				isStatic = value;
				changed = true;
				changedAllLayers = true;
			}
		}

		/// <summary>
		/// Making light dirty
		/// </summary>
		internal void MakeDirty() {
			changed = true;
		}

		/// <summary>
		/// Recalculate light
		/// </summary>
		internal void RebuildTexture(Map map) {

			// Getting all chunks
			List<Map.Chunk> chunks = new List<Map.Chunk>();
			Map.Chunk[] tempChunks = map.GetAllChunks();
			foreach (Map.Chunk chunk in tempChunks) {
				if (chunk.TouchesLight(this)) {
					chunks.Add(chunk);
					chunk.needRelightStatic = chunk.needRelightStatic || isStatic || changedAllLayers;
					chunk.needRelightDynamic = chunk.needRelightDynamic || !isStatic || changedAllLayers;
				}
			}

			// Skipping lightmapping without shadows
			if (!Shadows) {
				CheckEmptyBuffer();
				LastUpdatePoint = Position;
				Caps.CheckErrors();
				return;
			}

			// Saving matrices
			Matrix4 camMat = ShaderSystem.CameraMatrix;
			Matrix4 entMat = ShaderSystem.EntityMatrix;
			Matrix4 prjMat = ShaderSystem.ProjectionMatrix;
			Matrix4 texMat = ShaderSystem.TextureMatrix;
			int texSize = CalculateLightmapSize();

			if (projRotations == null) {
				projRotations = new Matrix4[6]{
					Matrix4.CreateRotationY(MathHelper.PiOver2) * Matrix4.CreateScale(1, 1, -1),
					Matrix4.CreateRotationY(-MathHelper.PiOver2) * Matrix4.CreateScale(1, 1, -1),
					Matrix4.CreateRotationX(-MathHelper.PiOver2) * Matrix4.CreateScale(-1, 1, 1),
					Matrix4.CreateRotationX(MathHelper.PiOver2) * Matrix4.CreateScale(-1, 1, 1),
					Matrix4.Identity * Matrix4.CreateScale(-1, 1, 1),
					Matrix4.CreateRotationY(MathHelper.Pi) * Matrix4.CreateScale(-1, 1, 1)
				};
			}

			// Pushing state
			Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1, 0.01f, range);
			
			// Preparing render texture
			GL.Enable(EnableCap.TextureCubeMap);
			if (textureBuffer == 0 || !GL.IsTexture(textureBuffer)) {
				textureBuffer = GL.GenTexture();
				GL.BindTexture(TextureTarget.TextureCubeMap, textureBuffer);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
			} else {
				GL.BindTexture(TextureTarget.TextureCubeMap, textureBuffer);
			}
			for (int i = 0; i < 6; i++) {
				GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.R32f, texSize, texSize, 0, PixelFormat.Red, PixelType.Float, IntPtr.Zero);
			}
			GL.BindTexture(TextureTarget.TextureCubeMap, 0);
			GL.Disable(EnableCap.TextureCubeMap);

			// Preparing depth buffer
			CheckFrameAndDepth(texSize);
			Caps.CheckErrors();

			GL.Enable(EnableCap.DepthTest);
			MapShadowShader shader = MapShadowShader.Shader;

			// Clearing surface
			Matrix4 currentMat = mat.ClearRotation();
			for (int i = 0; i < 6; i++) {

				// Binding framebuffer
				GL.Viewport(0, 0, texSize, texSize);
				GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, textureBuffer, 0);
				GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthBuffer, 0);

				Caps.CheckErrors();

				GL.ClearDepth(1f);
				GL.ClearColor(float.MaxValue, 0, 0, 0);
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				// Calculating current matrix
				ShaderSystem.CameraMatrix = (projRotations[i] * currentMat).Inverted();
				ShaderSystem.ProjectionMatrix = projMatrix;

				// Rendering
				foreach (Map.Chunk chunk in chunks) {
					ShaderSystem.EntityMatrix = chunk.GetMatrix();
					shader.Bind();
					chunk.RenderShadow();
					Core.Engine.Current.drawCalls++;
					shader.Unbind();
				}
			}
			
			// Releasing buffer
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			Caps.CheckErrors();

			// Last position
			LastUpdatePoint = Position;

			// Restoring matrices
			ShaderSystem.CameraMatrix = camMat;
			ShaderSystem.EntityMatrix = entMat;
			ShaderSystem.ProjectionMatrix = prjMat;
			ShaderSystem.TextureMatrix = texMat;
		}

		/// <summary>
		/// Lightmap resolution detection
		/// </summary>
		/// <returns>Desired lightmap size</returns>
		int CalculateLightmapSize() {

			// Works like a charm
			return 256;
		}

		/// <summary>
		/// Rebuilding empty buffer if neccessary
		/// </summary>
		void CheckEmptyBuffer() {
			// Preparing render texture
			if (emptyBuffer == 0 || !GL.IsTexture(emptyBuffer)) {
				byte[] raw = BitConverter.GetBytes(float.MaxValue);

				emptyBuffer = GL.GenTexture();
				GL.Enable(EnableCap.TextureCubeMap);
				GL.BindTexture(TextureTarget.TextureCubeMap, emptyBuffer);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
				for (int i = 0; i < 6; i++) {
					GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.R32f, 1, 1, 0, PixelFormat.Red, PixelType.Float, raw);
				}
				GL.BindTexture(TextureTarget.TextureCubeMap, 0);
				GL.Disable(EnableCap.TextureCubeMap);
			}
		}

		/// <summary>
		/// Checking frame and depth buffer
		/// </summary>
		static void CheckFrameAndDepth(int size) {

			// Preparing depth buffer
			GL.Enable(EnableCap.Texture2D);
			if (depthBuffer == 0 || !GL.IsTexture(depthBuffer)) {
				depthBuffer = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, depthBuffer);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.DepthTextureMode, (int)All.Luminance);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)All.CompareRToTexture);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)All.Lequal);
			} else {
				GL.BindTexture(TextureTarget.Texture2D, depthBuffer);
			}
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, size, size, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.Disable(EnableCap.Texture2D);

			// Preparing framebuffer
			if (frameBuffer == 0 || !GL.IsFramebuffer(frameBuffer)) {
				frameBuffer = GL.GenFramebuffer();
			}
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
		}

		/// <summary>
		/// Rebuilding directional light
		/// </summary>
		internal static void RebuildDirectionalTexture(IEnumerable<Map.Chunk> chunks, ref int glTex, int texSize) {
			
			// Preparing render texture
			GL.Enable(EnableCap.TextureCubeMap);
			if (glTex == 0 || !GL.IsTexture(glTex)) {
				glTex = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, glTex);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
			} else {
				GL.BindTexture(TextureTarget.Texture2D, glTex);
			}
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, texSize, texSize, 0, PixelFormat.Red, PixelType.Float, IntPtr.Zero);
			GL.BindTexture(TextureTarget.TextureCubeMap, 0);
			GL.Disable(EnableCap.TextureCubeMap);

			// Preparing depth buffer
			CheckFrameAndDepth(texSize);
			GL.Enable(EnableCap.DepthTest);
			MapShadowShader shader = MapShadowShader.Shader;

			// Binding framebuffer
			GL.Viewport(0, 0, texSize, texSize);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, glTex, 0);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthBuffer, 0);
			GL.ClearDepth(1f);
			GL.ClearColor(float.MaxValue, 0, 0, 0);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// Rendering
			foreach (Map.Chunk chunk in chunks) {
				ShaderSystem.EntityMatrix = chunk.GetMatrix();
				shader.Bind();
				chunk.RenderShadow();
				Core.Engine.Current.drawCalls++;
				shader.Unbind();
			}

			// Releasing buffer
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.Disable(EnableCap.DepthTest);
		}

		/// <summary>
		/// Saving current state
		/// </summary>
		internal void Cleanup() {
			changed = false;
			changedAllLayers = false;
		}

		/// <summary>
		/// Does light have changes
		/// </summary>
		internal bool IsChanged {
			get {
				return changed;
			}
		}

		/// <summary>
		/// Last updated position
		/// </summary>
		internal Vector3 LastUpdatePoint {
			get;
			private set;
		}

		/// <summary>
		/// Buffer for shading
		/// </summary>
		internal int DepthTextureBuffer {
			get {
				if (Shadows) {
					return textureBuffer;
				}
				return emptyBuffer;
			}
		}

		/// <summary>
		/// Light internal range
		/// </summary>
		float range = 3f;

		/// <summary>
		/// Light internal color
		/// </summary>
		Color color = Color.White;

		/// <summary>
		/// Internal texture
		/// </summary>
		Texture texture = null;

		/// <summary>
		/// Current texture angle
		/// </summary>
		float textureAngle = 0;

		/// <summary>
		/// Light parameters changed flag
		/// </summary>
		bool changed = false;

		/// <summary>
		/// Light changed both layers
		/// </summary>
		bool changedAllLayers = false;

		/// <summary>
		/// Texture ready flag
		/// </summary>
		bool textureWasReady = false;

		/// <summary>
		/// Does this light cast shadows
		/// </summary>
		bool hasShadows = true;

		/// <summary>
		/// Light moves only sometimes
		/// </summary>
		bool isStatic = false;

		/// <summary>
		/// Current light texture
		/// </summary>
		int textureBuffer;

		/// <summary>
		/// Basic render buffer
		/// </summary>
		static int frameBuffer, depthBuffer;

		/// <summary>
		/// Depth texture for light without shadows
		/// </summary>
		static int emptyBuffer;

		/// <summary>
		/// Matrices for rotation
		/// </summary>
		static Matrix4[] projRotations;

		/// <summary>
		/// Matrix rebuilding
		/// </summary>
		protected override void RebuildMatrix() {
			changed = true;
			base.RebuildMatrix();
		}
		
	}
}
