using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using static MAU.MyAngularUi;

namespace MAU.ReadyElement.Angular
{
	public static class MauMatBottomSheet
	{
		private const string ServiceName = "MatBottomSheet";

		public static void Open()
		{
			var data = new JObject
			{
				{ "serviceName", ServiceName },
				{ "methodName", nameof(Open) },
				{ "methodArgs", new JArray() }
			};

			_ = MyAngularUi.SendRequest("null", RequestType.ServiceMethodCall, data);
		}
	}
}
