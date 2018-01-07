using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Components.Rendering {

	/// <summary>
	/// Абстрактный класс анимируемого меша
	/// </summary>
	public abstract class AnimatedMeshComponent : MeshComponent, IRenderable {

		/// <summary>
		/// Длина анимации
		/// </summary>
		public int AnimationLength {
			get {
				Frame[] fr = StepFrames;
				if (fr!=null) {
					return fr.Length-1;
				}
				return 0;
			}
		}

		/// <summary>
		/// Кадры меша
		/// </summary>
		public virtual Frame[] Frames {
			get {
				Frame[] fr = frames;
				MeshComponent me = this;
				while (fr == null) {
					if (me.Proxy != null) {
						me = me.Proxy;
					} else {
						break;
					}
					if (me is AnimatedMeshComponent) {
						fr = (me as AnimatedMeshComponent).frames;
					}
				}
				return fr;
			}
			set {
				frames = value;
				if (frames != null) {
					bool needRecalc = false;
					foreach (Frame fr in frames) {
						if ((fr.Time % 1f)!=0f) {
							needRecalc = true;
							break;
						}
					}
					if (needRecalc) {
						RebuildFrames();
					} else {
						stepFrames = frames;
					}
				} else {
					stepFrames = null;
				}
			}
		}

		/// <summary>
		/// Обработанные кадры
		/// </summary>
		internal virtual Frame[] StepFrames {
			get {
				Frame[] fr = stepFrames;
				MeshComponent me = this;
				while (fr == null) {
					if (me.Proxy != null) {
						me = me.Proxy;
					} else {
						break;
					}
					if (me is AnimatedMeshComponent) {
						fr = (me as AnimatedMeshComponent).stepFrames;
					}
				}
				return fr;
			}
		}

		/// <summary>
		/// Внутренние кадры меша
		/// </summary>
		Frame[] frames;

		/// <summary>
		/// Кадры на каждый фрейм
		/// </summary>
		Frame[] stepFrames;

		/// <summary>
		/// Кадр меша
		/// </summary>
		public abstract class Frame {

			/// <summary>
			/// Время кадра
			/// </summary>
			public float Time {
				get;
				set;
			}

		}

		/// <summary>
		/// Обновление анимации и рендер
		/// </summary>
		void IRenderable.Render() {
			// Обновление переходного кадра
			if (queuedTransitionUpdate != null) {
				UpdateTransition();
				queuedTransitionUpdate = null;
			}

			// Обновление анимации
			if (queuedUpdate != null) {
				UpdateAnimation();
				queuedUpdate = null;
			}

			// Рендер
			HandleRender();
		}

		/// <summary>
		/// Требуется ли обновление анимации
		/// </summary>
		internal QueuedMeshUpdate queuedUpdate;

		/// <summary>
		/// Требуется ли сохранение перехода
		/// </summary>
		internal QueuedMeshUpdate queuedTransitionUpdate;

		/// <summary>
		/// Создание копии текущего кадра для интерполяции
		/// </summary>
		protected abstract void UpdateTransition();

		/// <summary>
		/// Обновление анимации
		/// </summary>
		protected abstract void UpdateAnimation();

		/// <summary>
		/// Поиск и обновление кадров
		/// </summary>
		protected virtual void RebuildFrames() { }

		/// <summary>
		/// Внутреннее запланированное обновление меша
		/// </summary>
		internal class QueuedMeshUpdate {
			/// <summary>
			/// Первый кадр
			/// </summary>
			public int FirstFrame;
			/// <summary>
			/// Последний кадр
			/// </summary>
			public int LastFrame;
			/// <summary>
			/// Текущий кадр
			/// </summary>
			public float Time;
			/// <summary>
			/// Анимация круговая
			/// </summary>
			public bool IsLooping;
			/// <summary>
			/// Играется ли анимация в обратную сторону
			/// </summary>
			public bool IsBackward;
		}
	}
}
