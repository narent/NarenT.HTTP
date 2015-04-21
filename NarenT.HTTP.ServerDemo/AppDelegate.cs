using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.Net.NetworkInformation;
using NarenT.HTTP;
using NarenT.HTTP.Actions;
using NarenT.Common;

namespace NarenT.HTTP.ServerDemo
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		private Server HttpServer;
		private RoutingEngine RoutingEngine;

		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			// If you have defined a root view controller, set it here:
			// window.RootViewController = myViewController;

			StartHttpServer ();

			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}

		private void StartHttpServer()
		{
			string ipAddress = null;
			var wifi = NetworkInterface.GetAllNetworkInterfaces().Where(iff => iff.Name == "en0").FirstOrDefault();
			if (wifi != null)
			{
				ipAddress = wifi.GetIPProperties()
					.UnicastAddresses
					.Where(address => address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					.Select(address => address.Address.ToString()).FirstOrDefault();
			}

			this.RoutingEngine = new RoutingEngine ()
				.Tap(r => r.AddRoute (() => new StaticFileAction (), (s) => true));
			this.HttpServer = new Server(UrlScheme.http, "localhost", 8080, "/", this.RoutingEngine);
			Console.WriteLine ("http server running on " + ipAddress + ":8080");
			this.HttpServer.Start();
		}
	}
}