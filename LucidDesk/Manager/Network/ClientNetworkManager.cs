//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Net.Sockets;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Windows.Threading;

//namespace LucidDesk.Manager
//{
//    public class ClientNetworkManager
//    {
//        public event EventHandler<BitmapImage> ScreenShareUpdateInvoke;
//        public event EventHandler ConnectedToSeverInvoke;
//        public event EventHandler DisConnectedToSeverInvoke;
//        public string ClientIpaddress;
//        private TcpClient client;
//        private NetworkStream stream;
//        public bool isConnected = false;
//        private LowLevelKeyboardProc _proc;
//        private IntPtr _hookID = IntPtr.Zero;

//        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

//        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr GetModuleHandle(string lpModuleName);


//        public ClientNetworkManager()
//        {
//            _proc = HookCallback;
//            _hookID = SetHook(_proc);
//        }

//        private IntPtr SetHook(LowLevelKeyboardProc proc)
//        {
//            using (var curProcess = Process.GetCurrentProcess())
//            using (var curModule = curProcess.MainModule)
//            {
//                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0);
//            }
//        }

//        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
//        {
//            if (nCode >= 0 && (wParam == (IntPtr)0x0100 || wParam == (IntPtr)0x0104))
//            {
//                int vkCode = Marshal.ReadInt32(lParam);
//                bool isWindowsKey = (vkCode == 0x5B || vkCode == 0x5C);
//                bool isAltTab = (vkCode == 0x09 && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)));

//                if (isWindowsKey)
//                {
//                    // Send Windows key event to the server
//                    //SendKeyEvent((Key)vkCode, "KeyDown");
//                    SendKeyEvent((Key)vkCode, "WindowKey");
//                    return (IntPtr)1; // Suppress the key press locally
//                }
//                else if (isAltTab)
//                {
//                    // Handle Alt+Tab locally (don't send to server)
//                    SendKeyEvent((Key)vkCode, "AltTab");
//                    return (IntPtr)1; // Suppress the key press locally
//                }

//            }
//            return CallNextHookEx(_hookID, nCode, wParam, lParam);

//        }



//        public async Task ConnectToServer()
//        {
//            try
//            {
//                client = new TcpClient(ClientIpaddress, 9000);
//                stream = client.GetStream();
//                isConnected = true;
//                MessageBox.Show("Connected to server");
//                ConnectedToSeverInvoke?.Invoke(this, EventArgs.Empty);
//                while (isConnected)
//                {
//                    try
//                    {
//                        // Read the image data length from the stream (assuming length is sent as an int before image data)
//                        byte[] lengthBuffer = new byte[4];
//                        await stream.ReadAsync(lengthBuffer, 0, 4);
//                        int imageLength = BitConverter.ToInt32(lengthBuffer, 0);

//                        // Read the actual image data
//                        byte[] imageData = new byte[imageLength];
//                        int bytesRead = 0;
//                        while (bytesRead < imageLength)
//                        {
//                            bytesRead += await stream.ReadAsync(imageData, bytesRead, imageLength - bytesRead);
//                        }

//                        // Create the BitmapImage from the received image data
//                        using (MemoryStream memoryStream = new MemoryStream(imageData))
//                        {
//                            BitmapImage bitmap = new BitmapImage();
//                            bitmap.BeginInit();
//                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
//                            bitmap.StreamSource = memoryStream;
//                            bitmap.EndInit();
//                            bitmap.Freeze(); // Freeze the BitmapImage to make it cross-thread accessible

//                            ScreenShareUpdateInvoke?.Invoke(this, bitmap);

//                        }
//                    }
//                    catch (IOException ex) when (ex.InnerException is SocketException socketEx &&
//                                                    (socketEx.SocketErrorCode == SocketError.ConnectionReset ||
//                                                     socketEx.SocketErrorCode == SocketError.ConnectionAborted))
//                    {

//                        MessageBox.Show("Connection to the server was lost: " + ex.Message);
//                        isConnected = false;
//                        DisConnectedToSeverInvoke?.Invoke(this, EventArgs.Empty);
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show("Error receiving image data: " + ex.Message);
//                        isConnected = false;
//                        DisConnectedToSeverInvoke?.Invoke(this, EventArgs.Empty);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error connecting to server: " + ex.Message);
//                isConnected = false;
//                DisConnectedToSeverInvoke?.Invoke(this, EventArgs.Empty);
//            }
//        }
//        //MouseLeft



//        public void SendMouseEvent(Point position, string eventType, double ScreenImageActualWidth, double ScreenImageActualHeight)
//        {
//            if (client != null && client.Connected)
//            {
//                NetworkStream stream = client.GetStream();
//                StreamWriter writer = new StreamWriter(stream);

//                // Get the client's screen resolution
//                double screenWidth = SystemParameters.PrimaryScreenWidth;
//                double screenHeight = SystemParameters.PrimaryScreenHeight;

//                writer.WriteLine($"{eventType}:{(position.X / ScreenImageActualWidth) * screenWidth},{(position.Y / ScreenImageActualHeight) * screenHeight}:{screenWidth},{screenHeight}");
//                writer.Flush();
//            }
//        }



//        public void SendClipboardContentToServer()
//        {
//            if (client != null && client.Connected)
//            {
//                NetworkStream stream = client.GetStream();
//                StreamWriter writer = new StreamWriter(stream);

//                if (Clipboard.ContainsText())
//                {
//                    string clipboardText = Clipboard.GetText();
//                    writer.WriteLine($"ClipboardText:{clipboardText}");
//                    writer.Flush();
//                }
//                // You can handle other clipboard content types (e.g., images) similarly
//            }
//        }
//        //KeyPress

//        public void SendKeyEvent(Key key, string eventType)
//        {
//            if (client != null && client.Connected)
//            {
//                NetworkStream stream = client.GetStream();
//                StreamWriter writer = new StreamWriter(stream);

//                // Convert Key to virtual key code
//                byte virtualKeyCode = (byte)KeyInterop.VirtualKeyFromKey(key);
//                // Get the client's screen resolution
//                double screenWidth = SystemParameters.PrimaryScreenWidth;
//                double screenHeight = SystemParameters.PrimaryScreenHeight;

//                // Send the event to the server
//                writer.WriteLine($"{eventType}:{0},{0}:{screenWidth}:{screenHeight}:{virtualKeyCode}");
//                writer.Flush();
//            }
//        }
//        //Mouse Rightclick

//        public void SendMouseRightEvent(Point position, string eventType, double ScreenImageActualWidth, double ScreenImageActualHeight)
//        {
//            if (client != null && client.Connected)
//            {
//                NetworkStream stream = client.GetStream();
//                StreamWriter writer = new StreamWriter(stream);

//                // Get the client's screen resolution
//                double screenWidth = SystemParameters.PrimaryScreenWidth;
//                double screenHeight = SystemParameters.PrimaryScreenHeight;

//                writer.WriteLine($"{eventType}:{(position.X / ScreenImageActualWidth) * screenWidth},{(position.Y / ScreenImageActualHeight) * screenHeight}:{screenWidth},{screenHeight}");
//                writer.Flush();
//            }
//        }

//    }
//}

using LucidDesk.Manager.Classes;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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



namespace LucidDesk.Manager
{
  

    public partial class ClientNetworkManager
    {
        public DeskConnectionInformation deskConnectionInformation;
        public DeskConnectionInformation DeskConnectionInformation {
            set{
                deskConnectionInformation = value;
                ClientIpaddress = deskConnectionInformation.ReceiverDesk.IPAddress;
            }
            get{
                return deskConnectionInformation;
            }
        }

        public event EventHandler<BitmapImage> ScreenShareUpdateInvoke;
        public event EventHandler ConnectedToSeverInvoke;
        public event EventHandler DisConnectedToSeverInvoke;
        public event EventHandler ConnectionEstabishFailInvoke;
        public ClientNetworkManager()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }
        private TcpClient _tcpClient;
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

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)0x0100 || wParam == (IntPtr)0x0104))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                bool isWindowsKey = (vkCode == 0x5B || vkCode == 0x5C);
                bool isAltTab = (vkCode == 0x09 && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)));
                bool isCtrlV = (vkCode == 0x56) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
                bool isClipboardOpen = (vkCode == 0x56) && (Keyboard.IsKeyUp(Key.LWin) || Keyboard.IsKeyUp(Key.RWin));

                if (isWindowsKey && !isClipboardOpen)
                {
                    // Send Windows key event to the server
                    //SendKeyEvent((Key)vkCode, "KeyDown");
                    SendKeyEvent((Key)vkCode, "WindowKey");
                    return (IntPtr)1;
                }
                else if (isClipboardOpen && !isCtrlV)
                {
                    SendKeyEvent((Key)vkCode, "ClipBoardOpen");
                    return (IntPtr)(1);
                }
                else if (isAltTab)
                {
                    // Handle Alt+Tab locally (don't send to server)
                    SendKeyEvent((Key)vkCode, "AltTab");
                    return (IntPtr)1; // Suppress the key press locally
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        }

        private void ConnectButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {

                Task.Run(() => ConnectToServer());
                _cancellationTokenSource = new CancellationTokenSource();
                _tcpClient = new TcpClient();
                try
                {
                    _tcpClient.Connect(ClientIpaddress, 12345);

                    _waveOut = new WaveOutEvent();
                    _bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2));
                    _waveOut.Init(_bufferedWaveProvider);
                    _waveOut.Play();

                    Thread receiveThread = new Thread(ReceiveAudio);
                    receiveThread.Start();
                }
                catch
                {
                    return;
                }
            }
        }

        private void ReceiveAudio()
        {
            using (var networkStream = _tcpClient.GetStream())
            {
                var buffer = new byte[1024];
                int bytesRead;

                try
                {
                    while (!_cancellationTokenSource.IsCancellationRequested &&
                           (bytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        _bufferedWaveProvider.AddSamples(buffer, 0, bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        public void InviteRequestSent(DeskConnectionInformation deskConnectionInformation)
        {
            try{
                using (TcpClient client = new TcpClient(ClientIpaddress, 5000))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    string json = JsonConvert.SerializeObject(deskConnectionInformation);
                    writer.Write(json);
                    writer.Flush();
                }
            }
            catch{
                ConnectionEstabishFailInvoke?.Invoke(this, EventArgs.Empty);
            }
           
        }

        public async Task ConnectToServer()
        {
            try
            {
                client = new TcpClient(ClientIpaddress, 8000);
                stream = client.GetStream();
                isConnected = true;
                MessageBox.Show("Connected to server");
                ConnectedToSeverInvoke?.Invoke(this, EventArgs.Empty);


                while (isConnected)
                {
                    try
                    {
                        // Read the image data length from the stream (assuming length is sent as an int before image data)
                        byte[] lengthBuffer = new byte[4];
                        await stream.ReadAsync(lengthBuffer, 0, 4);
                        int imageLength = BitConverter.ToInt32(lengthBuffer, 0);

                        // Read the actual image data
                        byte[] imageData = new byte[imageLength];
                        int bytesRead = 0;
                        while (bytesRead < imageLength)
                        {
                            bytesRead += await stream.ReadAsync(imageData, bytesRead, imageLength - bytesRead);
                        }

                        // Create the BitmapImage from the received image data
                        using (MemoryStream memoryStream = new MemoryStream(imageData))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = memoryStream;
                            bitmap.EndInit();
                            bitmap.Freeze(); // Freeze the BitmapImage to make it cross-thread accessible

                            // Update the UI on the UI thread
                            ScreenShareUpdateInvoke?.Invoke(this, bitmap);

                        }
                    }
                    catch (IOException ex) when (ex.InnerException is SocketException socketEx &&
                                                    (socketEx.SocketErrorCode == SocketError.ConnectionReset ||
                                                     socketEx.SocketErrorCode == SocketError.ConnectionAborted))
                    {
                       
                            MessageBox.Show("Connection to the server was lost: " + ex.Message);
                       
                        isConnected = false;
                        DisConnectedToSeverInvoke?.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                      
                            MessageBox.Show("Error receiving image data: " + ex.Message);
                        DisConnectedToSeverInvoke?.Invoke(this, EventArgs.Empty);
                        isConnected = false;
                    }
                }
            }
            catch (Exception ex)
            {
             
                    MessageBox.Show("Error connecting to server: " + ex.Message);
                DisConnectedToSeverInvoke?.Invoke(this, EventArgs.Empty);
                isConnected = false;
            }
        }


        public void SendMouseScrollEvent(string eventType, double x, double y, double delta = 0)
        {
            if (client != null && client.Connected&& deskConnectionInformation.MouseAccess)
            {
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);

                // Get the client's screen resolution
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;

                writer.WriteLine($"{eventType}:{x},{y}:{screenWidth},{screenHeight}:{delta}");
                writer.Flush();
            }
        }
       



        public void SendMouseEvent(Point position, string eventType, double ScreenImageActualWidth, double ScreenImageActualHeight)
        {
            if (client != null && client.Connected&& deskConnectionInformation.MouseAccess)
            {
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);

                // Get the client's screen resolution
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;

                writer.WriteLine($"{eventType}:{(position.X / ScreenImageActualWidth) * screenWidth},{(position.Y / ScreenImageActualHeight) * screenHeight}:{screenWidth},{screenHeight}");
                writer.Flush();
            }
        }
        //clipboard


        public void SendClipboardContentToServer()
        {
            if (client != null && client.Connected && deskConnectionInformation.ClipboardAccess)
            {
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);

                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    writer.WriteLine($"ClipboardText:{clipboardText}");
                    writer.Flush();
                }
                // You can handle other clipboard content types (e.g., images) similarly
            }
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
                    Clipboard.SetText(receiveText);
                }
            }
        }

        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
        if(deskConnectionInformation.KeyboardAccess){
                if (e.Key == Key.V && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    SendClipboardContentToServer();
                }
                if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    //  receiveThread = new Thread(ReceiveClipboard);
                }
                SendKeyEvent(e.Key, "KeyDown");
            }
           
        }

        public void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (deskConnectionInformation.KeyboardAccess)
            {
                SendKeyEvent(e.Key, "KeyUp");
            }
        }

        public void SendKeyEvent(Key key, string eventType)
        {
            if (client != null && client.Connected&& deskConnectionInformation.KeyboardAccess)
            {
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);

                // Convert Key to virtual key code
                byte virtualKeyCode = (byte)KeyInterop.VirtualKeyFromKey(key);
                // Get the client's screen resolution
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;

                // Send the event to the server
                writer.WriteLine($"{eventType}:{0},{0}:{screenWidth}:{screenHeight}:{virtualKeyCode}");
                writer.Flush();
            }
        }
        //Mouse Rightclick

        public void SendMouseRightEvent(Point position, string eventType, double ScreenImageActualWidth, double ScreenImageActualHeight)
        {
            if (client != null && client.Connected&& deskConnectionInformation.MouseAccess)
            {
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);

                // Get the client's screen resolution
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;

                writer.WriteLine($"{eventType}:{(position.X / ScreenImageActualWidth) * screenWidth},{(position.Y / ScreenImageActualHeight) * screenHeight}:{screenWidth},{screenHeight}");
                writer.Flush();
            }
        }
      


    }
}


