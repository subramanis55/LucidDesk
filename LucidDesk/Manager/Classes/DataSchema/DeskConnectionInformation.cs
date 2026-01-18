using LucidDesk.Manager.Classes.DataSchema.Screens;
using LucidDesk.Manager.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Classes
{
    public class DeskConnectionInformation
    {
        public DeskConnectionInformation()
        {

        }
        public DeskConnectionInformation(Desk receiverDesk, Desk senderDesk, AccessType accessType, ConnectionType connectionType)
        {
            ReceiverDesk = receiverDesk;
            SenderDesk = senderDesk;
            ConnectionType = connectionType;
            AccessType = accessType;
            if (AccessType == AccessType.FullAccess)
                KeyboardAccess = MouseAccess = AudioAccess = ClipboardAccess = true;
            else if (AccessType == AccessType.Default)
                KeyboardAccess = MouseAccess = ClipboardAccess = true;

        }
        public DeskConnectionInformation(Desk receiverDesk, Desk senderDesk, bool keyboardAccess, bool mouseAccess, bool audioAccess, bool clipboardAccess, ConnectionType connectionType)
        {
            ReceiverDesk = receiverDesk;
            SenderDesk = senderDesk;
            KeyboardAccess = keyboardAccess;
            MouseAccess = mouseAccess;
            AudioAccess = audioAccess;
            ClipboardAccess = clipboardAccess;
            ConnectionType = connectionType;
        }

        private AccessType accessType;

        private ConnectionType connectionType;
        public Desk ReceiverDesk { get; set; }
        public Desk SenderDesk { get; set; }
        public bool AudioAccess { get; set; }
        public bool ClipboardAccess { get; set; }
        public bool KeyboardAccess { get; set; }
        public bool MouseAccess { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public bool IsRequestStatusUpdate { get; set; }
        public string InviteID{  get; set; }
        public ScreenInformation ScreenInformation { set; get; } = new ScreenInformation();

        [NonSerialized]
        public TcpClient TcpClient;
        public AccessType AccessType
        {
            set
            {
                accessType = value;

            }
            get
            {
                return accessType;

            }
        }

        public ConnectionType ConnectionType
        {
            get
            {
                return connectionType;
            }

            set
            {
                connectionType = value;
            }
        }

        public void AddDeskScreensInformations()
        {
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                ScreenInformation.AddScreen(new LucidDesk.Manager.Classes.DataSchema.Screens.Screen() { Bounds = screen.Bounds, IsPrimaryScreen = screen.Primary, DeviceName = screen.DeviceName });
            }
        }
    }
}
