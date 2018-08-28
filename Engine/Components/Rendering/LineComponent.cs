using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Cubed.Data.Types;
using Cubed.Data.Shaders;
using Cubed.Graphics;

namespace Cubed.Components.Rendering {

	/// <summary>
	/// Набор линий
	/// </summary>
	public class LineComponent : EntityComponent, IRenderable {

		/// <summary>
		/// Массив индексов для куба
		/// </summary>
		static ushort[] indexArray;

		/// <summary>
		/// Расположение относительно центра объекта
		/// </summary>
		public Vector3[] Vertices {
			get {
				return vertexList;
			}
			set {
				vertexList = value;
				if (vertexList == null) {
					vertexList = new Vector3[0];
				}
				needBuffer = true;
				RebuildParentCull();
			}
		}

		/// <summary>
		/// Current line mode
		/// </summary>
		public LineType Mode {
			get;
			set;
		}

		/// <summary>
		/// Blending
		/// </summary>
		internal override EntityComponent.BlendingMode RenditionBlending {
			get {
				if (WireColor.A < 255) {
					return BlendingMode.AlphaChannel;
				}
				return BlendingMode.ForceOpaque;
			}
		}

		/// <summary>
		/// Rendition pass
		/// </summary>
		internal override EntityComponent.TransparencyPass RenditionPass {
			get {
				if (WireColor.A < 255) {
					return TransparencyPass.Blend;
				}
				return TransparencyPass.Opaque;
			}
		}

		/// <summary>
		/// Цвет линий
		/// </summary>
		public Color WireColor;

		/// <summary>
		/// Толщина линий
		/// </summary>
		public float WireWidth;

		/// <summary>
		/// Скрытые характеристики объекта
		/// </summary>
		Vector3[] vertexList;

		/// <summary>
		/// Необходима перестройка буффера
		/// </summary>
		bool needBuffer;

		/// <summary>
		/// Массив вершин
		/// </summary>
		float[] vertexArray;

		// Скрытые переменные буфферов
		int vertexBuffer;
		int indexBuffer;

		/// <summary>
		/// Конструктор компонента
		/// </summary>
		public LineComponent() {
			vertexList = new Vector3[0];
			WireColor = Color.White;
			Mode = LineType.Strip;
			WireWidth = 1f;
		}

		/// <summary>
		/// Получение сферы отсечения
		/// </summary>
		/// <returns>Сфера отсечения</returns>
		internal override CullBox GetCullingBox() {
			Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			foreach (Vector3 v in vertexList) {
				max.X = Math.Max(max.X, v.X);
				max.Y = Math.Max(max.Y, v.Y);
				max.Z = Math.Max(max.Z, v.Z);
				min.X = Math.Min(min.X, v.X);
				min.Y = Math.Min(min.Y, v.Y);
				min.Z = Math.Min(min.Z, v.Z);
			}
			return new CullBox() {
				Min = min,
				Max = max
			};
		}

		/// <summary>
		/// Destroying component
		/// </summary>
		internal override void Destroy() {
			vertexArray = null;
			if (vertexBuffer != 0) {
				GL.DeleteBuffer(vertexBuffer);
			}
			if (indexBuffer != 0) {
				GL.DeleteBuffer(indexBuffer);
			}
			needBuffer = false;
		}

		/// <summary>
		/// Отрисовка куба
		/// </summary>
		void IRenderable.Render() {

			// Выход если выключен
			if (!Enabled || vertexList.Length == 0) {
				return;
			}

			// Генерация массивов
			if (needBuffer || vertexArray == null) {

				if (vertexBuffer > 0) {
					GL.DeleteBuffer(vertexBuffer);
				}

				// Вершины
				vertexArray = new float[vertexList.Length * 3];
				for (int i = 0; i < vertexList.Length; i++) {
					vertexArray[i * 3 + 0] = vertexList[i].X;
					vertexArray[i * 3 + 1] = vertexList[i].Y;
					vertexArray[i * 3 + 2] = -vertexList[i].Z;
				}

				needBuffer = false;
			}

			// Режим
			PrimitiveType primitive = PrimitiveType.Lines;
			switch (Mode) {
				case LineType.Strip:
					primitive = PrimitiveType.LineStrip;
					break;
				case LineType.Loop:
					primitive = PrimitiveType.LineLoop;
					break;
			}

			// Отрисовка куба, без текстуры
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.LineWidth(WireWidth);
			if (Caps.ShaderPipeline) {

				ShaderSystem.CheckVertexBuffer(ref vertexBuffer, vertexArray, BufferUsageHint.StaticDraw);

				WireCubeShader shader = WireCubeShader.Shader;
				shader.DiffuseColor = WireColor;
				shader.Bind();
				GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
				GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.DrawArrays(primitive, 0, vertexList.Length);
				shader.Unbind();
			} else {
				GL.Color3(WireColor);
				GL.EnableClientState(ArrayCap.VertexArray);
				GL.VertexPointer(3, VertexPointerType.Float, 0, vertexArray);
				GL.DrawArrays(primitive, 0, vertexList.Length);
				GL.DisableClientState(ArrayCap.VertexArray);
			}
		}

		/// <summary>
		/// Lines mode
		/// </summary>
		public enum LineType {
			Pairs,
			Strip,
			Loop
		}

	}
}
