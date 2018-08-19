using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Types;
using Cubed.World;

namespace Cubed.Components {

	/// <summary>
	/// Некоторый компонент от энтити - может быть чем угодно
	/// </summary>
	public abstract class EntityComponent {

		/// <summary>
		/// Включен ли компонент
		/// </summary>
		public bool Enabled;

		/// <summary>
		/// Родитель компонента
		/// </summary>
		public Entity Parent {
			get;
			internal set;
		}

		/// <summary>
		/// Проход рендера
		/// </summary>
		internal virtual TransparencyPass RenditionPass {
			get {
				return TransparencyPass.Opaque;
			}
		}

		/// <summary>
		/// Режим смешивания поверхности
		/// </summary>
		internal virtual BlendingMode RenditionBlending {
			get {
				return BlendingMode.AlphaChannel;
			}
		}

		/// <summary>
		/// Конструктор компонента
		/// </summary>
		public EntityComponent() {
			Enabled = true;
		}

		/// <summary>
		/// Получение бокса для отсечения
		/// </summary>
		/// <returns></returns>
		internal virtual CullBox GetCullingBox() { return null; }

		/// <summary>
		/// Перестройка родительского отсекающего объема
		/// </summary>
		internal void RebuildParentCull() {
			if (Parent!=null) {
				Parent.needCullRebuild = true;
			}
		}

		/// <summary>
		/// Destroying component
		/// </summary>
		internal virtual void Destroy() {}

		/// <summary>
		/// Режим прозрачности
		/// </summary>
		internal enum TransparencyPass {
			/// <summary>
			/// Непрозрачный
			/// </summary>
			Opaque,
			/// <summary>
			/// Альфатест (0-1)
			/// </summary>
			AlphaTest,
			/// <summary>
			/// Полное попиксельное смешивание
			/// </summary>
			Blend
		}

		/// <summary>
		/// Режим смешивания
		/// </summary>
		public enum BlendingMode : byte {
			/// <summary>
			/// Прозрачность текстуры или диффузного цвета
			/// </summary>
			AlphaChannel = 0,
			/// <summary>
			/// Цвет как альфаканал
			/// </summary>
			Brightness = 1,
			/// <summary>
			/// Добавление - цвета пикселей суммируются
			/// </summary>
			Add = 2,
			/// <summary>
			/// Умножение - цвета пикселей перемножаются
			/// </summary>
			Multiply = 3,
			/// <summary>
			/// Непрозрачный - модель принудительно рисуется непрозрачной
			/// </summary>
			ForceOpaque = 4
		}
	}
}
