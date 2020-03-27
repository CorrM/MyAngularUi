using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketNet;
using WebSocketNet.Server;

namespace MAU.WebSocket
{
	internal class MauSockHandler : WebSocketBehavior
	{
		internal static MauSockHandler Instance { get; private set; }

		public MauSockHandler()
		{
			Instance ??= this;
		}

		internal new bool Send(string data)
		{
			if (ConnectionState != WebSocketState.Open)
				return false;

			base.Send(data);
			return true;
		}

		protected override void OnOpen()
		{
			Debug.WriteLine("OPENED");
		}
		protected override void OnMessage(MessageEventArgs e)
		{
			_ = MyAngularUi.OnMessage(e);
		}
	}
}
