using System;
using LGW.mailType;
using LGW.MessageSender;

namespace LGW
{
	/// <summary>
	/// Ornek Programimizda static mail queue yi kullanip, init ile 4 sample message ile dolduruyoruz.
	/// </summary>
	public class Program
	{
		public static void Main()
		{
			// Init or connect to mail request queue
			MailQueue.init();
			MessageDetail mailContent  = MailQueue.peek();
			while(mailContent.status != 0){
				MailProcessor processor = new MailProcessor(MailTypeSelector.getTypeForMail(mailContent.content),MailProviderSelector.getProviderForMail(mailContent.content));
				processor.Process(mailContent);
				mailContent  = MailQueue.peek();
			}
			MailQueue.destroy();
			Console.WriteLine("DONE");
			Console.ReadKey();
		}
	}
}
