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
        private AccessType accessType;
        private ConnectionType connectionType;
        public Desk ReceiverDesk { get; set; }
        public Desk SenderDesk { get; set; }
        public bool AudioAccess { get; set; }
        public bool VideoAccess = true;
        public bool ClipboardAccess { get; set; }
        public bool KeyboardAccess{ get; set;}
        public bool MouseAccess { get; set; }
        public bool InviteStatus { get; set;}
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
        public TcpClient TcpClient { get; set; }

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
    }
}
