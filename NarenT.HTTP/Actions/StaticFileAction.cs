using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NarenT.HTTP.Actions;

namespace NarenT.HTTP.Actions
{
	public class StaticFileAction : HttpActionBase
	{
		private List<Tuple<string, string>> staticFileLocationMappings = new List<Tuple<string, string>>();

		public StaticFileAction()
		{
		}
		
		#region implemented abstract members of NarenT.Net.HttpAction
		public override ActionResult GET (System.Net.HttpListenerContext context)
		{
			var trimmedhttpActionPath = context.Request.Url.LocalPath.TrimStart('/');
			if (string.IsNullOrWhiteSpace(trimmedhttpActionPath)) {
				trimmedhttpActionPath = "index.html";
			}

			var staticFileLocationMapping = this.staticFileLocationMappings.FirstOrDefault(s => trimmedhttpActionPath.StartsWith(s.Item1));
			var folderPath = "static/";
			if (staticFileLocationMapping != null)
			{
				folderPath = staticFileLocationMapping.Item2;
				trimmedhttpActionPath = trimmedhttpActionPath.Substring(staticFileLocationMapping.Item1.Length);
			}

			var filePath = System.IO.Path.Combine(folderPath, trimmedhttpActionPath);
			if (File.Exists(filePath)) {
				return new StaticFileActionResult(System.IO.File.ReadAllBytes(filePath)) {
					ContentType = MimeTypes.GetMimeType(Path.GetExtension(filePath).TrimStart('.'))
				};
			} else {
				return new ActionResult() { HttpStatusCode = System.Net.HttpStatusCode.NotFound };
			}
		}
		
		public override ActionResult POST (System.Net.HttpListenerContext context)
		{
			throw new System.NotImplementedException();
		}
		
		#endregion

		public void AddStaticFileMapping(string prefix, string folder)
		{
			this.staticFileLocationMappings.Add(new Tuple<string, string>(prefix, folder));
		}
	}

	public class StaticFileActionResult : ActionResult
	{
		public StaticFileActionResult (byte[] data)
		{
			this.Data = data;
		}
	}
}

