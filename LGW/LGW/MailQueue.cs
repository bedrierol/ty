
using System;
using System.Collections.Generic;

namespace LGW
{
	/// <summary>
	/// Static mailReqQueue. In production systems,
	/// one may use scalable queue implementation like jmx or Kafka
	/// more queue clients can peek in case of rush

	/// </summary>
	public static class MailQueue
	{
		static Queue<MessageDetail> sira = new Queue<MessageDetail>();
		public static void init(){
			
			sira.Enqueue(new MessageDetail(){content="some mail content with order", status=1,sendHistory=new List<string>{},details="11", sendDate = new DateTime()});
			sira.Enqueue(new MessageDetail(){content="some mail content..", status=2,sendHistory=new List<string>{},details="12", sendDate = new DateTime()});
			sira.Enqueue(new MessageDetail(){content="some mail content with more order", status=3,sendHistory=new List<string>{},details="13", sendDate = new DateTime()});
			sira.Enqueue(new MessageDetail(){content="some more mail content..", status=4,sendHistory=new List<string>{},details="14", sendDate = new DateTime()});

		}
		public static void destroy(){
			// Do cleanup operations
		}
		public static MessageDetail peek(){
			// this peek should check sendTime to assure Scheduled mails
			// this peek should check status to assure Retry mails
			// this peek should check status to assure Transient Retry mails
			if (sira.Count > 0)
				return sira.Dequeue();
			else{
				return new MessageDetail(){status=0,sendHistory=new List<string>{},details="0", sendDate = new DateTime()};
			}
		}
		// This method can be exposed as a Rest Method
		public static bool enqueue(MessageDetail messageDetail){
			try{
				sira.Enqueue(messageDetail);
				return true;
			}catch(Exception ex){
				Console.WriteLine(ex.Message);
				return false;
			}
		}
	}
}
