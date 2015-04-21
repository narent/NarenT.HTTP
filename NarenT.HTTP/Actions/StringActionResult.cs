using System.Text;
using NarenT.HTTP;

namespace NarenT.HTTP.Actions
{
	public class StringActionResult : ActionResult
	{
		public StringActionResult(string data) : base()
		{
			this.Data = Encoding.UTF8.GetBytes(data);
		}
	}
}