using LucidDesK.DS.DataSchema;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Network.TCP
{
    public class TCPConnectionHandler : INetworkConnectionHandler
    {
        private Stream stream;

        public event EventHandler<Data> ReceivedDataInvoke;
        public event EventHandler<bool> ConnectionStatusInvoke;
        public bool IsConnected { get; private set; }
        public int LocalPort { get; private set; }
        public string LocalIpAddress { get; private set; }
        public int RemotePort { get; private set; }
        public string RemoteIpAddress { get; private set; }

        private CancellationTokenSource cancecancellationToken;
        public TcpClient Client
        {
            get;
            private set;
        }

        public TCPConnectionHandler()
        {

        }

        public TCPConnectionHandler(TcpClient client)
        {
            Client = client;
            stream = Client.GetStream();

        }
        private void SaveConnectionInfo()
        {
            IsConnected = true;
            stream = Client.GetStream();
            LocalPort = ((IPEndPoint)Client.Client.LocalEndPoint).Port;
            LocalIpAddress = ((IPEndPoint)Client.Client.LocalEndPoint).Address.ToString();
            RemotePort = ((IPEndPoint)Client.Client.RemoteEndPoint).Port;
            RemoteIpAddress = ((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString();
        }

        public async Task<bool> HandlePunchAsync(int localPort, string remoteIpAddress, int remotePort)
        {
            const int maxAttempts = 15;
            const int attemptTimeoutMs = 600;
            const int delayBetweenAttemptsMs = 250;
            Client?.Close();
            Client?.Dispose();
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                Client = new TcpClient();
                try
                {
                    Client.Client = CreateReusableSocket(localPort);
                    var cts = new CancellationTokenSource(attemptTimeoutMs);
                    await Client.ConnectAsync(IPAddress.Parse(remoteIpAddress), remotePort);
                    SaveConnectionInfo();
                    cancecancellationToken = new CancellationTokenSource();
                    ConnectionStatusInvoke?.Invoke(this, false);
                    HandleReceivedDataAsync(cancecancellationToken);
                    return true;
                }
                catch
                {
                    Client?.Dispose(); // this attempt's socket is dead either way — fresh one next try
                }

                if (attempt < maxAttempts)
                    await Task.Delay(delayBetweenAttemptsMs);
            }
            return false;
        }


        public async Task<bool> ConnectAsync(string remoteIpAddress, int remotePort)
        {
            try
            {
                Client?.Close();
                Client?.Dispose();
                Client = new TcpClient();
                Client.Client = CreateReusableSocket(0);
                if (IPAddress.TryParse(remoteIpAddress, out IPAddress ipAddress))
                    await Client.ConnectAsync(ipAddress, remotePort);
                else
                    return false;
                SaveConnectionInfo();
                cancecancellationToken = new CancellationTokenSource();
                ConnectionStatusInvoke?.Invoke(this, false);
                HandleReceivedDataAsync(cancecancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                // TODO: Log
                Close();
                return false;
            }
        }

        private static Socket CreateReusableSocket(int localPort)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(new IPEndPoint(IPAddress.Any, localPort));
            return socket;
        }

        private async Task HandleReceivedDataAsync(CancellationTokenSource cancecancellationTokenSource)
        {
            try
            {
                stream = Client.GetStream();
                while (Client.Connected)
                {
                    {
                        byte[] lengthBuffer = new byte[4];
                        if (!await ReadExactAsync(stream, lengthBuffer, 4))
                        {
                            Client.Close();
                            return;
                        }
                        int lengthToRead = BitConverter.ToInt32(lengthBuffer, 0);
                        if (lengthToRead <= 0)
                        {
                            Client.Close();
                            return;
                        }
                        const int MaxMessageSize = 10 * 1024 * 1024; // 10 MB — adjust to your protocol's max
                        if (lengthToRead > MaxMessageSize)
                        {
                            Client.Close();
                            return;
                        }
                        byte[] dataBuffer = new byte[lengthToRead];
                        if (!await ReadExactAsync(stream, dataBuffer, lengthToRead))
                        {
                            Client.Close();
                            return;
                        }
                        string json = Encoding.UTF8.GetString(dataBuffer);
                        var data = JsonConvert.DeserializeObject<Data>(json);
                        ReceivedDataInvoke?.Invoke(this, data);
                    }
                }
            }
            catch (Exception ex)
            {
                Close();
                //Need to add log
            }
        }
        private async Task<bool> ReadExactAsync(Stream stream, byte[] buffer, int count)
        {
            int offset = 0;
            while (offset < count)
            {
                int read = await stream.ReadAsync(buffer, offset, count - offset);
                if (read == 0)
                {
                    return false;
                }
                offset += read;
            }
            return true;
        }


        public Task<bool> WriteObjectAsync(object dataObject)
        {
            var json = JsonConvert.SerializeObject(dataObject);
            byte[] data = Encoding.UTF8.GetBytes(json);
            return WriteInStream(data);
        }

        private async Task<bool> WriteInStream(byte[] data)
        {
            try
            {
                // Send data length first (4 bytes)
                byte[] lengthPrefix = BitConverter.GetBytes(data.Length);
                await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
                await stream.WriteAsync(data, 0, data.Length);
                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                // TODO: Add logging

                return false;
            }
            return true;
        }

        public void Dispose()
        {
            ConnectionStatusInvoke?.Invoke(this, false);
            cancecancellationToken.Cancel();
            IsConnected = false;
            Client?.Close();
        }

        public void Close()
        {
            ConnectionStatusInvoke?.Invoke(this, false);
            IsConnected = false;
            Client?.Close();
        }
    }
}
