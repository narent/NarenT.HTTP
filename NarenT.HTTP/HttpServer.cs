using System;
using System.Net;
using NarenT.Common;
using System.Linq;

namespace NarenT.HTTP
{
	public enum UrlScheme
	{
		http,
		https
	}

	public delegate void RequestReceivedDelegate (string path);

	public class Server
	{
		//private static System.Threading.AutoResetEvent ListenForNextRequest = new System.Threading.AutoResetEvent(false);
		public UrlScheme Scheme {
			get;
			private set;
		}

		public string Host {
			get;
			private set;
		}

		public int Port {
			get;
			private set;
		}

		public string ApplicationRootPath {
			get;
			private set;
		}

		public string Prefix {
			get;
			private set;
		}

		private HttpListener Listener {
			get;
			set;
		}

		public bool IsListening {
			get {
				return this.Listener != null && this.Listener.IsListening;
			}
		}

		public RoutingEngine RoutingEngine
		{
			get;
			set;
		}

		public event RequestReceivedDelegate RequestReceived;

		public Server (UrlScheme scheme, string host, int port, string applicationRootPath, RoutingEngine routingEngine = null)
		{
			Scheme = scheme;
			Host = host;
			Port = port;
			ApplicationRootPath = applicationRootPath;
			if (String.IsNullOrEmpty (ApplicationRootPath))
				ApplicationRootPath = "/";
			if (!applicationRootPath.StartsWith ("/"))
				ApplicationRootPath = String.Format ("/{0}", ApplicationRootPath);
			if (!applicationRootPath.EndsWith ("/"))
				ApplicationRootPath = String.Format ("{0}/", ApplicationRootPath);
			//Prefix = String.Format("{0}://{1}:{2}{3}", Scheme.ToString(), Host, Port.ToString(), ApplicationRootPath);
			Prefix = String.Format ("{0}://{1}:{2}/", Scheme.ToString (), Host, Port.ToString ());
			this.RoutingEngine = routingEngine ?? new RoutingEngine ();
		}

		public void Start ()
		{
			if (!HttpListener.IsSupported) {
				Console.WriteLine ("HttpListener is not supported");
				return;
			}

			this.Listener = new HttpListener ();
			this.Listener.Prefixes.Add (Prefix);
			this.Listener.Start ();
			this.Listener.BeginGetContext (new AsyncCallback (ListenerCallback), Listener);
			Console.WriteLine ("Listening has started...");
		}

		public void Stop ()
		{
			if (Listener != null && Listener.IsListening) {
				Listener.Stop ();
			}

			if (Listener != null) {
				Listener.Close ();
			}
		}

		private ActionResult GetResult(HttpListenerContext context, string httpActionPath)
		{
			var request = context.Request;
			var route = this.RoutingEngine.Resolve(httpActionPath);
			var httpAction = route.Into(r => r.ActionFactory());
			return httpAction.Into(a => {
				try {
					if (request.HttpMethod == "GET") {
						return a.GET (context);
					} else if (request.HttpMethod == "POST") {
						return a.POST (context);
					}

					return new ActionResult () { HttpStatusCode = HttpStatusCode.MethodNotAllowed };
				} catch (Exception ex) {
					return new ActionResult ()
					{
						HttpStatusCode = HttpStatusCode.InternalServerError,
						Data = GetExceptionResponseData(ex),
					};
				}

			}).Or(new ActionResult () { HttpStatusCode = HttpStatusCode.NotFound }).First();
		}

		private void ListenerCallback (IAsyncResult result)
		{
			HttpListener listener = (HttpListener)result.AsyncState;
			HttpListenerContext context = listener.EndGetContext (result);
			HttpListenerRequest request = context.Request;
			Console.WriteLine (request.RawUrl);
			Console.WriteLine (request.Url.ToString ());
			Console.WriteLine (request.HttpMethod);
			var httpActionPath = request.Url.LocalPath;
			Console.WriteLine ("Got request for {0}", httpActionPath);
			var ar = GetResult(context, httpActionPath);
			this.RequestReceived.SomeOrNone ()
				.IntoAction (onRequestReceived => {
					ar.SomeOrNone().When (r => r.HttpStatusCode == HttpStatusCode.OK || r.HttpStatusCode == HttpStatusCode.Accepted)
						.IntoAction (r => onRequestReceived (httpActionPath));
				});

			context.Response.StatusCode = (int)ar.HttpStatusCode;
			context.Response.StatusDescription = ar.HttpStatusCode.ToString ();
			context.Response.ContentType = ar.ContentType;
			foreach (var header in ar.Headers) {
				context.Response.AddHeader (header.Item1, header.Item2);
			}

			if (ar.HttpStatusCode != HttpStatusCode.OK && ar.Data.Length == 0) {
				context.Response.ContentType = "text/html";
				byte[] descriptiveError = System.Text.Encoding.UTF8.GetBytes (ar.HttpStatusCode.ToString ());
				context.Response.ContentLength64 = descriptiveError.Length;
				System.IO.Stream output = context.Response.OutputStream;
				output.Write (descriptiveError, 0, descriptiveError.Length);
				output.Close ();
			} else {
				context.Response.ContentLength64 = ar.Data.Length;
				System.IO.Stream output = context.Response.OutputStream;
				output.Write (ar.Data, 0, ar.Data.Length);
				output.Close ();
			}
			// Wait for a new request
			Console.WriteLine ("Listening again...");
			Listener.BeginGetContext (new AsyncCallback (ListenerCallback), Listener);
		}

		private byte[] GetExceptionResponseData (Exception ex)
		{
			return System.Text.Encoding.UTF8.GetBytes (String.Format ("<pre>{0}\n{1}<pre>", ex.Message, ex.StackTrace));
		}
	}
}