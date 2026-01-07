using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Classes.DataSchema;
using LucidDesk.Manager.Classes.DataSchema.Screens;
using LucidDesk.Manager.Enum;
using LucidDesk.Settings;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;



namespace LucidDesk.Manager
{

    public partial class ClientNetworkManager
    {
        public DeskConnectionInformation deskConnectionInformation;
        public DeskConnectionInformation DeskConnectionInformation
        {
            set
            {
                deskConnectionInformation = value;
                if (SettingsManager.Settings.ApplicationMode == ApplicationMode.Local)
                {
                    ClientIpaddress = deskConnectionInformation.ReceiverDesk.HostName != null ? SystemInformationManager.GetPcIPAddress(deskConnectionInformation.ReceiverDesk.HostName) : deskConnectionInformation.ReceiverDesk.IPAddress;
                }

            }
            get
            {
                return deskConnectionInformation;
            }
        }

        public const int PORT = 12345;
        public event EventHandler<DeskImageData> ScreenShareUpdateInvoke;
        public event EventHandler<string> DisConnectedToSeverInvoke;
        public event EventHandler ConnectionEstabishFailInvoke;
        public event EventHandler<DeskConnectionInformation> ConnectionResponseReceived;
        public ClientNetworkManager()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }
        private TcpClient AudioTcpClient;
        private WaveOutEvent _waveOut;
        private BufferedWaveProvider _bufferedWaveProvider;
        private CancellationTokenSource _cancellationTokenSource;
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        public bool isConnected;
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        public string ClientIpaddress;

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

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private bool WindowsKey;
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

                //if (isWindowsKey && !isClipboardOpen && !isCtrlV)
                //{
                //    // Send Windows key event to the server
                //    //SendKeyEvent((Key)vkCode, "KeyDown");
                //    SendKeyEvent((Key)vkCode, "WindowKey");
                //    return (IntPtr)1;
                //}
                //else if (isClipboardOpen && !isCtrlV && WindowsKey)
                //{
                //    SendKeyEvent((Key)vkCode, "ClipBoardOpen");
                //    return (IntPtr)(1);
                //}
                //else if (isAltTab)
                //{
                //    // Handle Alt+Tab locally (don't send to server)
                //    SendKeyEvent((Key)vkCode, "AltTab");
                //    return (IntPtr)1; // Suppress the key press locally
                //}

            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        }

        private void ConnectButton_ClickAsync(DeskConnectionInformation deskConnectionInformation)
        {
            if (!isConnected)
            {

                //Task.Run(() => ConnectToServer());
                //_cancellationTokenSource = new CancellationTokenSource();
                //AudioTcpClient = new TcpClient();
                //try
                //{
                //    AudioTcpClient.Connect(ClientIpaddress, PORT);

                //    _waveOut = new WaveOutEvent();
                //    _bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2));
                //    _waveOut.Init(_bufferedWaveProvider);
                //    _waveOut.Play();

                //    Thread receiveThread = new Thread(ReceiveAudio);
                //    receiveThread.Start();
                //}
                //catch
                //{
                //    return;
                //}
            }
        }

        private void ReceiveAudio()
        {
            //using (var networkStream = AudioTcpClient.GetStream())
            //{
            //    var buffer = new byte[1024];
            //    int bytesRead;

            //    try
            //    {
            //        while (!_cancellationTokenSource.IsCancellationRequested &&
            //               (bytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0)
            //        {
            //            _bufferedWaveProvider.AddSamples(buffer, 0, bytesRead);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Error: {ex.Message}");
            //    }
            //}
        }

        public void InviteRequestSent(DeskConnectionInformation deskConnectionInformation)
        {
            try
            {
                using (TcpClient client = new TcpClient(ClientIpaddress, PORT))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    string json = JsonConvert.SerializeObject(deskConnectionInformation);
                    writer.Write(json);
                    writer.Flush();
                }
            }
            catch
            {
                ConnectionEstabishFailInvoke?.Invoke(this, EventArgs.Empty);
            }

        }

        public async Task ConnectToServer(DeskConnectionInformation deskConnectionInformation)
        {
            DeskConnectionInformation = deskConnectionInformation;
            try
            {
                client = new TcpClient(ClientIpaddress, PORT);
                stream = client.GetStream();
                isConnected = true;
                sendConnectRequest(deskConnectionInformation);
                HandleServerReponseDatas();
            }
            catch (Exception ex)
            {
                DisConnectedToSeverInvoke?.Invoke(this, ex.Message);
                isConnected = false;
            }
        }

        private async Task sendConnectRequest(DeskConnectionInformation deskConnectionInformation)
        {
            string json = JsonConvert.SerializeObject(new Data() { DataObject = deskConnectionInformation, ReponseAndReqType = ReponseAndReqType.ConnectReq });
            WriteObject(stream, json);
        }

        private void WriteObject(Stream stream, byte[] data)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(data.Length);
                writer.Write(data);
                writer.Flush();
            }
        }
        private void WriteObject(Stream stream, string json)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] lengthPrefix = BitConverter.GetBytes(data.Length); // 4 bytes

            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }

        private async Task HandleServerReponseDatas()
        {
            while (isConnected)
            {
                try
                {
                    // Read the image data length from the stream (assuming length is sent as an int before image data)
                    byte[] lengthBuffer = new byte[4];
                    await stream.ReadAsync(lengthBuffer, 0, 4);
                    int imageLength = BitConverter.ToInt32(lengthBuffer, 0);
                    // Read the actual image data
                    byte[] dataBuffer = new byte[imageLength];
                    int bytesRead = 0;
                    while (bytesRead < imageLength)
                    {
                        bytesRead += await stream.ReadAsync(dataBuffer, bytesRead, imageLength - bytesRead);
                    }
                    string json = Encoding.UTF8.GetString(dataBuffer);
                    var data = JsonConvert.DeserializeObject<Data>(json);

                    if (data.ReponseAndReqType == ReponseAndReqType.ScreenShareImageData)
                        ScreenShareUpdateInvoke?.Invoke(this, data.GetDeserializeDeskImageData());
                    else if (data.ReponseAndReqType == ReponseAndReqType.ReqResponse)
                    {
                        DeskConnectionInformation deskConnectionInformation = data.GetDeserializeDeskConnectionInformation();
                        if (deskConnectionInformation.Status == true && deskConnectionInformation.IsRequestStatusUpdate == true)
                        {
                            DeskConnectionInformation.Status = true;
                            DeskConnectionInformation.ScreenInformation = deskConnectionInformation.ScreenInformation;
                        }
                        ConnectionResponseReceived?.Invoke(this, deskConnectionInformation);
                    }

                }
                catch (IOException ex) when (ex.InnerException is SocketException socketEx && (socketEx.SocketErrorCode == SocketError.ConnectionReset || socketEx.SocketErrorCode == SocketError.ConnectionAborted))
                {
                    isConnected = false;
                    DisConnectedToSeverInvoke?.Invoke(this, "Error at receiving reponse data: " + ex.Message);
                }
                catch (Exception ex)
                {
                    DisConnectedToSeverInvoke?.Invoke(this, "Error at receiving reponse data: " + ex.Message);
                    isConnected = false;
                }
            }
        }

        public void SendMouseScrollEvent(ControlKeyType controlKeyType, double x, double y, double delta = 0)
        {
            if (client != null && client.Connected && deskConnectionInformation.MouseAccess)
            {
                NetworkStream stream = client.GetStream();
                // Get the client's screen resolution
                string scrollData = $"{x},{y}:{SystemInformationManager.ScreenWidth},{SystemInformationManager.ScreenHeight}:{delta}";
                Data data = new Data();
                data.ReponseAndReqType = ReponseAndReqType.ScreenShareKeyData;
                data.DataObject = new DeskControlData()
                {
                    ControlDataType = controlKeyType,
                    ControlData = scrollData
                };
                string json = JsonConvert.SerializeObject(data);
                WriteObject(stream, json);
            }
        }

        public void SendMouseEvent(ControlKeyType controlKeyType, Point position, double ScreenImageActualWidth, double ScreenImageActualHeight, Manager.Classes.DataSchema.Screens.Screen selectedScreen=null)
        {
            if (client != null && client.Connected && deskConnectionInformation.MouseAccess)
            {
                NetworkStream stream = client.GetStream();
                // Get the client's screen resolution

                //string mouseData = $"{(position.X / ScreenImageActualWidth) * SystemInformationManager.ScreenWidth},{(position.Y / ScreenImageActualHeight) * SystemInformationManager.ScreenHeight}:{SystemInformationManager.ScreenWidth},{SystemInformationManager.ScreenHeight}";
                string mouseData = $"{(position.X / ScreenImageActualWidth)+(selectedScreen?.Bounds.X ?? 0) },{(position.Y / ScreenImageActualHeight)+ (selectedScreen?.Bounds.Y ?? 0)}:{SystemInformationManager.ScreenWidth},{SystemInformationManager.ScreenHeight}";
                Data data = new Data();
                data.ReponseAndReqType = ReponseAndReqType.ScreenShareKeyData;
                data.DataObject = new DeskControlData()
                {
                    ControlDataType = controlKeyType,
                    ControlData = mouseData
                };
                string json = JsonConvert.SerializeObject(data);
                WriteObject(stream, json);
            }
        }

        public void SendScreenSwitchEvent(Classes.DataSchema.Screens.Screen e)
        {
            if (client == null && !client.Connected)
                return;
            Data data = new Data();
            data.ReponseAndReqType = ReponseAndReqType.ScreenShareKeyData;
            data.DataObject = new DeskControlData()
            {
                ControlDataType = ControlKeyType.ScreenSwitch,
                ControlData = $"{DeskConnectionInformation.ScreenInformation.Screens.IndexOf(e)}:{JsonConvert.SerializeObject(e)}"
            };
            string json = JsonConvert.SerializeObject(data);
            WriteObject(client.GetStream(), json);
        }
        //clipboard


        public void SendClipboardContentToServer()
        {
            //if (client != null && client.Connected && deskConnectionInformation.ClipboardAccess)
            //{
            //    NetworkStream stream = client.GetStream();
            //    if (Clipboard.ContainsText())
            //    {
            //        string clipboardText = Clipboard.GetText();
            //        writer.WriteLine($"ClipboardText:{clipboardText}");
            //        writer.Flush();
            //    }
            //    // You can handle other clipboard content types (e.g., images) similarly
            //}
        }


        //KeyPress

        public void ReceiveClipboard()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int byteRead = stream.Read(buffer, 0, buffer.Length);
                if (byteRead > 0)
                {
                    string receiveText = Encoding.UTF8.GetString(buffer, 0, byteRead);
                    System.Windows.Clipboard.SetText(receiveText);
                }
            }
        }

        //public void Window_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (deskConnectionInformation.KeyboardAccess)
        //    {
        //        if (e.Key == Key.V && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
        //        {
        //            SendClipboardContentToServer();
        //        }
        //        if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
        //        {
        //            //  receiveThread = new Thread(ReceiveClipboard);
        //        }
        //        SendKeyEvent(e.Key, "KeyDown");
        //    }

        //}

        //public void Window_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (deskConnectionInformation.KeyboardAccess)
        //    {
        //        SendKeyEvent(e.Key, "KeyUp");
        //    }
        //}

        public void SendKeyEvent(ControlKeyType controlKeyType, Key key)
        {
            if (client != null && client.Connected)
            {
                NetworkStream stream = client.GetStream();
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
                string json = JsonConvert.SerializeObject(data);
                WriteObject(stream, json);
            }
        }
        //Mouse Rightclick

        public void SendMouseRightEvent(ControlKeyType controlKeyType, Point position, double ScreenImageActualWidth, double ScreenImageActualHeight)
        {
            if (client != null && client.Connected)
            {
                NetworkStream stream = client.GetStream();
                // Get the client's screen resolution
                string keydata = $"{(position.X / ScreenImageActualWidth) * SystemInformationManager.ScreenWidth},{(position.Y / ScreenImageActualHeight) * SystemInformationManager.ScreenHeight}:{SystemInformationManager.ScreenWidth},{SystemInformationManager.ScreenHeight}";
                Data data = new Data();
                data.ReponseAndReqType = ReponseAndReqType.ScreenShareKeyData;
                data.DataObject = new DeskControlData()
                {
                    ControlDataType = controlKeyType,
                    ControlData = keydata
                };
                string json = JsonConvert.SerializeObject(data);
                WriteObject(stream, json);
            }
        }

        public void ConnectionClose()
        {
            if (client != null)
                client.Close();
            if (AudioTcpClient != null)
                AudioTcpClient.Close();
        }


    }
}


