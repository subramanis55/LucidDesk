using LucidDesk.DS.Classes;
using LucidDesk.DS.Enum;
using LucidDesk.Manager.Network;
using LucidDesk.Manager.RemoteControllers;
using LucidDesK.DS.DataSchema;
using System;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Connection
{
    public class ConnectionHandler
    {
        public event Action DisConnectedToSeverInvoke;
        public event EventHandler<DeskConnectionInformation> ConnectionResponseReceived;
        public event Action<DeskImageData> ReceivedDeskImageDataInvoke;

        public DeskConnectionInformation deskConnectionInformation;

        public bool IsRemoteServer;

        public bool IsConnected => networkConnectionHandler?.IsConnected ?? false;

        private INetworkConnectionHandler networkConnectionHandler;

        public RemoteInputSender RemoteInputSender { get; private set; }

        public RemoteInputReceiver RemoteInputReceiver { get; private set; }
        public DeskConnectionInformation DeskConnectionInformation
        {
            set
            {
                deskConnectionInformation = value;

            }
            get
            {
                return deskConnectionInformation;
            }
        }

        public ConnectionHandler(DeskConnectionInformation deskConnectionInformation, RemoteInputReceiver remoteInputReceiver, INetworkConnectionHandler connectionHandler)
        {
            IsRemoteServer = true;
            RemoteInputReceiver = remoteInputReceiver;
            networkConnectionHandler = connectionHandler;
            networkConnectionHandler.ReceivedDataInvoke += NetworkConnectionHandlerReceivedDataInvoke;
        }
        public ConnectionHandler(DeskConnectionInformation deskConnectionInformation, RemoteInputSender remoteInputSender, RemoteInputReceiver remoteInputReceiver, INetworkConnectionHandler connectionHandler)
        {
            RemoteInputReceiver = remoteInputReceiver;
            networkConnectionHandler = connectionHandler;
            networkConnectionHandler.ReceivedDataInvoke += NetworkConnectionHandlerReceivedDataInvoke;
        }

        private void NetworkConnectionHandlerReceivedDataInvoke(object sender, Data data)
        {
            if (data.ReponseAndReqType == ReponseAndReqType.ScreenShareImageData)
                ReceivedDeskImageDataInvoke?.Invoke(data.GetDeserializeDeskImageData());
            else if (data.ReponseAndReqType == ReponseAndReqType.ScreenShareKeyData)
                RemoteInputReceiver.HandleRemoteEvent(data.GetDeserializeDeskControlData());

        }

        public void Close()
        {
            networkConnectionHandler?.Close();
        }

        public Task<bool> Start()
        {
            return networkConnectionHandler?.Start();
        }

        public void WriteObject(Data dataObject)
        {
            networkConnectionHandler?.WriteObject(dataObject);
        }

    }
}
