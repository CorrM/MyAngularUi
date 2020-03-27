using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Events
{
	public struct MauEventInfo
	{
		public string TypeName;
		public JObject Data;

		public MauEventInfo(string typeName, JObject data)
		{
			TypeName = typeName;
			Data = data;
		}
	}
}
