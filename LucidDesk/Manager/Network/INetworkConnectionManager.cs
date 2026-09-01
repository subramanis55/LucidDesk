using System;

namespace LucidDesk.Manager.Network
{
    public interface INetworkConnectionManager : IDisposable
    {
        event Action<INetworkConnectionHandler> NewConnectionInvoke;
        int PORT { get; }
        bool IsStarted { get; }

        bool StartServer();

        void StopServer();
    }
}
