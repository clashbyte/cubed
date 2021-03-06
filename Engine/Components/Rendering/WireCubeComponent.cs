﻿using System;
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
	/// Куб из линий
	/// </summary>
	public class WireCubeComponent : EntityComponent, IRenderable {

		/// <summary>
		/// Массив индексов для куба
		/// </summary>
		static ushort[] indexArray;

		/// <summary>
		/// Flag for semitransparent
		/// </summary>
		public bool AlwaysVisible {
			get;
			set;
		}

		/// <summary>
		/// Расположение относительно центра объекта
		/// </summary>
		public Vector3 Position {
			get {
				return pos;
			}
			set {
				pos = value;
				needBuffer = true;
				RebuildParentCull();
			}
		}

		/// <summary>
		/// Размеры
		/// </summary>
		public Vector3 Size {
			get {
				return size;
			}
			set {
				size = value;
				needBuffer = true;
				RebuildParentCull();
			}
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
		Vector3 pos, size;

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
		public WireCubeComponent() {
			Position = Vector3.Zero;
			Size = Vector3.One;
			WireColor = Color.White;
			WireWidth = 1f;
		}

		/// <summary>
		/// Получение сферы отсечения
		/// </summary>
		/// <returns>Сфера отсечения</returns>
		internal override CullBox GetCullingBox() {
			return new CullBox() {
				Min = pos - size / 2,
				Max = pos + size / 2
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
			if (!Enabled) {
				return;
			}

			// Генерация массивов
			if (needBuffer || vertexArray == null) {
				float halfX = size.X / 2f;
				float halfY = size.Y / 2f;
				float halfZ = size.Z / 2f;

				if (vertexBuffer > 0 || indexBuffer > 0) {
					GL.DeleteBuffer(vertexBuffer);
					GL.DeleteBuffer(indexBuffer);
				}

				// Вершины
				vertexArray = new float[]{
					
					// Ближняя левая верхняя		(0)
					pos.X - halfX,
					pos.Y + halfY,
					-pos.Z + halfZ,

					// Ближняя правая верхняя		(1)
					pos.X + halfX,
					pos.Y + halfY,
					-pos.Z + halfZ,

					// Ближняя левая нижняя			(2)
					pos.X - halfX,
					pos.Y - halfY,
					-pos.Z + halfZ,

					// Ближняя правая нижняя		(3)
					pos.X + halfX,
					pos.Y - halfY,
					-pos.Z + halfZ,

					// Дальняя левая верхняя		(4)
					pos.X - halfX,
					pos.Y + halfY,
					-pos.Z - halfZ,

					// Дальняя правая верхняя		(5)
					pos.X + halfX,
					pos.Y + halfY,
					-pos.Z - halfZ,

					// Дальняя левая нижняя			(6)
					pos.X - halfX,
					pos.Y - halfY,
					-pos.Z - halfZ,

					// Дальняя правая нижняя		(7)
					pos.X + halfX,
					pos.Y - halfY,
					-pos.Z - halfZ

				};

				needBuffer = false;
			}

			// Массив индексов
			if (indexArray == null) {
				indexArray = new ushort[]{

					// Передняя часть
					0, 1,
					1, 3,
					3, 2,
					2, 0,

					// Задняя часть
					4, 5,
					5, 7,
					7, 6,
					6, 4,

					// Соединения
					0, 4,
					1, 5,
					2, 6,
					3, 7

				};
			}

			// Depth test
			if (AlwaysVisible) {
				GL.Disable(EnableCap.DepthTest);
			}

			// Отрисовка куба, без текстуры
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.LineWidth(WireWidth);
			if (Caps.ShaderPipeline) {

				ShaderSystem.CheckVertexBuffer(ref vertexBuffer, vertexArray, BufferUsageHint.StaticDraw);
				ShaderSystem.CheckIndexBuffer(ref indexBuffer, indexArray, BufferUsageHint.StaticDraw);

				WireCubeShader shader = WireCubeShader.Shader;
				shader.DiffuseColor = WireColor;
				shader.Bind();
				GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
				GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
				GL.DrawElements(BeginMode.Lines, indexArray.Length, DrawElementsType.UnsignedShort, 0);
				shader.Unbind();
			} else {
				GL.Color3(WireColor);
				GL.EnableClientState(ArrayCap.VertexArray);
				GL.VertexPointer(3, VertexPointerType.Float, 0, vertexArray);
				GL.DrawElements(PrimitiveType.Lines, indexArray.Length, DrawElementsType.UnsignedShort, indexArray);
				GL.DisableClientState(ArrayCap.VertexArray);
			}
			GL.Enable(EnableCap.DepthTest);
		}

	}
}
