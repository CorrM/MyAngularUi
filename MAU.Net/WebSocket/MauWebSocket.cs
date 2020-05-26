using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fleck;

namespace MAU.WebSocket
{
	public class MauWebSocket : IDisposable
	{
		#region [ Events ]

		public delegate void OnConnectCallBack();
		public event OnConnectCallBack OnOpen;

		public delegate void OnCloseCallBack();
		public event OnCloseCallBack OnClose;

		public delegate void OnMessageCallBack(string message);
		public event OnMessageCallBack OnMessage;

		#endregion

		private readonly WebSocketServer _wsServer;
		private IWebSocketConnection WebSock { get; set; }

		public MauWebSocket(int port)
		{
			_wsServer = new WebSocketServer($"ws://0.0.0.0:{port}/MauHandler")
			{
				RestartAfterListenError = true
			};
		}

		public void Start()
		{
			_wsServer.Start(socket =>
			{
				WebSock = socket;
				socket.OnOpen = () => OnOpen?.Invoke();
				socket.OnClose = () => OnClose?.Invoke();
				socket.OnMessage = (message) => OnMessage?.Invoke(message);
			});
		}
		public async Task<bool> Send(string data)
		{
			if (WebSock == null || !WebSock.IsAvailable)
				return false;

			await WebSock.Send(data);
			return true;
		}

		public void Dispose()
		{
			_wsServer?.Dispose();
		}
	}
}
