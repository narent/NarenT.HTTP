using System;
using System.Net;

namespace NarenT.HTTP
{
	public abstract class HttpActionBase : IHttpAction
	{
		public HttpActionBase ()
		{
		}

		public abstract ActionResult GET (HttpListenerContext context);

		public abstract ActionResult POST (HttpListenerContext context);

		protected void WriteStringToResponse (HttpListenerContext context, string str)
		{
			HttpListenerResponse response = context.Response;
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes (str);
			response.ContentLength64 = buffer.Length;
			System.IO.Stream output = response.OutputStream;
			output.Write (buffer, 0, buffer.Length);
			output.Close ();
		}

		protected string[] GetParts (string httpActionPath)
		{
			if (httpActionPath == "/" || string.IsNullOrEmpty (httpActionPath))
				return new string[] {};
			else
				return httpActionPath.Remove (0, 1).Split (new string[] { "/" }, StringSplitOptions.None);	
		}
	}
}

