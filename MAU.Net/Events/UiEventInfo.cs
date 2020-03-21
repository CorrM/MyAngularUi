using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Events
{
	public struct UiEventInfo
	{
		public string TypeName;
		public JObject Data;

		public UiEventInfo(string typeName, JObject data)
		{
			TypeName = typeName;
			Data = data;
		}
	}
}
