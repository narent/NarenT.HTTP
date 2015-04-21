using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace NarenT.HTTP.Actions
{
	public class JsonActionResult : ActionResult
	{
		public JsonActionResult(object obj) : base()
		{
			this.Data = UTF8Encoding.Default.GetBytes(JsonConvert.SerializeObject (obj));
			this.ContentType = "application/json";
		}
	}
}