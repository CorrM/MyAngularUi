using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebSocketNet;
using WebSocketNet.Server;

namespace MAU
{
	public class MyAngularUi : IDisposable
	{
		private static Dictionary<string, UiElement> UiElements { get; } = new Dictionary<string, UiElement>();
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

		public static void RegisterUi(UiElement element)
		{
			UiElements.Add(element.Id, element);
		}
		public static void RegisterUi(ICollection<UiElement> element)
		{
			foreach (UiElement uiElement in element)
				UiElements.Add(uiElement.Id, uiElement);
		}

		public static bool GetUiElement(string elementId, out UiElement element)
		{
			if (UiElements.ContainsKey(elementId))
			{
				element = UiElements[elementId];
				return true;
			}

			element = null;
			return false;
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
