using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.PowerShell.Ssh.Helpers.Network.Models;


public class TunnelServer
{
    private string _localAddr;
    private int _localPort;
    private string _bastionEndpoint;
    private string _remoteHost;
    private int _remotePort;
    private ClientWebSocket _webSocket;

    public TunnelServer(IAzureContext context, int localPort, BastionHost bastion, string bastionEndpoint, string remoteHost, int remotePort)
    {
        _localAddr = "localhost";
        _localPort = localPort;
        _bastionEndpoint = bastionEndpoint;
        _remoteHost = remoteHost;
        _remotePort = remotePort;
        _webSocket = new ClientWebSocket();
    }

    public async Task StartServerAsync()
    {
        IPAddress localIPAddress = IPAddress.Parse(_localAddr);
        TcpListener listener = new TcpListener(localIPAddress, _localPort);
        listener.Start();
        Console.WriteLine($"Listening on {_localAddr}:{_localPort}");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");

            string authToken = await GetAuthTokenAsync();

            Uri serverUri = new Uri($"wss://{_bastionEndpoint}/webtunnel/{authToken}");
            await _webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Connected to WebSocket server.");

            Task receiveTask = ReceiveFromWebSocketAsync(client);
            Task sendTask = SendToWebSocketAsync(client);

            await Task.WhenAll(receiveTask, sendTask);

            client.Close();
            _webSocket.Dispose();
        }
    }

    private async Task<string> GetAuthTokenAsync()
    {
        return await Task.FromResult("your_auth_token_here");
    }

    private async Task ReceiveFromWebSocketAsync(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];
        while (_webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                await stream.WriteAsync(buffer, 0, result.Count);
            }
        }
    }

    private async Task SendToWebSocketAsync(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];
        while (_webSocket.State == WebSocketState.Open)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
    }
}