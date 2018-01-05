
using System;
using LGW.mailType;

namespace LGW
{
	/// <summary>
	/// Description of MailTypeSelector.
	/// </summary>
	public class MailTypeSelector
	{
		public static IMailType getTypeForMail(string content){
			// check the content and other factors and choose the right mail type and sender
			if(content.Contains("order")){
				return new OrderMail();
			}else{
				return new LostPasswordMail();
			}
			
		}
	}
}
