using Newtonsoft.Json.Linq;
using static MAU.MyAngularUi;

namespace MAU.ReadyComponents.Angular
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

			MyAngularUi.SendRequest("null", RequestType.ServiceMethodCall, data);
		}
	}
}
