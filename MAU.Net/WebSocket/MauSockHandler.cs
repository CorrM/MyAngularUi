using System.Data;
using System.Diagnostics;

namespace MAU.WebSocket
{
	//internal class MauSockHandler : WebSocketBehavior
	//{
	//	internal static MauSockHandler Instance { get; private set; }

	//	public MauSockHandler()
	//	{
	//		Instance ??= this;
	//	}

	//	internal new bool Send(string data)
	//	{
	//		if (ConnectionState != WebSocketState.Open)
	//			return false;

	//		base.Send(data);
	//		return true;
	//	}

	//	protected override async void OnOpen() => await MyAngularUi.OnOpen();
	//	protected override void OnMessage(MessageEventArgs e) => _ = MyAngularUi.OnMessage(e);
	//	protected override async void OnClose(CloseEventArgs e) => await MyAngularUi.OnClose(e);
	//}
}
