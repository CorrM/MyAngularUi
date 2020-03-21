using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Events
{
	public class UiEventHandler
	{
		public delegate void MauEventHandler(string eventName, UiEventInfo eventInfo);
	}
}
