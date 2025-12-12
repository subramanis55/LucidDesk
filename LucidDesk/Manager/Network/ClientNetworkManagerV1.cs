using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Classes.DataSchema;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LucidDesk.Manager.Network
{
    internal class ClientNetworkManagerV1
    {

        public event Action<ImageData> ScreenShareUpdateInvoke;
        public event Action ConnectedToServerInvoke;
        public event Action DisConnectedToServerInvoke;
        public event Action ConnectionEstablishFailInvoke;

        private string ClientIpaddress;


        public DeskConnectionInformation deskConnectionInformation;
        public DeskConnectionInformation DeskConnectionInformation
        {
            set
            {
                deskConnectionInformation = value;
                ClientIpaddress = deskConnectionInformation.ReceiverDesk.IPAddress;
            }
            get
            {
                return deskConnectionInformation;
            }
        }

        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;

        public bool IsConnected => _client?.Connected ?? false;

        public void ConnectToDesk(DeskConnectionInformation deskConnectionInformation)
        {
            ConnectAsync(deskConnectionInformation.ReceiverDesk.IPAddress, 8000);
        }

        private async Task ConnectAsync(string ip, int port)
        {
            if (IsConnected) return;

            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);
            _stream = _client.GetStream();
            ConnectedToServerInvoke?.Invoke();
            _cts = new CancellationTokenSource();
            readAsync(_stream);
        }
        public async void readAsync(NetworkStream stream)
        {
            while (true)
            {
                ReadWithLengthPrefixAsync(stream, _cts.Token);
            }
        }

        public async Task DisconnectAsync()
        {
            if (!IsConnected) return;
            await SendAsync(new ReponseData() { ReponseType = Enum.ReponseType.DisConnect });
            _cts.Cancel();
            _stream?.Close();
            _client?.Close();
            DisConnectedToServerInvoke?.Invoke();
        }

        private async Task<object> ReadWithLengthPrefixAsync(NetworkStream stream, CancellationToken token)
        {
            byte[] lengthBuffer = new byte[4];

            // Read 4 bytes for message length
            int read = await ReadExactAsync(stream, lengthBuffer, 4, token);
            if (read == 0) return null;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBuffer);

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            if (messageLength <= 0) return null;

            // Now read the full JSON message
            byte[] dataBuffer = new byte[messageLength];
            read = await ReadExactAsync(stream, dataBuffer, messageLength, token);
            if (read == 0) return null;

            string json = Encoding.UTF8.GetString(dataBuffer);
            return JsonConvert.DeserializeObject<ReponseData>(json);
        }

        private async Task<int> ReadExactAsync(NetworkStream stream, byte[] buffer, int size, CancellationToken token)
        {
            int totalRead = 0;
            int toRead = Math.Min(64 * 1024, size - totalRead);
            while (totalRead < size)
            {
                int bytesRead = await stream.ReadAsync(buffer, totalRead, toRead, token);
                if (bytesRead == 0)
                    return 0; // disconnected
                totalRead += bytesRead;
            }
            return totalRead;
        }

        public async Task SendAsync(object data)
        {
            if (!IsConnected) return;

            string json = JsonConvert.SerializeObject(data);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            await _stream.WriteAsync(bytes, 0, bytes.Length);
        }

    }

}

