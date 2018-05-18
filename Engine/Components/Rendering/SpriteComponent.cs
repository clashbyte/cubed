using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Core;
using Cubed.Data.Shaders;
using Cubed.Data.Types;
using Cubed.Graphics;
using Cubed.World;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.Components.Rendering {

	/// <summary>
	/// Sprite component
	/// </summary>
	public class SpriteComponent : EntityComponent, IUpdatable, IRenderable {

		/// <summary>
		/// Renderable texture
		/// </summary>
		public Texture Texture {
			get;
			set;
		}

		/// <summary>
		/// Blending
		/// </summary>
		public EntityComponent.BlendingMode Blending {
			get;
			set;
		}

		/// <summary>
		/// Color tint
		/// </summary>
		public Color Tint {
			get;
			set;
		}

		/// <summary>
		/// Sprite facing mode
		/// </summary>
		public FacingMode Facing {
			get;
			set;
		}

		/// <summary>
		/// Enable sprite lighting
		/// </summary>
		public bool AffectedByLight {
			get;
			set;
		}

		/// <summary>
		/// Sprite offset
		/// </summary>
		public Vector2 Offset {
			get {
				return offset;
			}
			set {
				if (offset != value) {
					offset = value;
					rebuildMesh = true;
				}
			}
		}

		/// <summary>
		/// Sprite offset
		/// </summary>
		public Vector2 Scale {
			get {
				return scale;
			}
			set {
				if (scale != value) {
					scale = value;
					rebuildMesh = true;
				}
			}
		}

		/// <summary>
		/// Mesh offset
		/// </summary>
		Vector2 offset;

		/// <summary>
		/// Mesh scale
		/// </summary>
		Vector2 scale;
		
		/// <summary>
		/// Flag for mesh rebuild
		/// </summary>
		bool rebuildMesh;

		/// <summary>
		/// Flag for tex coord rebuild
		/// </summary>
		bool rebuildTexCoords;

		/// <summary>
		/// Raw vertex
		/// </summary>
		float[] vertexData;

		/// <summary>
		/// Raw tex coord
		/// </summary>
		float[] texCoordData;

		/// <summary>
		/// Raw index data
		/// </summary>
		ushort[] indexData;

		/// <summary>
		/// Vertex coord buffer
		/// </summary>
		int vertexBuffer;

		/// <summary>
		/// Index coord buffer
		/// </summary>
		int texCoordBuffer;

		/// <summary>
		/// Index coord buffer
		/// </summary>
		int indexBuffer;

		/// <summary>
		/// Component constructor
		/// </summary>
		public SpriteComponent() {
			offset = Vector2.Zero;
			scale = Vector2.One;
			Tint = Color.White;
			Blending = BlendingMode.AlphaChannel;
			Facing = FacingMode.XY;
			AffectedByLight = false;
			rebuildMesh = true;
			rebuildTexCoords = true;
		}
		
		/// <summary>
		/// Blending mode
		/// </summary>
		internal override EntityComponent.BlendingMode RenditionBlending {
			get {
				return Blending;
			}
		}

		/// <summary>
		/// Выборка, в какой из проходов рендера включить данный меш
		/// </summary>
		internal override EntityComponent.TransparencyPass RenditionPass {
			get {
				if (Tint.A < 255) {
					return TransparencyPass.Blend;
				}
				switch (Blending) {
					case BlendingMode.AlphaChannel:
						if (Texture != null) {
							if (Texture.Transparency == Texture.TransparencyMode.AlphaFull) {
								return TransparencyPass.Blend;
							} else if (Texture.Transparency == Texture.TransparencyMode.AlphaCut) {
								return TransparencyPass.AlphaTest;
							}
						}
						break;
					case BlendingMode.Brightness:
					case BlendingMode.Add:
					case BlendingMode.Multiply:
						return TransparencyPass.Blend;
				}
				return TransparencyPass.Opaque;
			}
		}

		/// <summary>
		/// Получение сферы отсечения
		/// </summary>
		/// <returns>Сфера отсечения</returns>
		internal override CullBox GetCullingBox() {
			float sz = scale.LengthFast / 2f + offset.LengthFast;
			return new CullBox() {
				Min = -Vector3.One * sz,
				Max = Vector3.One * sz
			};
		}

		/// <summary>
		/// Update logics
		/// </summary>
		void IUpdatable.Update() {
			
		}

		/// <summary>
		/// Render sprite
		/// </summary>
		void IRenderable.Render() {

			// Rebuilding buffers
			if (rebuildMesh) {

				// Creating mesh
				vertexData = new float[] {
					-0.5f * scale.X + offset.X,  0.5f * scale.Y + offset.Y, 0,
					 0.5f * scale.X + offset.X,  0.5f * scale.Y + offset.Y, 0,
					-0.5f * scale.X + offset.X, -0.5f * scale.Y + offset.Y, 0,
					 0.5f * scale.X + offset.X, -0.5f * scale.Y + offset.Y, 0,
				};
				indexData = new ushort[] {
					0, 1, 2,
					1, 3, 2
				};

				// Sending buffer
				if (Caps.ShaderPipeline) {
					if (vertexBuffer == 0 || !GL.IsBuffer(vertexBuffer)) {
						vertexBuffer = GL.GenBuffer();
					}
					GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
					GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * 4, vertexData, BufferUsageHint.StaticDraw);
					GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

					if (indexBuffer == 0 || !GL.IsBuffer(indexBuffer)) {
						indexBuffer = GL.GenBuffer();
					}
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
					GL.BufferData(BufferTarget.ElementArrayBuffer, indexData.Length * 2, indexData, BufferUsageHint.StaticDraw);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

				}
				rebuildMesh = false;
				rebuildTexCoords = true;
			}
			if (rebuildTexCoords) {
				// Creating mesh
				texCoordData = new float[] {
					0, 0,
					1, 0,
					0, 1,
					1, 1
				};

				// Sending buffer
				if (Caps.ShaderPipeline) {
					if (texCoordBuffer == 0 || !GL.IsBuffer(texCoordBuffer)) {
						texCoordBuffer = GL.GenBuffer();
					}
					GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordBuffer);
					GL.BufferData(BufferTarget.ArrayBuffer, texCoordData.Length * 4, texCoordData, BufferUsageHint.StaticDraw);
					GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				}
				rebuildTexCoords = false;
			}

			// Calculating matrix
			Matrix4 pmat = Parent.RenditionMatrix;
			Matrix4 cmat = Camera.Current.RenditionMatrix;
			Matrix4 mat = Matrix4.Identity;
			if (Facing == FacingMode.XY) {
				mat =
					Matrix4.CreateFromQuaternion(cmat.ExtractRotation() * Quaternion.FromEulerAngles(-Parent.LocalAngles.Z / 180f * (float)Math.PI, 0, 0)) *
					pmat.ClearRotation();
			} else if (Facing == FacingMode.Y) {
				mat =
					Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(-Parent.LocalAngles.Z / 180f * (float)Math.PI, -Camera.Current.Angles.Y / 180f * (float)Math.PI, 0)) *
					pmat.ClearRotation();
			} else {
				mat =
					pmat;
			}

			// Binding texture
			if (this.Texture != null) {
				this.Texture.Bind();
			} else {
				Texture.BindEmpty();
			}
			GL.Disable(EnableCap.CullFace);

			Color color = Tint;
			if (AffectedByLight) {
				Vector3 pos = Parent.Position;
				Color light = Scene.Current.GetLightAtPoint(pos.X, pos.Y, pos.Z);
				color = Color.FromArgb(
					Tint.A,
					(byte)(((float)color.R / 255f) * ((float)light.R / 255f) * 255f),
					(byte)(((float)color.G / 255f) * ((float)light.G / 255f) * 255f),
					(byte)(((float)color.B / 255f) * ((float)light.B / 255f) * 255f) 
				);
			}


			// Handling
			if (Caps.ShaderPipeline) {

				Matrix4 prev = ShaderSystem.EntityMatrix;
				ShaderSystem.EntityMatrix = mat;

				SpriteShader shader = SpriteShader.Shader;
				shader.DiffuseColor = color;
				shader.Bind();
				GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
				GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordBuffer);
				GL.VertexAttribPointer(shader.TexCoordBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
				GL.DrawElements(PrimitiveType.Triangles, indexData.Length, DrawElementsType.UnsignedShort, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				shader.Unbind();

				ShaderSystem.EntityMatrix = prev;
			} else {

				Matrix4 imat = Camera.Current.InvertedMatrix;
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.LoadMatrix(ref imat);
				GL.MultMatrix(ref mat);

				GL.Color4(Tint);
				GL.EnableClientState(ArrayCap.VertexArray);
				GL.VertexPointer(3, VertexPointerType.Float, 0, vertexData);
				GL.ClientActiveTexture(TextureUnit.Texture0);
				GL.EnableClientState(ArrayCap.TextureCoordArray);
				GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, texCoordData);
				GL.DrawElements(PrimitiveType.Triangles, indexData.Length, DrawElementsType.UnsignedShort, indexData);
				GL.DisableClientState(ArrayCap.TextureCoordArray);
				GL.DisableClientState(ArrayCap.VertexArray);

				GL.PopMatrix();
			}
			Engine.Current.drawCalls++;

			// Returning
			GL.Enable(EnableCap.CullFace);
		}

		/// <summary>
		/// Sprite align mode
		/// </summary>
		public enum FacingMode : byte {
			Disabled = 0,
			XY = 1,
			Y = 2
		}
	}
}
