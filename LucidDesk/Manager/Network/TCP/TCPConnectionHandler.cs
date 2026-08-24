using LucidDesK.DS.DataSchema;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Network.TCP
{
    public class TCPConnectionHandler : INetworkConnectionHandler
    {
        private Stream stream;

        public event EventHandler<Data> ReceivedDataInvoke;

        public bool IsConnected { get; private set; }
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

        public async Task<bool> Start()
        {
            if (Client == null || !Client.Connected || !Client.GetStream().CanRead)
                return false;
            HandleReceivedDataAsync();
            return true;
        }

        public async Task<bool> ConnectAsync(string localIPAddress, int localPort, string remoteIpAddress, int remotePort)
        {
            try
            {
                var localEndPoint = new IPEndPoint(IPAddress.Parse(localIPAddress), localPort);
                Client = new TcpClient(localEndPoint);
                await Client.ConnectAsync(IPAddress.Parse(remoteIpAddress), remotePort);
                stream = Client.GetStream();
                IsConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                // TODO: Log
                Client?.Dispose();
                Client = null;
                return false;
            }
        }

        private async Task HandleReceivedDataAsync()
        {
            try
            {
                stream = Client.GetStream();
                while (Client.Connected)
                {
                    {
                        byte[] lengthBuffer = new byte[4];
                        await stream.ReadAsync(lengthBuffer, 0, 4);
                        int lengthToRead = BitConverter.ToInt32(lengthBuffer, 0);
                        if (lengthToRead == 0)
                            Client.Close();
                        byte[] dataBuffer = new byte[lengthToRead];
                        int bytesRead = 0;
                        while (bytesRead < lengthToRead)
                        {
                            bytesRead += await stream.ReadAsync(dataBuffer, bytesRead, lengthToRead - bytesRead);
                        }
                        string json = Encoding.UTF8.GetString(dataBuffer);
                        var data = JsonConvert.DeserializeObject<Data>(json);
                        ReceivedDataInvoke?.Invoke(this, data);
                    }
                }
            }
            catch (Exception ex)
            {
                //Need to add log
            }
        }

        public void WriteObject(Data dataObject)
        {
            var json = JsonConvert.SerializeObject(dataObject);
            byte[] data = Encoding.UTF8.GetBytes(json);
            WriteInStream(data);
        }

        public void WriteObject(byte[] data)
        {
            WriteInStream(data);
        }

        public void WriteObject(string json)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            WriteInStream(data);
        }

        private async Task WriteInStream(byte[] data)
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
                // Logger.LogError(ex, "Error writing data to stream");
            }
        }

        public void Close()
        {
            IsConnected = false;
            Client.Close();
        }
    }
}
