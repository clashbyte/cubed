using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using Cubed.Graphics;
using Cubed.Data.Shaders;
using Cubed.Data.Types;

namespace Cubed.Components.Rendering {

	/// <summary>
	/// Компонент повершинно-анимированного меша
	/// </summary>
	public class MorphMeshComponent : AnimatedMeshComponent {

		/// <summary>
		/// Кадры анимации
		/// </summary>
		public override AnimatedMeshComponent.Frame[] Frames {
			get {
				return base.Frames;
			}
			set {
				base.Frames = value;
				if (value != null) {
					RebuildCullingSphere();
					queuedUpdate = new QueuedMeshUpdate() {
						FirstFrame = 0,
						LastFrame = 1,
						Time = 0f,
						IsLooping = true
					};
					if (Caps.ShaderPipeline) {
						vertexCount = (value[0] as MorphFrame).verts.Length;
					}
				}
			}
		}

		/// <summary>
		/// Вершины - только чтение
		/// </summary>
		public override Vector3[] Vertices {
			get {
				if (vertices == null) {
					Frame[] frm = Frames;
					if (frm != null) {
						MorphFrame mf = InterpolateFrame((MorphFrame)frm[0], (MorphFrame)frm[0], 0f);
						queuedUpdate = new QueuedMeshUpdate() {
							FirstFrame = 0,
							LastFrame = 1,
							IsLooping = false,
							Time = 0
						};
						vertices = mf.verts;
					}
				}
				return base.Vertices;
			}
		}

		/// <summary>
		/// Нормали - только чтение
		/// </summary>
		public override Vector3[] Normals {
			get {
				if (normals == null) {
					Frame[] frm = Frames;
					if (frm != null) {
						MorphFrame mf = InterpolateFrame((MorphFrame)frm[0], (MorphFrame)frm[0], 0f);
						queuedUpdate = new QueuedMeshUpdate() {
							FirstFrame = 0,
							LastFrame = 1,
							IsLooping = false,
							Time = 0
						};
						normals = mf.normals;
					}
				}
				return base.Normals;
			}
		}

		/// <summary>
		/// Текущий кадр
		/// </summary>
		MorphFrame currentFrame;
		/// <summary>
		/// Переходный кадр
		/// </summary>
		MorphFrame transitionFrame;

		/// <summary>
		/// Первый и второй буфферы позиций вершин
		/// </summary>
		int firstVertexBuffer, secondVertexBuffer;
		/// <summary>
		/// Первый и второй буфферы нормалей вершин
		/// </summary>
		int firstNormalBuffer, secondNormalBuffer;

		/// <summary>
		/// Переход между двумя буфферами
		/// </summary>
		float bufferInterpolation;

		/// <summary>
		/// Предыдущие использованные кадры - снижают нагрузку на пайплайн
		/// </summary>
		int firstFrameUsed = -64, secondFrameUsed = -64;

		/// <summary>
		/// Конструктор
		/// </summary>
		public MorphMeshComponent() : base() {
			cull = null;
		}
		
		/// <summary>
		/// Новый рендерер
		/// </summary>
		protected override void ModernRender() {

			// Проверка буфферов
			needVertexBuffer = false;
			needNormalBuffer = false;
			CheckBuffers();

			// Количество вершин и индексов
			int vCount = GetVertexCount();
			int iCount = GetIndexCount();

			// Рендер
			if (iCount > 0 && vCount > 0) {

				// Установка текстуры
				if (Texture != null) {
					Texture.Bind();
				} else {
					Texture.BindEmpty();
				}

				// Шейдер
				MorphMeshShader shader = MorphMeshShader.Shader;
				shader.DiffuseColor = Diffuse;
				shader.FrameDelta = bufferInterpolation;
				shader.LightMultiplier = Unlit ? 1f : LIGHT_MULT;
				shader.Bind();

				// Вершины
				GL.BindBuffer(BufferTarget.ArrayBuffer, firstVertexBuffer);
				GL.VertexAttribPointer(shader.FirstVertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, secondVertexBuffer);
				GL.VertexAttribPointer(shader.SecondVertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				
				// Нормали
				GL.BindBuffer(BufferTarget.ArrayBuffer, firstNormalBuffer);
				GL.VertexAttribPointer(shader.FirstNormalBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, secondNormalBuffer);
				GL.VertexAttribPointer(shader.SecondNormalBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				
				// Текстурные координаты
				GL.BindBuffer(BufferTarget.ArrayBuffer, SearchTexCoordBuffer());
				GL.VertexAttribPointer(shader.TexCoordBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
				
				// Индексы и отрисовка
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, SearchIndexBuffer());
				GL.DrawElements(BeginMode.Triangles, iCount, DrawElementsType.UnsignedShort, 0);
				shader.Unbind();

				// Отрисовка оверлея
				if (WireframeOverlay) {
					shader.DiffuseColor = WireframeColor;
					shader.LightMultiplier = 1f;
					shader.Bind();
					Texture.BindEmpty();
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
					GL.Enable(EnableCap.PolygonOffsetLine);
					GL.LineWidth(1f);
					GL.DrawElements(BeginMode.Triangles, iCount, DrawElementsType.UnsignedShort, 0);
					GL.Disable(EnableCap.PolygonOffsetLine);
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
					shader.Unbind();
				}
			}

		}

		/// <summary>
		/// Получение бокса для отсечения
		/// </summary>
		/// <returns>Коробка</returns>
		internal override CullBox GetCullingBox() {
			if (cull == null) {
				CullBox cb = null;
				MeshComponent mc = this;
				while (cb == null) {
					if (mc.Proxy != null) {
						mc = mc.Proxy;
					} else {
						break;
					}
					cb = mc.GetCullingBox();
				}
			}
			return cull;
		}

		/// <summary>
		/// Интерполирование двух кадров
		/// </summary>
		/// <param name="mf1">Первый кадр</param>
		/// <param name="mf2">Второй кадр</param>
		/// <param name="delta">Значение интерполяции</param>
		MorphFrame InterpolateFrame(MorphFrame mf1, MorphFrame mf2, float delta) {
			MorphFrame mf = new MorphFrame();
			mf.verts = new float[mf1.verts.Length];
			mf.normals = new float[mf1.normals.Length];

			// Интерполяция данных
			float p1, p2;
			for (int i = 0; i < mf.verts.Length; i++) {
				
				// Вершина
				p1 = mf1.verts[i];
				p2 = mf2.verts[i];
				mf.verts[i] = p1 + (p2 - p1) * delta;

				// Нормаль
				p1 = mf1.normals[i];
				p2 = mf2.normals[i];
				mf.normals[i] = p1 + (p2 - p1) * delta;
			}

			// Сохранение временного кадра
			return mf;
		}

		/// <summary>
		/// Обновление анимации
		/// </summary>
		protected override void UpdateAnimation() {
			Frame[] frms = StepFrames;
			if (frms != null) {
				MorphFrame f1, f2;
				float d = SeekFrames(frms, queuedUpdate, out f1, out f2);
				if (Caps.ShaderPipeline) {
					UpdateBuffers(f1, f2, d);
				} else {
					currentFrame = InterpolateFrame(f1, f2, d);
					vertices = currentFrame.verts;
					normals = currentFrame.normals;
				}
			}
		}

		/// <summary>
		/// Создание переходного кадра
		/// </summary>
		protected override void UpdateTransition() {
			Frame[] frms = StepFrames;
			if (frms != null) {
				MorphFrame f1, f2;
				float d = SeekFrames(frms, queuedTransitionUpdate, out f1, out f2);
				transitionFrame = InterpolateFrame(f1, f2, d);
				transitionFrame.Time = -1;
			}
		}

		/// <summary>
		/// Поиск требуемых кадров
		/// </summary>
		/// <param name="q">Запланированное обновление меша</param>
		/// <param name="f1">Первый выходной кадр</param>
		/// <param name="f2">Второй выходной кадр</param>
		/// <returns>Переходное число в пределах [0-1]</returns>
		float SeekFrames(Frame[] frms, QueuedMeshUpdate q, out MorphFrame f1, out MorphFrame f2) {
			float d;
			if (q.Time < 0) {
				// Транзишн - смотрим на переходной кадр
				f1 = transitionFrame;
				f2 = GetFrameBackward(frms, q.IsBackward ? q.LastFrame+1 : q.FirstFrame, q.IsLooping, q.FirstFrame, q.LastFrame);
				d = 1f + q.Time;
			} else {
				// Обычная анимация - смотрим на соседние кадры
				f1 = GetFrameBackward(frms, q.Time, q.IsLooping, q.FirstFrame, q.LastFrame);
				f2 = GetFrameForward(frms, q.Time, q.IsLooping, q.FirstFrame, q.LastFrame);
				if (f1.Time != f2.Time) {
					// Если два разных кадра
					if (f1.Time > f2.Time) {
						if (q.Time > q.LastFrame && q.IsLooping) {
							d = (q.Time - q.LastFrame);
						} else {
							d = (q.Time - f2.Time) / (f1.Time - f2.Time);
						}
					} else {
						d = (q.Time - f1.Time) / (f2.Time - f1.Time);
					}
				} else {
					// Один и тот же кадр - избегаем деления на ноль
					d = 0;
				}
			}
			return d;
		}

		/// <summary>
		/// Поиск кадра вперед
		/// </summary>
		/// <param name="time">Время</param>
		/// <param name="allowLoop">Разрешить ли поиск с начала</param>
		/// <returns>Кадр</returns>
		MorphFrame GetFrameForward(Frame[] farr, float time, bool allowLoop, int minTime, int maxTime) {
			int idx = 0;
			if (time < minTime) {
				time = minTime;
			}
			if (time >= maxTime) {
				idx = allowLoop ? minTime : maxTime;
			} else {
				idx = (int)Math.Floor(time)+1;
			}
			if (idx < 0) idx = 0;
			if (idx >= farr.Length) idx = farr.Length - 1;
			return (MorphFrame)farr[idx];
		}

		/// <summary>
		/// Поиск кадра назад
		/// </summary>
		/// <param name="time">Время</param>
		/// <param name="allowLoop">Разрешить ли поиск с конца</param>
		/// <returns>Кадр</returns>
		MorphFrame GetFrameBackward(Frame[] farr, float time, bool allowLoop, int minTime, int maxTime) {
			int idx = 0;
			if (time > maxTime) {
				time = maxTime;
			}
			if (time < minTime) {
				idx = allowLoop ? maxTime : minTime;
			} else {
				idx = (int)Math.Floor(time);
			}
			if (idx < 0) idx = 0;
			if (idx >= farr.Length) idx = farr.Length - 1;
			return (MorphFrame)farr[idx];
		}

		/// <summary>
		/// Обновление состояния буфферов
		/// </summary>
		void UpdateBuffers(MorphFrame f1, MorphFrame f2, float d) {
			bool needFirstBuffer = false;
			bool needSecondBuffer = false;
			if (firstFrameUsed != f1.Time || f1.Time == -1) {
				needFirstBuffer = true;
				firstFrameUsed = (int)f1.Time;
			}
			if (secondFrameUsed != f2.Time || f2.Time == -1) {
				needSecondBuffer = true;
				secondFrameUsed = (int)f2.Time;
			}

			if (needFirstBuffer) {
				if (firstVertexBuffer == 0) firstVertexBuffer = GL.GenBuffer();
				if (firstNormalBuffer == 0) firstNormalBuffer = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, firstVertexBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(f1.verts.Length * 4), f1.verts, BufferUsageHint.StreamDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, firstNormalBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(f1.normals.Length * 4), f1.normals, BufferUsageHint.StreamDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			}

			if (needSecondBuffer) {
				if (secondVertexBuffer == 0) secondVertexBuffer = GL.GenBuffer();
				if (secondNormalBuffer == 0) secondNormalBuffer = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, secondVertexBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(f2.verts.Length * 4), f2.verts, BufferUsageHint.StreamDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, secondNormalBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(f2.normals.Length * 4), f2.normals, BufferUsageHint.StreamDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			}

			bufferInterpolation = d;
		}

		/// <summary>
		/// Изменение сферы отсечения
		/// </summary>
		void RebuildCullingSphere() {
			Frame[] frames = Frames;
			if (frames != null) {
				Vector3 max = Vector3.One * float.MinValue, min = Vector3.One * float.MaxValue;
				foreach (Frame f in frames) {
					MorphFrame cf = (MorphFrame)f;
					if (cf != null) {
						for (int i = 0; i < cf.verts.Length; i += 3) {
							if (cf.verts[i] > max.X) {
								max.X = cf.verts[i];
							}
							if (cf.verts[i + 1] > max.Y) {
								max.Y = cf.verts[i + 1];
							}
							if (-cf.verts[i + 2] > max.Z) {
								max.Z = -cf.verts[i + 2];
							}
							if (cf.verts[i] < min.X) {
								min.X = cf.verts[i];
							}
							if (cf.verts[i + 1] < min.Y) {
								min.Y = cf.verts[i + 1];
							}
							if (-cf.verts[i + 2] < min.Z) {
								min.Z = -cf.verts[i + 2];
							}
						}
						cull = new CullBox();
						cull.Min = min;
						cull.Max = max;
					}
				}
				RebuildParentCull();
			}
		}

		/// <summary>
		/// Морфный кадр анимации
		/// </summary>
		public class MorphFrame : AnimatedMeshComponent.Frame {

			/// <summary>
			/// Вершины
			/// </summary>
			public Vector3[] Vertices {
				get {
					if (verts != null) {
						Vector3[] va = new Vector3[verts.Length / 3];
						for (int i = 0; i < va.Length; i++) {
							int ps = i * 3;
							va[i] = new Vector3(
								verts[ps],
								verts[ps + 1],
								-verts[ps + 2]
							);
						}
						return va;
					}
					return null;
				}
				set {
					if (value != null) {
						verts = new float[value.Length * 3];
						for (int i = 0; i < value.Length; i++) {
							int ps = i * 3;
							verts[ps + 0] = value[i].X;
							verts[ps + 1] = value[i].Y;
							verts[ps + 2] = -value[i].Z;
						}
					} else {
						verts = null;
					}
				}
			}

			/// <summary>
			/// Нормали
			/// </summary>
			public Vector3[] Normals {
				get {
					if (normals != null) {
						Vector3[] na = new Vector3[normals.Length / 3];
						for (int i = 0; i < na.Length; i++) {
							int ps = i * 3;
							na[i] = new Vector3(
								normals[ps],
								normals[ps + 1],
								-normals[ps + 2]
							);
						}
						return na;
					}
					return null;
				}
				set {
					if (value != null) {
						normals = new float[value.Length * 3];
						for (int i = 0; i < value.Length; i++) {
							int ps = i * 3;
							normals[ps + 0] = value[i].X;
							normals[ps + 1] = value[i].Y;
							normals[ps + 2] = -value[i].Z;
						}
					} else {
						verts = null;
					}
				}
			}

			// Скрытые переменные
			internal float[] verts;
			internal float[] normals;
		}
	}
}
