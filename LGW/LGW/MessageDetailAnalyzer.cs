
using System;

namespace LGW
{
	/// <summary>
	/// Description of MessageDetailAnalyzer.
	/// </summary>
	class MessageDetailAnalyzer
	{
		public static MessageDetail Analyze(MessageDetail messageDetail)
		{
			// check the messageDetail and decide to retry or long retry
			// response with modified MessageDetail
			messageDetail.sendHistory.Add("checked");
			return messageDetail;
		}
	}
}
