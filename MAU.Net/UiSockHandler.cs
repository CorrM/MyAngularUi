using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketNet;
using WebSocketNet.Server;

namespace MAU
{
	internal class UiSockHandler : WebSocketBehavior
	{

		internal static UiSockHandler Instance { get; private set; }

		public UiSockHandler()
		{
			Instance ??= this;
		}

		internal new void Send(string data) => base.Send(data);

		protected override void OnOpen()
		{
			Debug.WriteLine("OPENED");
		}
		protected override void OnMessage(MessageEventArgs e)
		{
			_ = MyAngularUi.Instance().OnMessage(e);
		}
	}
}
