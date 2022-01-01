using System;
using System.Threading.Tasks;
using Fleck;

namespace MAU.WebSocket;

public class MauWebSocket : IDisposable
{
    #region [ Events ]

    public delegate void ConnectCallBack();
    public event ConnectCallBack OnOpen;

    public delegate void CloseCallBack();
    public event CloseCallBack OnClose;

    public delegate void MessageCallBack(string message);
    public event MessageCallBack OnMessage;

    #endregion

    private readonly WebSocketServer _wsServer;
    private IWebSocketConnection WebSock { get; set; }

    public MauWebSocket(int port)
    {
        _wsServer = new WebSocketServer($"ws://0.0.0.0:{port}/MauHandler")
        {
            RestartAfterListenError = true
        };

        FleckLog.LogAction = delegate { };
    }

    public void Start()
    {
        if (IsConnected())
            return;

        _wsServer.Start(socket =>
        {
            WebSock = socket;
            socket.OnOpen = () => OnOpen?.Invoke();
            socket.OnClose = () => OnClose?.Invoke();
            socket.OnMessage = (message) => OnMessage?.Invoke(message);
        });
    }
    public async Task<bool> SendAsync(string data)
    {
        if (WebSock?.IsAvailable != true)
            return false;

        try
        {
            Task sendR = WebSock.Send(data);
            if (sendR is null)
                return false;

            await sendR.ConfigureAwait(false);
        }
        catch (NullReferenceException)
        {
            return false;
        }

        return true;
    }

    public bool IsConnected()
    {
        return WebSock?.IsAvailable == true;
    }

    public void Dispose()
    {
        _wsServer?.Dispose();
    }
}