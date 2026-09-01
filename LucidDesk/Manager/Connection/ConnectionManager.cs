using LucidDesk.DS.Classes;
using LucidDesk.DS.Enum;
using LucidDesk.DS.Models;
using LucidDesk.DS.Response;
using LucidDesk.Manager.Database;
using LucidDesk.Manager.Network;
using LucidDesk.Manager.Network.TCP;
using LucidDesk.Manager.RemoteControllers;
using LucidDesk.Manager.Security;
using LucidDesk.Manager.Services;
using LucidDesk.Manager.Settings;
using LucidDesK.DS.DataSchema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Connection
{
    public static class ConnectionManager
    {
        public static event Action<bool> ServerConnectionStatusInvoke;
        public static event Action<DeskConnectionInformation> ConnectRequestStatusInvoke;
        public static event Action<DeskConnectionInformation> InviteRequestReceivedInvoke;
        public static event Action<DeskConnectionInformation> ConnectRequestReceivedInvoke;
        public static event Action<DeskConnectionInformation> ConnectionResponseReceived;

        private static INetworkConnectionHandler serverConnection;

        public static Dictionary<string, ConnectionHandler> ConnectionList = new Dictionary<string, ConnectionHandler>();

        public static List<INetworkConnectionHandler> WaitingList = new List<INetworkConnectionHandler>();

        public static Dictionary<DeskConnectionInformation, INetworkConnectionHandler> WaitingConnectionDict = new Dictionary<DeskConnectionInformation, INetworkConnectionHandler>();

        private static INetworkConnectionManager localNetworkConnectionManager;

        public static bool IsLocalSeverStarted { get => localNetworkConnectionManager?.IsStarted ?? false; }
        public static bool IsSeverConnected { get; private set; }
        public static async Task<bool> StartConnectionServer()
        {
            bool res = false;
            if (SettingsManager.Settings.ConnectionMode == "TCP")
            {
                localNetworkConnectionManager?.StopServer();
                localNetworkConnectionManager = new TCPManager(0);
                localNetworkConnectionManager.NewConnectionInvoke += NetworkConnectionManagerNewConnectionInvoke;
                localNetworkConnectionManager.StartServer();
                return await ConnectServer();
            }
            return false;
        }
        private static void NetworkConnectionManagerNewConnectionInvoke(INetworkConnectionHandler networkConnectionHandler)
        {
            networkConnectionHandler.ReceivedDataInvoke += HandleReceivedData;
        }

        public static async Task<bool> ConnectServer()
        {
            serverConnection?.Close();
            serverConnection = new TCPConnectionHandler();
            var res = await serverConnection.ConnectAsync(SettingsManager.Settings.ServerAddress, SettingsManager.Settings.ServerPort);
            if (res)
            {
                serverConnection.WriteObjectAsync(new PeerInfo() { PrivateIp = serverConnection.LocalIpAddress, PrivatePort = serverConnection.LocalPort, UserNumber = DeskProfileManager.UserDesk.Id.ToString() });
                serverConnection.ReceivedDataInvoke += HandleReceivedData;
            }
            ServerConnectionStatusInvoke?.Invoke(res);
            IsSeverConnected = res;
            return res;
        }

        public static async Task<BooleanMsg<ConnectionHandler>> ConnectResponseReceived(DeskConnectionInformation deskConnectionInformation)
        {
            var networkHandler = new TCPConnectionHandler();
            var peerInfo = await UserService.GetUserPeerInfoAsync(deskConnectionInformation.ReceiverDesk.Id);
            if (peerInfo == null)
                return "Connection Desk Not Found";
            var res = await networkHandler.HandlePunchAsync(serverConnection.LocalPort, peerInfo.PrivateIp, peerInfo.PublicPort);
            if (!res)
                return "Connection Failed";
            ConnectionHandler connectionHandler = new ConnectionHandler(deskConnectionInformation, new RemoteInputReceiver(), networkHandler);
            ConnectionList.Add(deskConnectionInformation.ReceiverDesk.Id, connectionHandler);
            ConnectionResponseReceived?.Invoke(deskConnectionInformation);
            return connectionHandler;
        }

        public static async Task<BooleanMsg> ConnectToDeskRequestSent(Desk remoteDesk)
        {
            DeskConnectionInformation deskConnectionInformation = new DeskConnectionInformation(DeskProfileManager.UserDesk, remoteDesk, AccessType.Default, ConnectionType.Connect);
            Data connectionReq = new Data(ReponseAndReqType.ConnectReq, deskConnectionInformation);
            var res = await serverConnection.WriteObjectAsync(connectionReq);
            return res;
        }
        public static async Task<BooleanMsg> ConnectToDeskRequestSent(DeskConnectionInformation deskConnectionInformation)
        {
            Data connectionReq = new Data(ReponseAndReqType.ConnectReq, deskConnectionInformation);
            var res = await serverConnection.WriteObjectAsync(connectionReq);
            return res;
        }

        private static async void HandleReceivedData(object sender, Data data)
        {
            try
            {
                if (data == null || WaitingList.Contains((INetworkConnectionHandler)sender))
                    return;

                if (data.ReponseAndReqType == ReponseAndReqType.ConnectReq)
                {
                    var connectionInformation = data.GetDeserializeDeskConnectionInformation();
                    WaitingConnectionDict.Add(connectionInformation, (INetworkConnectionHandler)sender);
                    //if (connectionInformation.ConnectionType == ConnectionType.Password && BCrypt.Net.BCrypt.Verify(connectionInformation.Password, DeskProfileManager.UserDesk.Password))
                    //{
                    //    connectionInformation.Status = true;
                    //    connectionInformation.IsRequestStatusUpdate = true;
                    //    connectionInformation.AddDeskScreensInformations();
                    //    ConnectionRequestUpdate(connectionInformation);
                    //}
                    if (connectionInformation.ConnectionType == ConnectionType.OneTimePassword && connectionInformation.Password == SecurityManager.Decrypt(""))
                    {
                        connectionInformation.Status = true;
                        connectionInformation.IsRequestStatusUpdate = true;
                        connectionInformation.AddDeskScreensInformations();
                        ConnectionRequestUpdate(connectionInformation);
                    }
                    else if (connectionInformation.ConnectionType == ConnectionType.Connect)
                    {
                        ConnectRequestReceivedInvoke?.Invoke(connectionInformation);
                    }
                    else if (connectionInformation.ConnectionType == ConnectionType.Invite)
                    {
                        InviteRequestReceivedInvoke?.Invoke(connectionInformation);
                    }
                    else
                    {
                        connectionInformation.Status = false;
                        connectionInformation.IsRequestStatusUpdate = true;
                        connectionInformation.Message = "Remote access permission failed";
                        ConnectionRequestUpdate(connectionInformation);
                    }
                }
                else if (data.ReponseAndReqType == ReponseAndReqType.ReqResponse)
                {
                    var connectionRes = false;
                    DeskConnectionInformation deskConnectionInformation = data.GetDeserializeDeskConnectionInformation();
                    if (deskConnectionInformation.Status == true && deskConnectionInformation.IsRequestStatusUpdate == true)
                    {
                        connectionRes = await ConnectResponseReceived(deskConnectionInformation);
                        if (connectionRes == false)
                        {
                            deskConnectionInformation.Status = false;
                            deskConnectionInformation.Message = "Connection Failed";
                        }
                    }
                    ConnectionResponseReceived?.Invoke(deskConnectionInformation);
                }
                //else if (data.ReponseAndReqType == ReponseAndReqType.ReqResponse)
                //{
                //    var connectionInformation = data.GetDeserializeDeskConnectionInformation();
                //    connectionInformation.SenderDesk?.Freeze();
                //    connectionInformation.ReceiverDesk?.Freeze();
                //    if (connectionInformation.ConnectionType == ConnectionType.Invite && connectionInformation.Status == true && connectionInformation.IsRequestStatusUpdate == true)
                //    {
                //        connectionInformation.AddDeskScreensInformations();
                //        RequestUpdate(connectionInformation);
                //    }
                //}
            }
            catch (Exception ex)
            {
                ((INetworkConnectionHandler)(sender)).Close();
                WaitingList.Remove(((INetworkConnectionHandler)(sender)));
                //ToDo Log
            }

        }

        private static void InviteRequestSent(DeskConnectionInformation deskConnectionInformation)
        {
            //    InviteInformation.Add(deskConnectionInformation.InviteID, deskConnectionInformation);
        }

        private static void ConnectionRequestUpdate(DeskConnectionInformation deskConnectionInformation)
        {
            //if (deskConnectionInformation.Status == true)
            //{
            //    DeskProfileManager.UpdateDeskProfileFromUserdata(deskConnectionInformation.SenderDesk.Clone());
            //    deskConnectionInformation.ReceiverDesk = DeskProfileManager.UserDesk;
            //}
            //}
            //if (deskConnectionInformation.IsRequestStatusUpdate&&deskConnectionInformation.Status)
            //{
            //    deskConnectionInformation.AddDeskScreensInformations();
            //}
            if (WaitingConnectionDict.ContainsKey(deskConnectionInformation))
            {
                WaitingConnectionDict.Remove(deskConnectionInformation);
            }
            Data data = new Data(ReponseAndReqType.ReqResponse, deskConnectionInformation);
            serverConnection.WriteObjectAsync(data);
        }

        public static void Restart()
        {
            StartConnectionServer();
        }
    }
}
