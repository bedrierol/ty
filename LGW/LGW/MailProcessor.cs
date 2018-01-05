using System;
using LGW.mailType;
using LGW.MessageSender;

namespace LGW
{
	/// <summary>
	/// Mail Processor, mail mesajlarinin type larini belirleyen
	/// senderProvider larini atayan ve gonderimleri yapip, sonuclarini analiz edip tekrar
	/// yonlendirmeyi yapan ana kisimdir.
	/// Bu yapi farkli process lerde coklanarak scale edilebilir.
	/// 
	/// </summary>
	public class MailProcessor
	{
		IMailType mailType = null;
		IMessageSender messageSender;
		public MailProcessor(IMailType _mailType, IMessageSender _messageSender)
		{
			mailType = _mailType;
			messageSender = _messageSender;
		}
		public void Process(MessageDetail messageDetail)
		{
			// write message based on template
			Console.WriteLine(mailType.GetMessageTemplate(messageDetail.content));
			// send message via provider
			MessageDetail respMessageDetail = messageSender.SendMessage(messageDetail.content);
			MessageDetailAnalyzer.Analyze(respMessageDetail);
		}

	}
}
