using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.PowerShell.Ssh.Helpers.Network.Models;
using Microsoft.Azure.Commands.Common.Exceptions;
using Azure.Core;
using Microsoft.Azure.Management.WebSites.Version2016_09_01.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Management.Automation;


public class TunnelServer
{
    private string _localAddr;
    private int _localPort;
    private string _bastionEndpoint;
    private string _remoteHost;
    private string _lastToken;


    private int _remotePort;
    private ClientWebSocket _webSocket;
    private IAzureContext _context;


    public TunnelServer(IAzureContext context, int localPort, BastionHost bastion, string bastionEndpoint, string remoteHost, int remotePort)
    {
        _context = context;
        _localAddr = "localhost";
        _localPort = localPort;
        _bastionEndpoint = bastionEndpoint;
        _remoteHost = remoteHost;
        _remotePort = remotePort;
        _webSocket = new ClientWebSocket();
    }

    public async Task StartBastionTunnelAsync()
    {
        IPAddress localIPAddress = Dns.GetHostAddresses(_localAddr)[0];
        TcpListener listener = new TcpListener(localIPAddress, _localPort);
        listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        listener.Start();
        try
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                string authToken = GetAuthTokenAsync();

                Uri serverUri = new Uri($"wss://{_bastionEndpoint}/omni/webtunnel/{authToken}");

                using (var webSocket = new ClientWebSocket())
                {
                    await webSocket.ConnectAsync(serverUri, CancellationToken.None).ConfigureAwait(false);

                    Task receiveTask = ReceiveFromBastionWebSocketAsync(client, webSocket);
                    Task sendTask = SendToBastionWebSocketAsync(client, webSocket);

                    await Task.WhenAll(receiveTask, sendTask);
                }

                client.Close();
            }
        }
        catch (Exception ex)
        {
            throw new AzPSCloudException($"An error occurred in the tunneling: {ex.Message}");
        }
        finally
        {
            listener.Stop();
        }

    }
    public void StartServer()
    {
        StartBastionTunnelAsync().GetAwaiter().GetResult();
    }

    private string GetAuthTokenAsync()
    {
        string accessToken= _context.Account.GetAccessToken();
        
        var content = new Dictionary<string, string>
        {
            { "resourceId", _remoteHost },
            { "protocol", "tcptunnel" },
            { "workloadHostPort", _remotePort.ToString() },
            { "aztoken", accessToken },
            { "token", _lastToken }
        };

        var stringContent = new FormUrlEncodedContent(content);

        var webAddress = $"https://{_bastionEndpoint}/api/tokens";
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Connection", "close");
            client.DefaultRequestHeaders.Add("User-Agent", "PowerShell");

            HttpResponseMessage response = client.PostAsync(webAddress, stringContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

            if (responseJson != null)
            {
                if (responseJson.ContainsKey("authToken") && responseJson["authToken"] != null)
                {
                    _lastToken = responseJson["authToken"].ToString();
                }
            }
            else
            {
                throw new AzPSCloudException("Invalid response from the server.");
            }
            return responseJson["websocketToken"].ToString();
        }
    }

    private async Task ReceiveFromBastionWebSocketAsync(TcpClient client, ClientWebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        using (var networkStream = client.GetStream())
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    await networkStream.WriteAsync(buffer, 0, result.Count);
                }
            }
        }
    }

    private async Task SendToBastionWebSocketAsync(TcpClient client, ClientWebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        using (var networkStream = client.GetStream())
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }
        }
    }
}
