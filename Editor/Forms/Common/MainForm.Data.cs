﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cubed.Forms.Common {

	/// <summary>
	/// Project management
	/// </summary>
	public partial class MainForm {

		/// <summary>
		/// Current path to the project
		/// </summary>
		public string CurrentProjectPath {
			get;
			set;
		}

		/// <summary>
		/// Starting 
		/// </summary>
		public StartAction StartingAction {
			get;
			set;
		}

		/// <summary>
		/// Form closing action
		/// </summary>
		[DefaultValue(CloseAction.FullClose)]
		public CloseAction ClosingAction {
			get;
			private set;
		}

		/// <summary>
		/// Form startup action
		/// </summary>
		public enum StartAction {
			Open,
			OpenNew
		}

		/// <summary>
		/// Form close method
		/// </summary>
		public enum CloseAction {
			ProjectClose,
			FullClose
		}

	}
}
