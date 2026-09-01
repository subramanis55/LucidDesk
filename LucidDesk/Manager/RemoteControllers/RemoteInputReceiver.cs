using LucidDesk.DS.Enum;
using LucidDesk.Manager.Connection;
using LucidDesK.DS.DataSchema;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LucidDesk.Manager.RemoteControllers
{
    [StructLayout(LayoutKind.Sequential)]
    struct INPUT
    {
        public uint type;
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MOUSEINPUT
    {
        public uint dx;
        public uint dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    public class RemoteInputReceiver
    {
        public RemoteInputReceiver()
        {

        }

        const uint INPUT_MOUSE = 0;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;
        private const uint MOUSEEVENTF_HWHEEL = 0x01000;
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_VIRTUALDESK = 0x4000;
        public bool isMouseAcess, isKeyboardAcess, isAudioAcess, isClipboardAcess;
        private byte VK_TAB = 0x09, VK_MENU = 0x12;


        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        public ConnectionHandler ConnectionHandler { get; private set; }

        public void HandleRemoteEvent(DeskControlData data) //"MouseDown:223.777777777778,232:1920,108j0" //"KeyDown:0,0:1920:1080:68"
        {
            string[] parts = data.ControlData.Split(':');
            double delta = 0;
            byte keyCode = 0;
            double clientScreenWidth = 0, clientScreenHeight = 0, x = 0, y = 0;
            if ((int)data.ControlDataType <= 10)
            {
                string[] coords = parts[0].Split(',');
                x = double.Parse(coords[0]);
                y = double.Parse(coords[1]);
                string[] screenSize = parts[1].Split(',');
                clientScreenWidth = double.Parse(screenSize[0]);
                clientScreenHeight = double.Parse(screenSize[1]);
                //double scaleX = SystemInformationManager.ScreenWidth / clientScreenWidth;
                //double scaleY = SystemInformationManager.ScreenHeight / clientScreenHeight;
                int screenX, screenY = 0;
                if (ScreenShareManager.SelectedScreen != null)
                {
                    System.Drawing.Rectangle virtualBounds = SystemInformation.VirtualScreen;
                    System.Drawing.Rectangle bounds = ScreenShareManager.SelectedScreen.Bounds;
                    screenX = (int)(bounds.X + (double)(x * ScreenShareManager.SelectedScreen.Bounds.Width));
                    screenY = (int)(bounds.Y + (double)(y * ScreenShareManager.SelectedScreen.Bounds.Height));
                }
                else
                {
                    System.Drawing.Rectangle virtualBounds = SystemInformation.VirtualScreen;
                    screenX = (int)(virtualBounds.X + (double)(x * (double)virtualBounds.Width));
                    screenY = (int)(virtualBounds.Y + (double)(y * (double)virtualBounds.Height));
                }
                //  System.Windows.Forms.Cursor.Position = new System.Drawing.Point(screenX, screenY);
                switch (data.ControlDataType)
                {
                    case ControlKeyType.MouseMove:
                        ExecuteMouseMove(screenX, screenY);
                        break;
                    case ControlKeyType.MouseDown:
                        ExecuteMouseMove(screenX, screenY);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);
                        //ExecuteMouseButton(MOUSEEVENTF_LEFTDOWN);
                        break;
                    case ControlKeyType.MouseUp:
                        ExecuteMouseMove(screenX, screenY);
                        mouse_event(MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, UIntPtr.Zero);
                        //ExecuteMouseButton(MOUSEEVENTF_LEFTUP);
                        break;
                    case ControlKeyType.MouseRightDown:
                        ExecuteMouseButton(MOUSEEVENTF_RIGHTDOWN);
                        break;
                    case ControlKeyType.MouseRightUp:
                        ExecuteMouseButton(MOUSEEVENTF_RIGHTUP);
                        break;
                    case ControlKeyType.Scroll:
                        delta = double.Parse(parts[2]);
                        ExecuteMouseWheel((int)delta);
                        break;
                }
            }
            else
            {
                switch (data.ControlDataType)
                {
                    case ControlKeyType.KeyDown:
                    case ControlKeyType.KeyUp:
                        keyCode = byte.Parse(parts[3]);
                        string[] coords = parts[0].Split(',');
                        x = double.Parse(coords[0]);
                        y = double.Parse(coords[1]);
                        keybd_event(keyCode, 0, data.ControlDataType == ControlKeyType.KeyDown ? KEYEVENTF_KEYDOWN : KEYEVENTF_KEYUP, UIntPtr.Zero);
                        break;
                    case ControlKeyType.ScreenSwitch:
                        string[] screeninfo = parts[0].Split(',');
                        if (int.TryParse(screeninfo[0], out int screenIndex))
                            ScreenShareManager.MoniterScreenSwitch(screenIndex);
                        break;
                }
            }
            //else if (eventType == "ClipboardText")
            //{
            //    string clipboardText = parts[0];
            //    Clipboard.SetText(clipboardText);
            //    return;
            //}
            //else if (eventType == "ClipBoardOpen")
            //{
            //    keybd_event(0x5B, 0, 0, IntPtr.Zero);
            //    keybd_event(0x56, 0, 0, IntPtr.Zero);
            //    keybd_event(0x56, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            //    keybd_event(0x5B, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            //}
            //switch (eventType)
            //{
            //    case "AltTab":
            //        keybd_event(VK_MENU, 0, 0, IntPtr.Zero);
            //        keybd_event(VK_TAB, 0, 0, IntPtr.Zero);
            //        break;
            //    case "WindowKey":
            //        keybd_event(0x5B, 0, 0, IntPtr.Zero);
            //        keybd_event(0x5B, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            //        break;
            //}
        }

        void ExecuteMouseMove(int x, int y)
        {
            INPUT input = new INPUT
            {
                type = 0, // INPUT_MOUSE
                mi = new MOUSEINPUT
                {
                    dx = ToAbsoluteX(x),
                    dy = ToAbsoluteY(y),
                    dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_VIRTUALDESK
                }
            };
            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        static void ExecuteMouseWheel(int delta)
        {
            INPUT input = new INPUT
            {
                type = INPUT_MOUSE,
                mi = new MOUSEINPUT
                {
                    dwFlags = MOUSEEVENTF_WHEEL,
                    mouseData = (uint)delta
                }
            };

            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }
        static void ExecuteMouseButton(uint flag)
        {
            INPUT input = new INPUT
            {
                type = 0,
                mi = new MOUSEINPUT
                {
                    dwFlags = flag
                }
            };
            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        static uint ToAbsoluteX(int x)
        {
            return (uint)(x * 65535 / (SystemInformation.VirtualScreen.Width - 1));
        }

        static uint ToAbsoluteY(int y)
        {
            return (uint)(y * 65535 / (SystemInformation.VirtualScreen.Height - 1));
        }
    }
}




