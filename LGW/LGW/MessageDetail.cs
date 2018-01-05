
using System;
using System.Collections.Generic;

namespace LGW
{
	/// <summary>
	/// Description of MessageDetail.
	/// </summary>
	public class MessageDetail
	{
		public string content;
		public int status;
		public DateTime sendDate;
		public string details;
		public List<string> sendHistory = new List<string>();

	}
}
