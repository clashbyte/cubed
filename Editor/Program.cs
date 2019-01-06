using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Cubed.Forms.Common;
using Cubed.Forms.Dialogs;
using Cubed.Properties;
using SharpRaven;
using SharpRaven.Data;

namespace Cubed {
	static class Program {

		/// <summary>
		/// Flag for allowing browsers
		/// </summary>
		public static bool AllowBrowser {
			get;
			private set;
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			AppDomain.CurrentDomain.AssemblyResolve += Resolver;

			#if DEBUG
			// Running app withount handling exceptions
			LoadApp();
			#else
			try {
				// Running app
				LoadApp();
			} catch(Exception ex) {

				// Sending to Sentry
				RavenClient ravenClient = new RavenClient("https://324c1d3a12214e81a38f8d4004d772ba@sentry.io/1360066");
				ravenClient.Capture(new SentryEvent(ex));

				// Showing form
				Application.Run(new ErrorHandlerForm(ex));
			}
			#endif
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void LoadApp() {

			// Check for OpenAL
			if (!Audio.AudioSystem.OpenALExists()) {
				if (MessageDialog.Open("Whoops!", "OpenAL (library for audio playback) is not found. Would you like to install it?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
					try {
						Process.Start(new ProcessStartInfo() {
							FileName = Path.Combine(Directory.GetCurrentDirectory(), "redist/oalinst.exe"),
							UseShellExecute = true,
						});
					} catch(Exception) { }
				};
				return;
			}
			
			// CEF Settings
			var settings = new CefSettings();
			settings.BackgroundColor = Cef.ColorSetARGB(255, 50, 50, 50);
			settings.LogSeverity = LogSeverity.Disable;
			

			// Set BrowserSubProcessPath based on app bitness at runtime
			settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
												   Environment.Is64BitProcess ? "x64" : "x86",
												   "CefSharp.BrowserSubprocess.exe");

			// Make sure you set performDependencyCheck 
			AllowBrowser = Cef.Initialize(settings, false, null);

			// Handling settings
			if (Settings.Default.UpgradeFromPrev) {
				Settings.Default.Upgrade();
				Settings.Default.UpgradeFromPrev = false;
				Settings.Default.Save();
			}

			// Running app
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Creating form
			StartupForm sf = new StartupForm();
			if (!sf.ExitWithoutShow) {
				Application.Run(sf);
			}
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
