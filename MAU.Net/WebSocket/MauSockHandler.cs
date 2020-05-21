using System.Diagnostics;
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

		protected override void OnOpen() => _ = MyAngularUi.OnOpen();
		protected override void OnMessage(MessageEventArgs e) => _ = MyAngularUi.OnMessage(e);
		protected override void OnClose(CloseEventArgs e) => _ = MyAngularUi.OnClose(e);
	}
}
