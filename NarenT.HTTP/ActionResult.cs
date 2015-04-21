using System;
using System.Net;
using System.Collections.Generic;

namespace NarenT.HTTP
{
	public class ActionResult
	{
		public HttpStatusCode HttpStatusCode {
			get;
			set;
		}

		public byte[] Data {
			get;
			set;
		}

		public string ContentType {
			get;
			set;
		}

		public List<Tuple<string, string>> Headers {
			get;
			set;
		}

		public ActionResult ()
		{
			Data = new byte[] {};
			ContentType = "text/html";
			HttpStatusCode = HttpStatusCode.OK;
			Headers = new List<Tuple<string, string>> ();
		}
	}
}