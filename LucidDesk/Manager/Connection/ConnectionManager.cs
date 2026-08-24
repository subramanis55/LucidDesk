using LucidDesk.DS.Classes;
using LucidDesk.DS.Enum;
using LucidDesk.Manager.Database;
using LucidDesk.Manager.Network;
using LucidDesk.Manager.Network.TCP;
using LucidDesk.Manager.Security;
using LucidDesk.Manager.Settings;
using LucidDesK.DS.DataSchema;
using System;
using System.Collections.Generic;

namespace LucidDesk.Manager.Connection
{
    public static class ConnectionManager
    {

        public static event Action<DeskConnectionInformation> ConnectRequestStatusInvoke;
        public static event Action<DeskConnectionInformation> InviteRequestReceivedInvoke;
        public static event Action<DeskConnectionInformation> ConnectRequestReceivedInvoke;

        public static Dictionary<string, ConnectionHandler> ConnectionList = new Dictionary<string, ConnectionHandler>();

        public static Dictionary<string, DeskConnectionInformation> InviteInformation = new Dictionary<string, DeskConnectionInformation>();

        public static List<INetworkConnectionHandler> WaitingList = new List<INetworkConnectionHandler>();

        public static Dictionary<DeskConnectionInformation, INetworkConnectionHandler> WaitingConnectionDict = new Dictionary<DeskConnectionInformation, INetworkConnectionHandler>();

        private static INetworkConnectionManager networkConnectionManager;

        public static bool IsSeverStarted { get => networkConnectionManager?.IsStarted ?? false; }
        public static bool StartConnectionServer()
        {
            if (SettingsManager.Settings.ConnectionMode == "TCP")
            {
                networkConnectionManager = new TCPManager(SettingsManager.Settings.ApplicationDefaultPort);
                networkConnectionManager.NewConnectionInvoke += NetworkConnectionManagerNewConnectionInvoke;
                networkConnectionManager.StartServer();
            }
            return true;
        }

        private static void NetworkConnectionManagerNewConnectionInvoke(INetworkConnectionHandler networkConnectionHandler)
        {
            networkConnectionHandler.ReceivedDataInvoke += HandleReceivedData;
        }

        private static void HandleReceivedData(object sender, Data data)
        {
            try
            {
                if (data == null || WaitingList.Contains((INetworkConnectionHandler)sender))
                    return;
                WaitingList.Add((INetworkConnectionHandler)sender);
                if (data.ReponseAndReqType == ReponseAndReqType.ConnectReq)
                {
                    var connectionInformation = data.GetDeserializeDeskConnectionInformation();
                    connectionInformation.SenderDesk.Freeze();
                    connectionInformation.ReceiverDesk.Freeze();
                    WaitingConnectionDict.Add(connectionInformation, (INetworkConnectionHandler)sender);
                    if (connectionInformation.ConnectionType == ConnectionType.Password && connectionInformation.ReceiverDesk.Password == SecurityManager.Decrypt(DeskProfileManager.UserDesk.Password))
                    {
                        connectionInformation.Status = true;
                        connectionInformation.IsRequestStatusUpdate = true;
                        connectionInformation.Message = "Connection Success";
                        connectionInformation.AddDeskScreensInformations();
                        RequestUpdate(connectionInformation);
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
                        connectionInformation.Message = "Connection Failed";
                        RequestUpdate(connectionInformation);
                    }
                }
                else if (data.ReponseAndReqType == ReponseAndReqType.ReqResponse)
                {
                    DeskConnectionInformation deskConnectionInformation = data.GetDeserializeDeskConnectionInformation();
                    deskConnectionInformation.SenderDesk.Freeze();
                    deskConnectionInformation.ReceiverDesk.Freeze();
                    if (deskConnectionInformation.Status == true && deskConnectionInformation.IsRequestStatusUpdate == true)
                    {
                        //DeskConnectionInformation.Status = true;
                        //DeskConnectionInformation.ScreenInformation = deskConnectionInformation.ScreenInformation;
                    }
                    //ConnectionResponseReceived?.Invoke(this, deskConnectionInformation);
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

        public static ConnectionHandler ConnectReq(DeskConnectionInformation deskConnectionInformation)
        {

            //TODO connection
            return null;
        }

        private static void AddInviteReq(DeskConnectionInformation deskConnectionInformation)
        {
            InviteInformation.Add(deskConnectionInformation.InviteID, deskConnectionInformation);
        }

        private static void RequestUpdate(DeskConnectionInformation deskConnectionInformation)
        {
            if (deskConnectionInformation.Status == true)
            {
                deskConnectionInformation.SenderDesk.RecentLoginTime = DateTime.Now;
                DeskProfileManager.UpdateDeskProfileFromUserdata(deskConnectionInformation.SenderDesk.Clone());
                deskConnectionInformation.ReceiverDesk = DeskProfileManager.UserDesk;
            }
            else
            {
                if (!deskConnectionInformation.IsRequestStatusUpdate)
                {
                    deskConnectionInformation.IsRequestStatusUpdate = true;
                    deskConnectionInformation.Message = "Connection failed";
                }

            }
            Data data = new Data() { DataObject = deskConnectionInformation, ReponseAndReqType = ReponseAndReqType.ReqResponse };
            deskConnectionInformation.NetworkConnectionHandler.WriteObject(data);
        }

    }
}
