using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebSocketNet;
using WebSocketNet.Server;

namespace MAU
{
	public class MyAngularUi : IDisposable
	{
		public WebSocketServer WebSocket { get; private set; }
		public int Port { get; }

		public MyAngularUi(int webSocketPort)
		{
			Port = webSocketPort;
		}

		public async Task<bool> Start()
		{
			WebSocket = new WebSocketServer(Port);
			WebSocket.AddWebSocketService<UiSockHandler>("/UiHandler");

			await Task.Run(WebSocket.Start);
			return WebSocket.IsListening;
		}

		public void Wait()
		{
			while (!UiSockHandler.Finished)
				Thread.Sleep(1);
		}

		#region IDisposable Support
		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue)
				return;

			if (disposing)
			{
				WebSocket?.Stop();
			}

			_disposedValue = true;
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}
