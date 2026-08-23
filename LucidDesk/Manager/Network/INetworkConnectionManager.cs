using System;

namespace LucidDesk.Manager.Network
{
    public interface INetworkConnectionManager
    {
        event Action<INetworkConnectionHandler> NewConnectionInvoke;
        int PORT { get; }
        bool IsStarted { get; }

        void StartServer();

        void StopServer();
    }
}
