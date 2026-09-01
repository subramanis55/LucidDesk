using LucidDesK.DS.DataSchema;
using System;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Network
{
    public interface INetworkConnectionHandler
    {
        event EventHandler<Data> ReceivedDataInvoke;

        bool IsConnected { get; }
        int LocalPort { get; }
        string LocalIpAddress { get; }

        Task<bool> ConnectAsync(string remoteAddress, int remotePort);

        void Close();


        Task<bool> WriteObjectAsync(object dataObject);


    }
}
