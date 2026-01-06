using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Enum
{
    public enum ApplicationMode
    {
        Local, Online
    }

    public enum AccessType
    {
        Default, ScreenShareing, FullAccess
    }
    public enum ConnectionType
    {
        Invite, Connect, Password
    }

    public enum ReponseAndReqType
    {
      ConnectReq,ReqResponse,Disconnect, ScreenShareImageData, ScreenShareKeyData
    }

    public enum ControlKeyType
    {
        MouseDown, MouseMove, MouseUp, MouseRightDown, MouseRightUp, Scroll, KeyUp = 11, KeyDown = 12, Clipboard = 13, Audio = 14,ScreenSwitch=15
    }

}
