using LucidDesK.DS.DataSchema;
using System;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Network.UDP
{
    internal class UDPConnectionHandler : INetworkConnectionHandler
    {
        public bool IsConnected => throw new NotImplementedException();
        public int LocalPort { get; private set; }
        public string LocalIpAddress { get; private set; }

        public event Action<Data> ReceivedDataInvoke;

        event EventHandler<Data> INetworkConnectionHandler.ReceivedDataInvoke
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
        public Task<bool> ConnectAsync(string remoteAddress, int remotePort)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteObjectAsync(object dataObject)
        {
            throw new NotImplementedException();
        }
    }
}
