
using System;

namespace LGW.MessageSender
{
	/// <summary>
	/// Description of CDCProvider.
	/// </summary>
	public class CDCProvider : IMessageSender
	{
		public MessageDetail SendMessage(string message)
		{
			MessageDetail ret = new MessageDetail();
			ret.sendHistory.Add(String.Format("CDCProvider : {0}", message));
			return ret;
			
		}
	}
}
