using LucidDesk.DS.Enum;
using LucidDesk.Manager.Connection;
using LucidDesK.DS.DataSchema;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace LucidDesk.Manager.RemoteControllers
{
    public class RemoteInputSender
    {
        public RemoteInputSender(ConnectionHandler connectionHandler)
        {
            ConnectionHandler = connectionHandler;

            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        private BufferedWaveProvider _bufferedWaveProvider;
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private bool WindowsKey;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public ConnectionHandler ConnectionHandler { get; private set; }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0 && (wParam == (IntPtr)0x0100 || wParam == (IntPtr)0x0104))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                bool isWindowsKey = (vkCode == 0x5B || vkCode == 0x5C);
                bool isAltTab = (vkCode == 0x09 && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)));
                bool isCtrlV = (vkCode == 0x56) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
                bool isClipboardOpen = (vkCode == 0x56) && (Keyboard.IsKeyUp(Key.LWin) || Keyboard.IsKeyUp(Key.RWin));
                if (isWindowsKey) WindowsKey = true;
                else WindowsKey = false;

            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void SendScreenSwitchEvent(int index, LucidDesK.DS.DataSchema.Screens.Screen e)
        {
            Data data = new Data();
            data.ReponseAndReqType = ReponseAndReqType.ScreenShareKeyData;
            data.DataObject = new DeskControlData()
            {
                ControlDataType = ControlKeyType.ScreenSwitch,
                //need to revert
                ControlData = $"{index}:{JsonConvert.SerializeObject(e)}"
            };
            ConnectionHandler.WriteObject(data);
        }

        public void SendMouseScrollEvent(ControlKeyType controlKeyType, Point position, double ScreenImageActualWidth, double ScreenImageActualHeight, double delta = 0)
        {
            // Get the client's screen resolution
            string scrollData = $"{(position.X / ScreenImageActualWidth)},{(position.Y / ScreenImageActualHeight)}:{SystemInformationManager.ScreenWidth},{SystemInformationManager.ScreenHeight}:{delta}";
            Data data = new Data();
            data.ReponseAndReqType = ReponseAndReqType.ScreenShareKeyData;
            data.DataObject = new DeskControlData()
            {
                ControlDataType = controlKeyType,
                ControlData = scrollData
            };
            ConnectionHandler.WriteObject(data);
        }

        public void SendMouseEvent(ControlKeyType controlKeyType, Point position, double ScreenImageActualWidth, double ScreenImageActualHeight, LucidDesK.DS.DataSchema.Screens.Screen selectedScreen = null)
        {
            // Get the client's screen resolution
            //string mouseData = $"{(position.X / ScreenImageActualWidth) * SystemInformationManager.ScreenWidth},{(position.Y / ScreenImageActualHeight) * SystemInformationManager.ScreenHeight}:{SystemInformationManager.ScreenWidth},{SystemInformationManager.ScreenHeight}";
            string mouseData = $"{(position.X / ScreenImageActualWidth)},{(position.Y / ScreenImageActualHeight)}:{SystemInformationManager.ScreenWidth},{SystemInformationManager.ScreenHeight}";
            Data data = new Data();
            data.ReponseAndReqType = ReponseAndReqType.ScreenShareKeyData;
            data.DataObject = new DeskControlData()
            {
                ControlDataType = controlKeyType,
                ControlData = mouseData
            };
            ConnectionHandler.WriteObject(data);
        }

        public void SendMouseRightEvent(ControlKeyType controlKeyType, Point position, double ScreenImageActualWidth, double ScreenImageActualHeight)
        {
            // Get the client's screen resolution
            string keydata = $"{(position.X / ScreenImageActualWidth)},{(position.Y / ScreenImageActualHeight)}:{SystemInformationManager.ScreenWidth},{SystemInformationManager.ScreenHeight}";
            Data data = new Data();
            data.ReponseAndReqType = ReponseAndReqType.ScreenShareKeyData;
            data.DataObject = new DeskControlData()
            {
                ControlDataType = controlKeyType,
                ControlData = keydata
            };
            ConnectionHandler.WriteObject(data);
        }

    

        public void SendKeyEvent(ControlKeyType controlKeyType, Key key)
        {
            // Convert Key to virtual key code
            byte virtualKeyCode = (byte)KeyInterop.VirtualKeyFromKey(key);
            // Get the client's screen resolution
            string keydata = $"{0},{0}:{SystemInformationManager.ScreenWidth}:{SystemInformationManager.ScreenHeight}:{virtualKeyCode}";
            Data data = new Data();
            data.ReponseAndReqType = ReponseAndReqType.ScreenShareKeyData;
            data.DataObject = new DeskControlData()
            {
                ControlDataType = controlKeyType,
                ControlData = keydata
            };
            ConnectionHandler.WriteObject(data);
        }

    }
}
