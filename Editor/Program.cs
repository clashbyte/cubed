using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Cubed.Forms.Common;
using Cubed.Forms.Dialogs;

namespace Cubed {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			AppDomain.CurrentDomain.AssemblyResolve += Resolver;
			LoadApp();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void LoadApp() {
			var settings = new CefSettings();
			settings.BackgroundColor = Cef.ColorSetARGB(255, 50, 50, 50);

			// Set BrowserSubProcessPath based on app bitness at runtime
			settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
												   Environment.Is64BitProcess ? "x64" : "x86",
												   "CefSharp.BrowserSubprocess.exe");

			// Make sure you set performDependencyCheck false
			Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

			// Running app
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new StartupForm());
		}

		// Will attempt to load missing assembly from either x86 or x64 subdir
		private static Assembly Resolver(object sender, ResolveEventArgs args) {
			if (args.Name.StartsWith("CefSharp")) {
				string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
				string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
													   Environment.Is64BitProcess ? "x64" : "x86",
													   assemblyName);

				return File.Exists(archSpecificPath)
						   ? Assembly.LoadFile(archSpecificPath)
						   : null;
			}

			return null;
		}
	}
}
