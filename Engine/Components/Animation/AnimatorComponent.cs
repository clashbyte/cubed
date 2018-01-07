using SpriteBoy.Engine.Components.Rendering;
using SpriteBoy.Engine.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SpriteBoy.Engine.Components.Animation {

	/// <summary>
	/// Аниматор для меша
	/// </summary>
	public class AnimatorComponent : EntityComponent, IUpdatable {

		/// <summary>
		/// Количество кадров
		/// </summary>
		public int FrameCount {
			get {
				return MaxTime;
			}
		}

		/// <summary>
		/// Максимальное время
		/// </summary>
		public int MaxTime {
			get {
				AnimatedMeshComponent[] ms = Parent.GetComponents<AnimatedMeshComponent>();
				int sz = 0;
				foreach (AnimatedMeshComponent m in ms) {
					int l = m.AnimationLength;
					if (l > sz) {
						sz = l;
					}
				}
				return sz;
			}
		}

		/// <summary>
		/// Режим анимации
		/// </summary>
		public AnimationMode Mode {
			get;
			private set;
		}

		/// <summary>
		/// Играется ли анимация
		/// </summary>
		public bool Playing {
			get;
			private set;
		}

		/// <summary>
		/// Первый кадр
		/// </summary>
		public int FirstFrame {
			get;
			private set;
		}

		/// <summary>
		/// Последний кадр
		/// </summary>
		public int LastFrame {
			get;
			private set;
		}

		/// <summary>
		/// Скорость проигрывания
		/// </summary>
		public float Speed {
			get {
				return speed;
			}
			set {
				speed = value;
			}
		}

		/// <summary>
		/// Идёт переход
		/// </summary>
		public bool IsTransition {
			get;
			private set;
		}

		/// <summary>
		/// Текущее время
		/// </summary>
		public float Time {
			get {
				if (IsTransition) {
					return -time / transLength;
				}
				if (Speed > 0) {
					return (float)FirstFrame + time;
 				}
				return (float)LastFrame - time;
			}
		}
		
		/// <summary>
		/// Текущее время
		/// </summary>
		float time;

		/// <summary>
		/// Скорость проигрывания
		/// </summary>
		float speed;

		/// <summary>
		/// Длина перехода
		/// </summary>
		float transLength;

		/// <summary>
		/// Последний апдейт однопроходной анимации
		/// </summary>
		bool oneShotUpdate;

		/// <summary>
		/// Анимированные меши
		/// </summary>
		AnimatedMeshComponent[] meshes;

		/// <summary>
		/// Обновление анимации
		/// </summary>
		internal override void Update() {
			if (Playing || oneShotUpdate) {

				// Рассчёт текущих значений
				float currentTime = 0f;
				if (IsTransition) {
					currentTime = time / transLength;
				} else {
					if (speed < 0) {
						currentTime = LastFrame - time;
						if (currentTime < FirstFrame) {
							currentTime = LastFrame + 1f - (currentTime % (LastFrame - FirstFrame + 1f));
						}
					} else {
						currentTime = FirstFrame + time;
					}
				}

				// Установка анимации
				foreach (AnimatedMeshComponent mc in meshes) {
					AnimatedMeshComponent.QueuedMeshUpdate qu = new AnimatedMeshComponent.QueuedMeshUpdate();
					qu.FirstFrame = FirstFrame;
					qu.LastFrame = LastFrame;
					qu.IsLooping = Mode == AnimationMode.Loop;
					qu.IsBackward = speed < 0;
					qu.Time = currentTime;
					mc.queuedUpdate = qu;
				}

				// Обвновление логики
				if (IsTransition) {
					time += 1f;
					if (time >= 0) {
						IsTransition = false;
					}
				}else{
					if (speed == 0 || FirstFrame == LastFrame) {
						Playing = false;
					} else {
						time += Math.Abs(speed);
						if (time > LastFrame - FirstFrame) {
							switch (Mode) {
								case AnimationMode.OneShot:
									time = LastFrame;
									if (oneShotUpdate) {
										Playing = false;
										oneShotUpdate = false;
									} else {
										oneShotUpdate = true;
									}
									break;
								case AnimationMode.Loop:
									time = time % (LastFrame - FirstFrame + 1f);
									break;
								case AnimationMode.PingPongLoop:
									time = time % (LastFrame - FirstFrame);
									speed *= -1;
									break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Установка кадра без анимирования 
		/// </summary>
		/// <param name="frame">Время для установки</param>
		public void StopWithFrame(float frame) {
			Stop();
			if (meshes == null) {
				meshes = Parent.GetComponents<AnimatedMeshComponent>();
			}
			if (meshes != null) {
				int max = MaxTime;
				foreach (AnimatedMeshComponent m in meshes) {
					AnimatedMeshComponent.QueuedMeshUpdate qu = new AnimatedMeshComponent.QueuedMeshUpdate();
					qu.FirstFrame = 0;
					qu.LastFrame = max;
					qu.IsLooping = false;
					qu.Time = 0;
					if (qu.LastFrame > 0) {
						qu.Time = Math.Min(frame, qu.LastFrame);;
					}
					m.queuedUpdate = qu;
				}
				speed = 0;
				FirstFrame = 0;
				LastFrame = max;
				time = frame;
			}
		}

		/// <summary>
		/// Остановка анимации
		/// </summary>
		public void Stop() {
			float currentTime = 0f;
			if (IsTransition) {
				currentTime = time / transLength;
			} else {
				if (speed < 0) {
					currentTime = LastFrame - time;
					if (currentTime < FirstFrame) {
						currentTime = LastFrame + 1f - (FirstFrame - currentTime);
					}
				} else {
					currentTime = FirstFrame + time;
				}
			}
			if (meshes!=null) {
				foreach (AnimatedMeshComponent m in meshes) {
					AnimatedMeshComponent.QueuedMeshUpdate qu = new AnimatedMeshComponent.QueuedMeshUpdate();
					qu.FirstFrame = FirstFrame;
					qu.LastFrame = LastFrame;
					qu.IsLooping = Mode == AnimationMode.Loop;
					qu.Time = currentTime;
					m.queuedUpdate = qu;
				}
			}
			
			Playing = false;
		}

		/// <summary>
		/// Анимирование меша
		/// </summary>
		/// <param name="from">Начальное время</param>
		/// <param name="to">Конечное время</param>
		/// <param name="mode">Режим анимации</param>
		/// <param name="transition">Количество переходных кадров</param>
		public void Animate(int from, int to, float spd = 1f, AnimationMode mode = AnimationMode.Loop, float transition = 0f) {
			meshes = Parent.GetComponents<AnimatedMeshComponent>();
			if (meshes!=null) {

				// Сохранение старых значений анимации
				int oldFirstFrame = FirstFrame;
				int oldLastFrame = LastFrame;
				float oldSpeed = speed;
				if (oldLastFrame == 0) {
					oldLastFrame = MaxTime;
				}
				float currentTime = 0f;
				if (IsTransition) {
					currentTime = time / transLength;
				} else {
					if (speed < 0) {
						currentTime = oldLastFrame - time;
						if (currentTime < oldFirstFrame) {
							currentTime = oldLastFrame + 1f - (oldFirstFrame - currentTime);
						}
					} else {
						currentTime = oldFirstFrame + time;
					}
				}
				bool allowRepeat = Mode == AnimationMode.Loop;

				// Запись новых значений
				speed = spd;
				FirstFrame = from;
				LastFrame = to;
				Mode = mode;
				IsTransition = false;
				if (FirstFrame>LastFrame) {
					int d = FirstFrame;
					FirstFrame = LastFrame;
					LastFrame = d;
				}
				int maxFrame = MaxTime;
				if (FirstFrame < 0) {
					FirstFrame = 0;
				}
				if (LastFrame > maxFrame) {
					LastFrame = maxFrame;
				}
				FirstFrame = FirstFrame;
				LastFrame = LastFrame;

				// Создание событие перехода
				if (transition > 0) {
					foreach (AnimatedMeshComponent m in meshes) {
						AnimatedMeshComponent.QueuedMeshUpdate qu = new AnimatedMeshComponent.QueuedMeshUpdate();
						qu.FirstFrame = oldFirstFrame;
						qu.LastFrame = oldLastFrame;
						qu.IsLooping = allowRepeat;
						qu.Time = currentTime;
						m.queuedTransitionUpdate = qu;
					}
					time = -transition;
					transLength = transition;
					IsTransition = true;
				}else{
					time = 0f;
				}
				Playing = true;
			}
		}

		/// <summary>
		/// Тип анимации
		/// </summary>
		public enum AnimationMode {
			/// <summary>
			/// Только в одну сторону
			/// </summary>
			OneShot,
			/// <summary>
			/// Повтор
			/// </summary>
			Loop,
			/// <summary>
			/// Повтор туда-обратно
			/// </summary>
			PingPongLoop
		}
	}
}
