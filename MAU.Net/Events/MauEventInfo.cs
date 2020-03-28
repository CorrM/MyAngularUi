using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Events
{
	public struct MauEventInfo
	{
		public string EventName;
		public string TypeName;
		public JObject Data;

		public MauEventInfo(string eventName, string typeName, JObject data)
		{
			EventName = eventName;
			TypeName = typeName;
			Data = data;
		}
	}
}
