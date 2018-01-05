
using System;
using LGW.MessageSender;

namespace LGW
{
	/// <summary>
	/// Description of MailProviderSelector.
	/// </summary>
	public class MailProviderSelector
	{
		public static IMessageSender getProviderForMail(string content){
			// check the content and other factors and choose the right mail provider
			return new ABCProvider();
		}
	}
}
