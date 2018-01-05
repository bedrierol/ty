
using System;

namespace LGW.mailType
{
	/// <summary>
	/// Description of OrderMail.
	/// </summary>
	public class OrderMail : IMailType
	{
		public string GetMessageTemplate(string content)
		{
			return "OrderMail" + content;
		}
	}
}
