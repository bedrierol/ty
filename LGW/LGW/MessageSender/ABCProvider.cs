
using System;

namespace LGW.MessageSender
{
	/// <summary>
	/// Description of ABCProvider.
	/// </summary>
	public class ABCProvider : IMessageSender
	{
		public MessageDetail SendMessage(string message)
		{
			MessageDetail ret = new MessageDetail();
			ret.sendHistory.Add(String.Format("ABCProvider : {0}", message));
			return ret;
		}
	}
}
