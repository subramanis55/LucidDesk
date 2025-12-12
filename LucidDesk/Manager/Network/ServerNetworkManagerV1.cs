
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LucidDesk.Manager.Classes;

namespace LucidDesk.Manager.Network
{
   public class ServerNetworkManagerV1
    {
        private TcpListener _listener;
        private readonly ConcurrentDictionary<TcpClient, NetworkStream> _clients = new ConcurrentDictionary<TcpClient, NetworkStream>();
        private CancellationTokenSource _cts;
        public event EventHandler<DeskConnectionInformation> InviteRequestReceivedInvoke;
        public event EventHandler<DeskConnectionInformation> ConnectRequestReceivedInvoke;
        public event EventHandler<DeskConnectionInformation> ConnectRequestStatusInvoke;
        public event EventHandler<DeskConnectionInformation> InviteRequestStatusInvoke;
        public event Action<TcpClient> ClientConnected;
        public event Action<TcpClient> ClientDisconnected;
        public event Action<TcpClient, object> DataReceived;
        private const int CHUNK_SIZE = 8192;
        public bool isStarted { get; private set; }
        private const int PORT = 8000;

        public bool StartServer()
        {
            if (isStarted)
                return true;
            StartAsync(PORT);
            isStarted = true;
            return isStarted;
        }

        private async Task StartAsync(int port)
        {
            if (isStarted) return;
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            isStarted = true;
            _ = AcceptClientsAsync(_cts.Token);
        }

        public void Stop()
        {
            if (!isStarted) return;

            _cts.Cancel();
            foreach (var client in _clients.Keys)
            {
                try { client.Close(); } catch { }
            }

            _listener.Stop();
            isStarted = false;
            Console.WriteLine("Server stopped.");
        }

        private async Task AcceptClientsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync();
                var stream = client.GetStream();
                _clients[client] = stream;
                ClientConnected?.Invoke(client);
                var res = HandleClientAsync(client, stream, token);
            }
        }

        private async Task HandleClientAsync(TcpClient client, NetworkStream stream, CancellationToken token)
        {

            var reponseDataLengthBuffer = new byte[4];
            try
            {
                while (!token.IsCancellationRequested && client.Connected)
                {
                    if (!stream.DataAvailable)
                    {
                        await Task.Delay(10, token);
                        continue;
                    }
                    int readBufferCount = await stream.ReadAsync(reponseDataLengthBuffer, 0, reponseDataLengthBuffer.Length, token);
                    int reponseDataLength = BitConverter.ToInt32(reponseDataLengthBuffer, 0);
                    var reponseDataBuffer = new byte[reponseDataLength];
                    int readDataBufferCount = 0;
                    while (readDataBufferCount< reponseDataLength)
                    {
                        int bytesRead = await stream.ReadAsync(reponseDataBuffer, readDataBufferCount, (CHUNK_SIZE <= (reponseDataLength - readDataBufferCount) ? CHUNK_SIZE : (reponseDataLength - readDataBufferCount)), token);
                        if (bytesRead == 0) continue;
                        readDataBufferCount += bytesRead;
                    }

                    string json = Encoding.UTF8.GetString(reponseDataBuffer, 0, readDataBufferCount);
                    ReponseData data = JsonConvert.DeserializeObject<ReponseData>(json);
                    DataReceived?.Invoke(client, data);
                }
            }
            catch { }
            finally
            {
                if (_clients.TryRemove(client, out _))
                {
                    client.Close();
                    ClientDisconnected?.Invoke(client);
                    Console.WriteLine("Client disconnected.");
                }
            }
        }

        private async Task SendWithLengthPrefixAsync(NetworkStream stream, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            // prefix the length as 4 bytes
            byte[] lengthPrefix = BitConverter.GetBytes(jsonBytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthPrefix); // ensure network byte order

            await stream.WriteAsync(lengthPrefix, 0, 4);
            await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
        }
        public async Task SendAsync(TcpClient client, object data)
        {
            if (!_clients.ContainsKey(client)) return;
            var stream = _clients[client];
            string json = JsonConvert.SerializeObject(data);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public async Task DisconnectClientAsync(TcpClient client)
        {
            if (!_clients.ContainsKey(client)) return;
            await SendAsync(client, new { Command = "Disconnect" });
            client.Close();
            _clients.TryRemove(client, out _);
            ClientDisconnected?.Invoke(client);
        }
    }
}
