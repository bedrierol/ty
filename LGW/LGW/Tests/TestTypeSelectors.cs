
using System;
using LGW.mailType;
using NUnit.Framework;
using System.Collections.Generic;

namespace LGW.Tests
{
	[TestFixture]
	public class TestTypeSelectors
	{
		[Test]
		public void TestMailTypeSelector()
		{
			// Create a mock messageDetail and check type selector for OrderMail
			MessageDetail messageDetail = new MessageDetail(){content="testerorder", status=5,sendHistory=new List<string>{},details="tester", sendDate = new DateTime()};
			IMailType resp =  MailTypeSelector.getTypeForMail(messageDetail.content);
			Assert.IsInstanceOf(typeof(OrderMail),resp);
		}
	}
}
