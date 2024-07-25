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
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Azure.Commands.Common.Authentication;


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
    private TcpListener _listener;


    public TunnelServer(IAzureContext context, int localPort, BastionHost bastion, string bastionEndpoint, string remoteHost, int remotePort)
    {
        _context = context;
        _localAddr = "localhost";  // Local address for TCP listener
        _localPort = localPort;  // Automatically select an available port if localPort is 0
        _bastionEndpoint = bastionEndpoint;  // Bastion endpoint for WebSocket connection
        _remoteHost = remoteHost;  // Remote host to connect to
        _remotePort = remotePort;  // Remote port on the remote hos
        _webSocket = new ClientWebSocket();
    }

    public async Task StartBastionTunnelAsync()
    {
        IPAddress localIPAddress = Dns.GetHostAddresses(_localAddr)[0];
        _listener = new TcpListener(localIPAddress, _localPort);
        _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);


        _listener.Start();
        try
        {
            while (true)
            {
                // Accepts incoming TCP client connection
                TcpClient client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"Client connected on local port: {_localPort}");


                string authToken = GetAuthTokenAsync();

                Uri serverUri = new Uri($"wss://{_bastionEndpoint}/omni/webtunnel/{authToken}");

                using (var webSocket = new ClientWebSocket())
                {
                    try
                    {
                        await webSocket.ConnectAsync(serverUri, CancellationToken.None).ConfigureAwait(false);

                        Task receiveTask = ReceiveFromBastionWebSocketAsync(client, webSocket);
                        Task sendTask = SendToBastionWebSocketAsync(client, webSocket);

                        await Task.WhenAll(receiveTask, sendTask);
                    }
                    catch (WebSocketException ex)
                    {
                        Console.WriteLine($"WebSocket error: {ex.Message}"); 
                        await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "WebSocket error", CancellationToken.None);
                    }
                }

                client.Close();
            }
        }
      
        catch (Exception ex) when (!(ex is TaskCanceledException))
        {
            throw new AzPSCloudException($"An error occurred in the tunneling: {ex.Message}");
        }
        finally
        {
            Cleanup();
        }

    }
    public void StartServer()

    {
        try
        {
            StartBastionTunnelAsync().GetAwaiter().GetResult();
        }
        finally
        {
            Cleanup();
        }
    }

    private string GetAuthTokenAsync()
    {
        IAccessToken accessToken = AzureSession.Instance.AuthenticationFactory.Authenticate(
                                       _context.Account,
                                       _context.Environment,
                                       _context.Tenant?.Id,
                                       null,
                                       ShowDialog.Never,
                                       null
                                       );

        var content = new Dictionary<string, string>
        {
            { "resourceId", _remoteHost },
            { "protocol", "tcptunnel" },
            { "workloadHostPort", _remotePort.ToString() },
            { "aztoken", accessToken.AccessToken },
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
        // The buffer size of 4 KB (4096 bytes) was chosen for the following reasons:
        // 1. Performance and Memory Balance:
        //    A 4 KB buffer size strikes an optimal balance between performance and memory usage. Larger buffers can consume 
        //    more memory and smaller buffers might result in more read/write operations which can degrade performance.
        // 2. Network Efficiency: 
        //    4 KB is a commonly used buffer size that aligns well with typical TCP packet sizes, helping to reduce fragmentation and 
        //    reassembly overhead.
        // 3. Consistency with Azure CLI: 
        //    This buffer size is consistent with the buffer size used by the Azure CLI bastion extension.


        var buffer = new byte[1024 * 4]; // Buffer for data transfer (4 KB)
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
                    await networkStream.WriteAsync(buffer, 0, result.Count); //Write data to TCP client
                }
            
            }
        }
    }

    private async Task SendToBastionWebSocketAsync(TcpClient client, ClientWebSocket webSocket)
    {
        var buffer = new byte[1024 * 4]; // Buffer for data transfer (4 KB)
        using (var networkStream = client.GetStream())
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), WebSocketMessageType.Binary, true, CancellationToken.None); //Send data to WebSocket
                }
            }
        }
    }
    public int GetAvailablePort()
    {
        int availablePort;
        using (Socket tempSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            tempSocket.Bind(new IPEndPoint(IPAddress.Loopback, 0)); // Bind to an available port
            availablePort = ((IPEndPoint)tempSocket.LocalEndPoint).Port;
        }

        return availablePort;
    }
    private void Cleanup()
    {
        if (_listener != null)
        {
            try
            {
                _listener.Stop();
            }
            catch (Exception ex)
            {
                throw new AzPSCloudException($"Exception during listener cleanup: {ex.Message}");
            }
        }

        if (_webSocket != null)
        {
            try
            {
                _webSocket.Dispose();
            }
            catch (Exception ex)
            {
                throw new AzPSCloudException($"Exception during WebSocket cleanup: {ex.Message}");
            }
        }

    }

}
