#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Database;
using LucidDesk.Manager.Enum;
using LucidDesk.Manager.Classes.DataSchema;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Timer = System.Threading.Timer;
using LucidDesk.Manager.Security;

#endregion
namespace LucidDesk.Manager
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
    public partial class ServerNetworkManager
    {
        private TcpListener _tcpListener;
        public const int PORT = 12345;
        private CancellationTokenSource _cancellationTokenSource;
        private Dictionary<TcpClient, DeskConnectionInformation> connections = new Dictionary<TcpClient, DeskConnectionInformation>();
        Dictionary<string, DeskConnectionInformation> InviteInformation = new Dictionary<string, DeskConnectionInformation>();
        private List<TcpClient> CurrentClients = new List<TcpClient>();
        private Dictionary<string, TcpClient> connectedClients = new Dictionary<string, TcpClient>();  // Track connected clients by MAC address
        public bool isStarted = false;
        private Timer _remainTimer;

        public event EventHandler<DeskConnectionInformation> ConnectRequestStatusInvoke;
        public event EventHandler<DeskConnectionInformation> InviteRequestReceivedInvoke;
        public event EventHandler<DeskConnectionInformation> ConnectRequestReceivedInvoke;

        private Thread _listenerThread;
        private CancellationTokenSource cancellationTokenSource;
        private TcpListener server;
        private Thread listenerThread;
        Dictionary<string, DeskConnectionInformation> DeskConnectionInformationList = new Dictionary<string, DeskConnectionInformation>();
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

        System.Windows.Forms.Timer ScreenShareTimer = new System.Windows.Forms.Timer();
        public void StartServer()
        {
            if (isStarted) return;

            _cancellationTokenSource = new CancellationTokenSource();
            _tcpListener = new TcpListener(IPAddress.Any, PORT);
            _tcpListener.Start();
            Task.Run(() => AcceptClients(_cancellationTokenSource.Token));

            // Start a timer to check for idle/expired connections
            _remainTimer = new System.Threading.Timer(RemainingClientsCheck, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));  // Check every 30 seconds

            isStarted = true;
        }

        private async Task AcceptClients(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();
                    HandleNewClient(client);
                }
            }
            catch (Exception ex)
            {
                StopServer();
            }
        }

        private void HandleNewClient(TcpClient client)
        {
            var networkStream = client.GetStream();
            HandleClientMessages(client);

        }

        private void HandleReceivedData(TcpClient client, Data data)
        {
            if (data == null)
                return;
            if (data.ReponseAndReqType == ReponseAndReqType.ConnectReq)
            {
                var connectionInformation = data.GetDeserializeDeskConnectionInformation();
                connectionInformation.SenderDesk.Freeze();
                connectionInformation.ReceiverDesk.Freeze();
                connectionInformation.TcpClient = client;
                connections.Add(client, connectionInformation);
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
                    ConnectRequestReceivedInvoke?.Invoke(this, connectionInformation);
                }
                else if ( connectionInformation.ConnectionType == ConnectionType.Invite)
                {
                    InviteRequestReceivedInvoke?.Invoke(this, connectionInformation);
                }
            }
            else if (data.ReponseAndReqType == ReponseAndReqType.ReqResponse)
            {
                var connectionInformation = data.GetDeserializeDeskConnectionInformation();
                connectionInformation.SenderDesk?.Freeze();
                connectionInformation.ReceiverDesk?.Freeze();
                connectionInformation.TcpClient = client;
                connections.Add(client, connectionInformation);
                if (connectionInformation.ConnectionType == ConnectionType.Invite&&InviteInformation.ContainsKey(connectionInformation.InviteID) &&connectionInformation.Status == true && connectionInformation.IsRequestStatusUpdate == true )
                {
                    connectionInformation.AddDeskScreensInformations();
                    RequestUpdate(connectionInformation);
                }
            }
            else if (data.ReponseAndReqType == ReponseAndReqType.ScreenShareKeyData && connections[client].Status == true)
            {
                HandleRemoteEvent(data.GetDeserializeDeskControlData());
            }
        }

        bool IsScreenShareON = false;
        private async void ScreenShareForClients(CancellationToken token)
        {
            try
            {
                IsScreenShareON = true;
                while (CurrentClients.Count > 0)
                {
                    await Task.Delay(40);
                    var screenImage = GetScreenShareImage();
                    Data data = new Data();
                    data.ReponseAndReqType = ReponseAndReqType.ScreenShareImageData;
                    data.DataObject = new DeskImageData() { ImageData = screenImage };
                    foreach (TcpClient client in CurrentClients.ToList())
                    {
                        if (client.Connected)
                        {
                            NetworkStream stream = client.GetStream();
                            WriteObject(stream, data);
                        }
                    }
                }
                IsScreenShareON = false;
            }
            catch (Exception ex)
            {
                IsScreenShareON = false;
            }
        }

        private void WriteObject(Stream stream, Data dataObject)
        {
            var json = JsonConvert.SerializeObject(dataObject);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] lengthPrefix = BitConverter.GetBytes(data.Length); // 4 bytes
            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            stream.Write(data, 0, data.Length);
            stream.Flush();

        }
        private void WriteObject(Stream stream, byte[] data)
        {
            byte[] lengthPrefix = BitConverter.GetBytes(data.Length); // 4 bytes

            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }
        private void WriteObject(Stream stream, string json)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] lengthPrefix = BitConverter.GetBytes(data.Length); // 4 bytes
            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }

        private byte[] GetScreenShareImage()
        {
            System.Drawing.Rectangle bounds;
            lock (selectedScreenLock)
            {
                if (selectedScreen == null)
                    bounds = SystemInformation.VirtualScreen;
                else bounds = selectedScreen.Bounds;
            }
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    var encoder = ImageCodecInfo.GetImageEncoders().First(e => e.FormatID == ImageFormat.Jpeg.Guid);
                    using (EncoderParameters encoderParams = new EncoderParameters(1))
                    {
                        encoderParams.Param[0] =
                            new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);
                        bitmap.Save(memoryStream, encoder, encoderParams);
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        private async Task HandleClientMessages(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                while (client.Connected)
                {
                    byte[] lengthBuffer = new byte[4];
                    await stream.ReadAsync(lengthBuffer, 0, 4);
                    int lengthToRead = BitConverter.ToInt32(lengthBuffer, 0);
                    if (lengthToRead==0)
                    client.Close();
                    byte[] dataBuffer = new byte[lengthToRead];
                    int bytesRead = 0;
                    while (bytesRead < lengthToRead)
                    {
                        bytesRead += await stream.ReadAsync(dataBuffer, bytesRead, lengthToRead - bytesRead);
                    }
                    string json = Encoding.UTF8.GetString(dataBuffer);
                    var data = JsonConvert.DeserializeObject<Data>(json);
                    HandleReceivedData(client, data);
                }
            }
            catch (IOException)
            {
                // Handle client disconnection
            }
        }

        private void HandleRemoteEvent(DeskControlData data) //"MouseDown:223.777777777778,232:1920,108j0" //"KeyDown:0,0:1920:1080:68"
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
                if (selectedScreen != null)
                {
                    System.Drawing.Rectangle virtualBounds = SystemInformation.VirtualScreen;
                    System.Drawing.Rectangle bounds = selectedScreen.Bounds;
                    screenX = (int)(bounds.X + (double)(x * selectedScreen.Bounds.Width));
                    screenY = (int)(bounds.Y + (double)(y * selectedScreen.Bounds.Height));
                }
                else
                {
                    System.Drawing.Rectangle virtualBounds = SystemInformation.VirtualScreen;
                    screenX = (int)(virtualBounds.X + (double)(x * (double)virtualBounds.Width));
                    screenY = (int)(virtualBounds.Y + (double)(y * (double)virtualBounds.Height));
                }
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(screenX, screenY);
                switch (data.ControlDataType)
                {
                    case ControlKeyType.MouseMove:
                        //ExecuteMouseMove(screenX, screenY);
                        break;
                    case ControlKeyType.MouseDown:
                        mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);
                        //ExecuteMouseButton(MOUSEEVENTF_LEFTDOWN);
                        break;
                    case ControlKeyType.MouseUp:
                        //ExecuteMouseMove(screenX, screenY);
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
                            MoniterScreenSwitch(screenIndex);
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
        private System.Windows.Forms.Screen selectedScreen = null;
        private readonly object selectedScreenLock = new object();
        private void MoniterScreenSwitch(int screenIndex)
        {
            lock (selectedScreenLock)
            {
                if (screenIndex < 0)
                {
                    selectedScreen = null;
                    return;
                }
                if (screenIndex < System.Windows.Forms.Screen.AllScreens.Length)
                    selectedScreen = System.Windows.Forms.Screen.AllScreens[screenIndex];
            }
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
                    dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE
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

        private void DisconnectClient(string macAddress)
        {
            if (connectedClients.ContainsKey(macAddress))
            {
                var client = connectedClients[macAddress];
                client.Close();
                connectedClients.Remove(macAddress);  // Remove from the list of connected clients
            }
        }

        public void StopServer()
        {
            if (!isStarted) return;

            _cancellationTokenSource.Cancel();
            _tcpListener.Stop();

            // Close all connected clients
            foreach (var client in connectedClients.Values)
            {
                client.Close();
            }

            connectedClients.Clear();
            isStarted = false;
        }

        // Remain Method to handle idle/expired clients
        private void RemainingClientsCheck(object state)
        {
            List<string> disconnectedClients = new List<string>();

            // Iterate over the connected clients and check if they are still alive
            foreach (var clientEntry in connectedClients)
            {
                try
                {
                    var client = clientEntry.Value;
                    if (client.Connected && client.GetStream().CanWrite)
                    {
                        // The client is still connected and able to send data
                    }
                    else
                    {
                        disconnectedClients.Add(clientEntry.Key);  // Add to remove list if disconnected
                    }
                }
                catch
                {
                    disconnectedClients.Add(clientEntry.Key);  // Add to remove list if an error occurred
                }
            }

            // Remove disconnected clients
            foreach (var macAddress in disconnectedClients)
            {
                DisconnectClient(macAddress);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopServer();
        }

        public void AddInviteReq(DeskConnectionInformation deskConnectionInformation)
        {
            InviteInformation.Add(deskConnectionInformation.InviteID, deskConnectionInformation);
        }

        public void RequestUpdate(DeskConnectionInformation deskConnectionInformation)
        {
            if (deskConnectionInformation.Status == true)
            {
                if (!DeskProfileManager.DeskProfilesDictionary.ContainsKey("" + deskConnectionInformation.SenderDesk.DeskId))
                    DeskProfileManager.CreateDeskProfiledata(deskConnectionInformation.SenderDesk);
                deskConnectionInformation.SenderDesk.RecentLoginTime = DateTime.Now;
            }
            else
            {
                deskConnectionInformation.SenderDesk.RecentLoginTime = DateTime.Now;
                DeskProfileManager.UpdateDeskProfiledata(deskConnectionInformation.SenderDesk);
            }

            CurrentClients.Add(deskConnectionInformation.TcpClient);
            if (!IsScreenShareON)
                Task.Run(() => ScreenShareForClients(_cancellationTokenSource.Token));

            deskConnectionInformation.ReceiverDesk = DeskProfileManager.UserDesk;

            Data data = new Data() { DataObject = deskConnectionInformation, ReponseAndReqType = ReponseAndReqType.ReqResponse };
            NetworkStream stream = deskConnectionInformation.TcpClient.GetStream();
            WriteObject(stream, data);
        }
    }
}




