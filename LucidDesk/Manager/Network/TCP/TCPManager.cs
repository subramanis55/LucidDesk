#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace LucidDesk.Manager.Network.TCP
{
    public class TCPManager : IDisposable, INetworkConnectionManager
    {
        private int port = 0;
        private TcpListener tcpListener;
        private CancellationTokenSource cancellationTokenSource;
        private bool isStarted = false;
        private Timer remainTimer;
        private Dictionary<TcpClient, TCPConnectionHandler> connectedClients = new Dictionary<TcpClient, TCPConnectionHandler>();

        public event Action<INetworkConnectionHandler> NewConnectionInvoke;

        public int PORT
        {
            get
            {
                return port;
            }
            private set
            {
                port = value;
            }
        }

        public bool IsStarted
        {
            get { return isStarted; }

            private set
            {
                isStarted = value;
            }
        }

        public TCPManager(int port)
        {
            PORT = port;

        }
        public bool StartServer()
        {
            if (IsStarted) return true;
            cancellationTokenSource = new CancellationTokenSource();
            tcpListener = new TcpListener(IPAddress.Any, PORT);
            tcpListener.Start();
            Task.Run(() => AcceptClients(cancellationTokenSource.Token));
            // Check every 30 seconds
            remainTimer = new System.Threading.Timer(RemainingClientsCheck, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            isStarted = true;
            return isStarted;
        }

        private async Task AcceptClients(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await tcpListener.AcceptTcpClientAsync();
                    HandleNewClient(client);
                }
            }
            catch (Exception ex)
            {
                StopServer();
            }
        }

        private async void HandleNewClient(TcpClient client)
        {
            if (connectedClients.ContainsKey(client))
                return;

            try
            {
                TCPConnectionHandler tCPConnectionHandler = new TCPConnectionHandler(client);
                connectedClients.Add(client, tCPConnectionHandler);
                NewConnectionInvoke?.Invoke(tCPConnectionHandler);
            }
            catch (IOException)
            {
                //Need to add log
            }
        }

        private void RemainingClientsCheck(object state)
        {
            foreach (var clientEntry in connectedClients)
            {
                try
                {
                    var client = clientEntry.Key;
                    if ((client.Connected && client.GetStream().CanWrite))
                    {
                        DisconnectClient(client);
                    }
                }
                catch (Exception ex)
                {
                    DisconnectClient(clientEntry.Key);
                }
            }
        }

        public void StopServer()
        {
            if (!IsStarted) return;
            cancellationTokenSource.Cancel();
            tcpListener.Stop();
            remainTimer.Dispose();
            foreach (var client in connectedClients.Values)
            {
                client.Close();
            }
            connectedClients.Clear();
            isStarted = false;
        }

        private void DisconnectClient(TcpClient client)
        {
            if (connectedClients.ContainsKey(client))
            {
                connectedClients[client].Close();
                connectedClients.Remove(client);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
