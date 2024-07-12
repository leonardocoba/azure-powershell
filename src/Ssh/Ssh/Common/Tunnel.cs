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

            string authToken =  GetAuthTokenAsync();

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
    public void StartServer()
    {
        StartServerAsync().GetAwaiter().GetResult();
    }

    private string GetAuthTokenAsync()
    {
        return "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1HTHFqOThWTkxvWGFGZnBKQ0JwZ0I0SmFLcyIsImtpZCI6Ik1HTHFqOThWTkxvWGFGZnBKQ0JwZ0I0SmFLcyJ9.eyJhdWQiOiJodHRwczovL21hbmFnZW1lbnQuY29yZS53aW5kb3dzLm5ldC8iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNzIwNzM4ODAxLCJuYmYiOjE3MjA3Mzg4MDEsImV4cCI6MTcyMDc0Mzk3MywiX2NsYWltX25hbWVzIjp7Imdyb3VwcyI6InNyYzEifSwiX2NsYWltX3NvdXJjZXMiOnsic3JjMSI6eyJlbmRwb2ludCI6Imh0dHBzOi8vZ3JhcGgud2luZG93cy5uZXQvNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3L3VzZXJzLzRmNGM0MDhkLTVjOGQtNGJhYS05MmExLTlhZDIyZTZkMTZkYy9nZXRNZW1iZXJPYmplY3RzIn19LCJhY3IiOiIxIiwiYWlvIjoiQVpRQWEvOFhBQUFBWUNjVnVNZThKaUFlZmpMYWZpZGZIbjJqeWhxNTZBQnBFQi9iOC9ic3E4d3JRNXkxSkZGZVFTdGtTTXhvb2FpVStMTVJSM0hYVXNvbEdwL3NuLytpRC9XcHNtL05aSkI1eDJ5UGpCblkvdFovNkQwM0FkQlNncnQzTWw0cUI0WXBjN0pZWXpvemxtZm4zdUgxeUFKT0ZoaFdweGdzMjJ1YjMyano5cGp4TFZvd2oxMDM5Znpxc05FUG1LTDJLcmt2IiwiYW1yIjpbInJzYSIsIm1mYSJdLCJhcHBpZCI6IjA0YjA3Nzk1LThkZGItNDYxYS1iYmVlLTAyZjllMWJmN2I0NiIsImFwcGlkYWNyIjoiMCIsImNhcG9saWRzX2xhdGViaW5kIjpbIjI5Mzk5Y2Y5LTliNmItNDIwNS1iNWIzLTEzYTEzNGU5YjIzMyJdLCJkZXZpY2VpZCI6IjljNTU3Mjc3LTcwODYtNDg1My04NjI5LTg1NTVkMjY2NTg3MyIsImZhbWlseV9uYW1lIjoiQ29iYWxlZGEiLCJnaXZlbl9uYW1lIjoiTGVvbmFyZG8iLCJpZHR5cCI6InVzZXIiLCJpcGFkZHIiOiIyMDAxOjQ4OTg6YTgwMDoxMDEwOjhjM2Q6N2YyZTozNTM5OmVhMmYiLCJuYW1lIjoiTGVvbmFyZG8gQ29iYWxlZGEiLCJvaWQiOiI0ZjRjNDA4ZC01YzhkLTRiYWEtOTJhMS05YWQyMmU2ZDE2ZGMiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMjEyNzUyMTE4NC0xNjA0MDEyOTIwLTE4ODc5Mjc1MjctNzY2NTE3NTAiLCJwdWlkIjoiMTAwMzIwMDM3QTBGQTlBOCIsInJoIjoiMC5BUm9BdjRqNWN2R0dyMEdScXkxODBCSGJSMFpJZjNrQXV0ZFB1a1Bhd2ZqMk1CTWFBSlkuIiwic2NwIjoidXNlcl9pbXBlcnNvbmF0aW9uIiwic3ViIjoiWXBJeVNtZC13b0JCSzQyVVd0M3ZXajduSFNMdlhvdlBzZU52b0Ezck9mMCIsInRpZCI6IjcyZjk4OGJmLTg2ZjEtNDFhZi05MWFiLTJkN2NkMDExZGI0NyIsInVuaXF1ZV9uYW1lIjoidC1sY29iYWxlZGFAbWljcm9zb2Z0LmNvbSIsInVwbiI6InQtbGNvYmFsZWRhQG1pY3Jvc29mdC5jb20iLCJ1dGkiOiJUejlHQ2l0NU4wVzd1Z1lub0hITkFBIiwidmVyIjoiMS4wIiwid2lkcyI6WyJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXSwieG1zX2NhZSI6IjEiLCJ4bXNfY2MiOlsiQ1AxIl0sInhtc19maWx0ZXJfaW5kZXgiOlsiMjYiXSwieG1zX2lkcmVsIjoiMSAyOCIsInhtc19yZCI6IjAuNDJMbFlCUmlsQUlBIiwieG1zX3NzbSI6IjEiLCJ4bXNfdGNkdCI6MTI4OTI0MTU0N30.qJaPF6NuMMwgirY_ZW42dnZblvG1Ym7m--cjq6Oq38IHAcIUQ1JHRMbMk63hkjJWOjCdVUwrXiia6Yc3pVR1Ssf2hcdU36IBLyNVSg7RUtqw5PsXRwDdtzyUZambed6OBbuTFJBtOjEcqnpOJCoj4c7BUZX9VCe5uzT1kBBP_ArSb_O2UK9T27TkHIqwNexX5Wh88snkpjGpQGqR3bz7N4FGx9M4sRJT7j66IFw3T96xkdQQQn--u3UOCDOH47j1wXgBoNIuLBtYc3lYKayJDIEF8xJsphctNpvD6z3i3IMBuTVikPwbIXF8ZWMxZTmyUJTgMJkSuv5yBc77Zt5C3w";

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