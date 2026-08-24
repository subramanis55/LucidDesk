using LucidDesK.DS.DataSchema;
using System;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Network
{
    public interface INetworkConnectionHandler
    {
        event EventHandler<Data> ReceivedDataInvoke;

        bool IsConnected { get; }
        Task<bool> Start();

        void Close();

        void WriteObject(Data dataObject);

        void WriteObject(byte[] data);

        void WriteObject(string json);

    }
}
