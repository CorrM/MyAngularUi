using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Fleck;

namespace MAU.WebSocket
{
	public class MauWebSocketServer : IDisposable
	{
		// Thread signal.
		private int _port;
		private TcpListener _server;
		private const string EoL = "\r\n";
		private bool _working;

		public List<TcpClient> Clients { get; }
		public int Port
		{
			get => _port;
			set
			{
				_port = value;
				if (_server == null)
					return;

				_server.Stop();
				_server = new TcpListener(IPAddress.Any, _port);
				_working = false;
				Start();
			}
		}
		public int MaxConnection { get; set; } = 1;
		public int HandleDelay { get; set; } = 8;
		public bool IsListening => _server.Server.IsBound;

		public delegate void SocketEvent(MauWebSocketServer sender, TcpClient client, List<string> data);
		public event SocketEvent OnOpen;
		public event SocketEvent OnMessage;
		public event SocketEvent OnClose;

		public MauWebSocketServer(int port)
		{
			Clients = new List<TcpClient>();
			Port = port;
			_server = new TcpListener(IPAddress.Any, port);
		}

		public void Start()
		{
			if (_working)
				return;

			_working = true;
			_server.Start();

			new Thread(() =>
			{
				try
				{
					while (_working)
					{
						while (Clients.Count >= MaxConnection)
							Thread.Sleep(HandleDelay);

						TcpClient client = _server.AcceptTcpClient();
						Task.Run(() => HandleClient(client));
						Thread.Sleep(HandleDelay);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"SocketException => {e.Message}");
					_server.Stop();
				}
			})
			{ IsBackground = true }.Start();

			var server = new WebSocketServer("ws://0.0.0.0:8181");
			server.Start(socket =>
			{
				socket.OnOpen = () => Console.WriteLine("Open!");
				socket.OnClose = () => Console.WriteLine("Close!");
				socket.OnMessage = (message) => socket.Send(message);
			});
		}
		private async Task HandleClient(TcpClient client)
		{
			IPAddress cIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
			int cIndex = Clients.FindIndex(c => ((IPEndPoint) c.Client.RemoteEndPoint).Address.Equals(cIp));

			if (cIndex != -1)
				Clients[cIndex] = client;
			else
				Clients.Add(client);

			NetworkStream stream = client.GetStream();

			while (client.Connected)
			{
				if (!stream.DataAvailable || client.Available < 3)
				{
					await Task.Delay(1);
					continue;
				}

				try
				{
					// Read data
					int toReadSize = client.Available;
					var data = new byte[toReadSize];
					stream.Read(data, 0, toReadSize);
					string dataStr = Encoding.UTF8.GetString(data);

					// Check data
					if (Regex.IsMatch(dataStr, "^GET", RegexOptions.IgnoreCase))
					{
						Console.WriteLine("=====Handshaking from client=====\n{0}", dataStr);

						// 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
						// 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
						// 3. Compute SHA-1 and Base64 hash of the new value
						// 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
						string swk = Regex.Match(dataStr, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
						string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
						byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
						string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

						// HTTP/1.1 defines the sequence CR LF as the end-of-line marker
						byte[] response = Encoding.UTF8.GetBytes(
							"HTTP/1.1 101 Switching Protocols" + EoL +
							"Connection: Upgrade" + EoL +
							"Upgrade: websocket" + EoL +
							"Sec-WebSocket-Accept: " + swkaSha1Base64 + EoL + EoL);

						stream.Write(response, 0, response.Length);
					}
					else
					{
						bool fin = (data[0] & 0b10000000) != 0; // full message has been sent .?
						int opCode = data[0] & 0b00001111; // expecting 1 - text message

						bool mask = (data[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"
						int msgLen = data[1] - 128; // & 0111 1111
						int offset = 2;

						if (msgLen == 126)
						{
							// was ToUInt16(bytes, offset) but the result is incorrect
							msgLen = BitConverter.ToUInt16(new[] { data[3], data[2] }, 0);
							offset = 4;
						}
						else if (msgLen == 127)
						{
							Console.WriteLine("TODO: msgLen == 127, needs qword to store msglen");
							// i don't really know the byte order, please edit this
							// msgLen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);
							// offset = 10;
						}

						if (msgLen == 0)
						{
							Console.WriteLine("msgLen == 0");
						}
						else if (mask)
						{
							var decoded = new byte[msgLen];
							byte[] masks = { data[offset], data[offset + 1], data[offset + 2], data[offset + 3] };
							offset += 4;

							for (int i = 0; i < msgLen; ++i)
								decoded[i] = (byte)(data[offset + i] ^ masks[i % 4]);

							string text = Encoding.UTF8.GetString(decoded);
							OnReceive(client, text);
						}
						else
						{
							Console.WriteLine("mask bit not set");
						}

						Console.WriteLine();
					}

				}
				catch
				{
					CloseSocket(client);
					return;
				}
			}
		}
		private void OnReceive(TcpClient client, string data)
		{
			
		}
		public void Send(TcpClient client, params string[] data)
		{
			//string dataStr = Utils.Aes256Encrypt(
			//	string.Join(separator, data),
			//	Utils.PacketKey.Substring(0, 32),
			//	Utils.PacketKey.Substring(0, 16)
			//) + "<EOF>";

			//byte[] byteData = Encoding.UTF8.GetBytes(dataStr);

			//try
			//{
			//	NetworkStream stream = client.GetStream();
			//	stream.Write(byteData, 0, byteData.Length);
			//}
			//catch
			//{
			//	CloseSocket(client);
			//}
		}

		public void CloseSocket(TcpClient client)
		{
			Clients.Remove(client);
			client.Close();
			client.Dispose();
		}
		public void Dispose()
		{
			_working = false;

			int i = 0;
			while (Clients.Count > 0)
			{
				CloseSocket(Clients[i]);
				i++;
			}

			_server.Stop();
		}
	}
}
