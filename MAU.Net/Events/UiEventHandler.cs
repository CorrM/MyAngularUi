using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Events
{
	public class UiEventHandler
	{
		public delegate void MauEventHandler(string eventType, JObject eventData);
	}
}
