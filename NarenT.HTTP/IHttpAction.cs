using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using NarenT.Common;
using System.Net;

namespace NarenT.HTTP
{
	public interface IHttpAction
	{
		ActionResult GET (HttpListenerContext context);

		ActionResult POST (HttpListenerContext context);
	}
}
