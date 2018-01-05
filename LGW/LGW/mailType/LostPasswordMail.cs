
using System;

namespace LGW.mailType
{
	/// <summary>
	/// Description of LostPasswordMail.
	/// </summary>
	public class LostPasswordMail : IMailType
	{
		public string GetMessageTemplate(string content)
		{
			return "LostPasswordMail"+content;
		}
	}
}
