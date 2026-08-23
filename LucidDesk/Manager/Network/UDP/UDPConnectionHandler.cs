using LucidDesK.DS.DataSchema;
using System;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Network.UDP
{
    internal class UDPConnectionHandler : INetworkConnectionHandler
    {
        public event Action<Data> ReceivedDataInvoke;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Start()
        {
            throw new NotImplementedException();
        }

        public void WriteObject(Data dataObject)
        {
            throw new NotImplementedException();
        }

        public void WriteObject(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void WriteObject(string json)
        {
            throw new NotImplementedException();
        }
    }
}
