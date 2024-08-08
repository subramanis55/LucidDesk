//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading;
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
//    public class ServerNetworkManager
//    {

//        public event EventHandler OnclickServerStart;
//        private TcpListener server;
//        private List<TcpClient> clients = new List<TcpClient>();
//        private CancellationTokenSource cts = new CancellationTokenSource();
//        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
//        private const uint MOUSEEVENTF_LEFTUP = 0x04;
//        private const uint KEYEVENTF_KEYDOWN = 0x0000;
//        private const uint KEYEVENTF_KEYUP = 0x0002;
//        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
//        private const uint MOUSEEVENTF_RIGHTUP = 0x10;
//        private bool isCtrlpressed = false;
//        public bool isStarted = false;

//        [DllImport("user32.dll", SetLastError = true)]
//        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

//        [DllImport("user32.dll", SetLastError = true)]
//        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

//        [DllImport("user32.dll", SetLastError = true)]
//        private static extern int ToAscii(byte uVirtKey, byte uScanCode, byte[] pKeyState, out byte pChar, uint wFlags);

//        public void StartListening()
//        {
//            server = new TcpListener(IPAddress.Any, 8000);
//            try
//            {
//                server.Start();
//                MessageBox.Show("Server started");
//                // Start capturing the screen
//                Task.Run(() => CaptureScreen(cts.Token));

//                while (!cts.Token.IsCancellationRequested)
//                {
//                    try
//                    {
//                        TcpClient client = server.AcceptTcpClient();
//                        lock (clients)
//                        {
//                            clients.Add(client);
//                        }
//                        Thread clientThread = new Thread(HandleClient)
//                        {
//                            IsBackground = true
//                        };
//                        clientThread.Start(client);
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show("Error accepting client: " + ex.Message);
//                        Application.Current.Shutdown();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Server stopped: " + ex.Message);
//                Application.Current.Shutdown();
//            }

//        }

//        private void HandleClient(object obj)
//        {
//            TcpClient client = (TcpClient)obj;
//            NetworkStream networkStream = client.GetStream();
//            try
//            {
//                using (StreamReader reader = new StreamReader(networkStream))
//                {
//                    while (true)
//                    {
//                        string eventMessage = reader.ReadLine();
//                        if (eventMessage != null)
//                        {
//                            HandleRemoteEvent(eventMessage);
//                        }
//                        else
//                        {  // If no data, the client might have disconnected
//                            break;
//                        }
//                    }
//                }
//            }
//            catch (IOException ex) when (ex.InnerException is SocketException socketEx &&
//                                        (socketEx.SocketErrorCode == SocketError.ConnectionReset ||
//                                         socketEx.SocketErrorCode == SocketError.ConnectionAborted))
//            {
//                StopServer();
//            }

//            finally
//            {
//                lock (clients)
//                {
//                    clients.Remove(client);
//                }
//                client.Close();
//            }
//        }

//        private void CaptureScreen(CancellationToken token)
//        {
//            while (!token.IsCancellationRequested)
//            {
//                try
//                {
//                    using (Bitmap bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight))
//                    {
//                        using (Graphics g = Graphics.FromImage(bitmap))
//                        {
//                            g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
//                        }

//                        using (MemoryStream memoryStream = new MemoryStream())
//                        {
//                            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
//                            byte[] imageData = memoryStream.ToArray();

//                            // Send the image data to all connected clients
//                            lock (clients)
//                            {
//                                foreach (TcpClient client in clients.ToList())
//                                {
//                                    if (client.Connected)
//                                    {
//                                        NetworkStream stream = client.GetStream();
//                                        BinaryWriter writer = new BinaryWriter(stream);
//                                        writer.Write(imageData.Length);
//                                        writer.Write(imageData);
//                                        writer.Flush();
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("Error capturing screen: " + ex.Message);
//                }
//                Thread.Sleep(3); // Capture the screen every 100ms
//            }
//        }

//        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
//        {
//            if (isStarted)
//            {
//                StopServer();
//            }
//        }

//        private void StopServer()
//        {
//            cts.Cancel();
//            lock (clients)
//            {
//                foreach (var client in clients)
//                {
//                    client.Close();
//                }
//                clients.Clear();
//            }
//            server.Stop();
//            //listenerThread.Join();
//            isStarted = false;
//        }

//        private void SendClipboard()
//        {
//            lock (clients)
//            {
//                foreach (var client in clients.ToList())
//                {
//                    if (client.Connected)
//                    {
//                        NetworkStream stream = client.GetStream();
//                        StreamWriter writer = new StreamWriter(stream);

//                        if (Clipboard.ContainsText())
//                        {
//                            string clipboardText = Clipboard.GetText();
//                            writer.WriteLine($"ClipboardText:{clipboardText}");
//                            writer.Flush();
//                        }
//                        // You can handle other clipboard content types (e.g., images) similarly
//                    }
//                }
//            }
//        }


//        private void HandleRemoteEvent(string eventMessage) //"MouseDown:223.777777777778,232:1920,1080" //"KeyDown:0,0:1920:1080:68"
//        {
//            string[] parts = eventMessage.Split(':');
//            string eventType = parts[0];
//            byte keyCode = 0;
//            double clientScreenWidth = 0, clientScreenHeight = 0, x = 0, y = 0;
//            if (eventType.Contains("Mouse"))
//            {
//                string[] coords = parts[1].Split(',');
//                x = double.Parse(coords[0]);
//                y = double.Parse(coords[1]);
//                string[] screenSize = parts[2].Split(',');
//                clientScreenWidth = double.Parse(screenSize[0]);
//                clientScreenHeight = double.Parse(screenSize[1]);
//            }
//            else if (eventType.Contains("Key"))
//            {
//                keyCode = byte.Parse(parts[4]);
//                string[] coords = parts[1].Split(',');
//                x = double.Parse(coords[0]);
//                y = double.Parse(coords[1]);
//                clientScreenWidth = double.Parse(parts[2]);
//                clientScreenHeight = double.Parse(parts[3]);
//            }
//            else if (eventType == "ClipboardText")
//            {
//                string clipboardText = parts[1];
//                Clipboard.SetText(clipboardText);
//                return;
//            }

//            // Calculate the scale factor based on the client's screen resolution
//            double scaleX = SystemParameters.PrimaryScreenWidth / clientScreenWidth;
//            double scaleY = SystemParameters.PrimaryScreenHeight / clientScreenHeight;

//            // Translate the image coordinates to screen coordinates
//            int screenX = (int)(x * scaleX);
//            int screenY = (int)(y * scaleY);

//            // Update the cursor position
//            System.Drawing.Point screenPos = new System.Drawing.Point(screenX, screenY);

//            switch (eventType)
//            {
//                case "MouseDown":
//                    System.Windows.Forms.Cursor.Position = screenPos;
//                    mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
//                    break;
//                case "MouseMove":
//                    System.Windows.Forms.Cursor.Position = screenPos;
//                    break;
//                case "MouseUp":
//                    System.Windows.Forms.Cursor.Position = screenPos;
//                    mouse_event(MOUSEEVENTF_LEFTUP, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
//                    break;
//                case "MouseRightDown":
//                    System.Windows.Forms.Cursor.Position = screenPos;
//                    mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
//                    break;
//                case "MouseRightUp":
//                    System.Windows.Forms.Cursor.Position = screenPos;
//                    mouse_event(MOUSEEVENTF_RIGHTUP, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
//                    break;
//                case "KeyDown":
//                    keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
//                    break;
//                case "KeyUp":
//                    keybd_event(keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
//                    break;
//            }
//        }
//    }
//}
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using System.Windows.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Database;
using LucidDesk.Manager.Enum;

namespace LucidDesk.Manager
{

    public partial class ServerNetworkManager
    {

        public event EventHandler OnclickServerStart;
        public event EventHandler<DeskConnectionInformation> InviteRequestReceivedInvoke;
        public event EventHandler<DeskConnectionInformation> ConnectRequestReceivedInvoke;
        public event EventHandler<DeskConnectionInformation> ConnectRequestStatusInvoke;
        public event EventHandler<DeskConnectionInformation> InviteRequestStatusInvoke;
        private TcpListener _tcpListener;
        private CancellationTokenSource _cancellationTokenSource;
        private Thread _listenerThread;
        private CancellationTokenSource cancellationTokenSource;
        private TcpListener server;
        private Thread listenerThread;
        Dictionary<string, DeskConnectionInformation> DeskConnectionInformationList = new Dictionary<string, DeskConnectionInformation>();
        private List<TcpClient> clients = new List<TcpClient>();
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;
        private const uint MOUSEEVENTF_HWHEEL = 0x01000;
        public bool isStarted = false, isMouseAcess, isKeyboardAcess, isAudioAcess, isClipboardAcess;
        private byte VK_TAB = 0x09, VK_MENU = 0x12;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
        };

        public void StartServer()
        {
            Thread listenerThread1 = new Thread(new ThreadStart(ReceiveInviteRequest))
            {
                IsBackground = true
            };
            listenerThread1.Start();


            if (isStarted) return;
            isAudioAcess = true;
            isKeyboardAcess = true;
            isClipboardAcess = true;
            isMouseAcess = true;
            cancellationTokenSource = new CancellationTokenSource();
            listenerThread = new Thread(new ThreadStart(StartListening))
            {
                IsBackground = true
            };
            listenerThread.Start();
            isStarted = true;
            if (isAudioAcess)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _tcpListener = new TcpListener(IPAddress.Any, 12345);
                _tcpListener.Start();
                _listenerThread = new Thread(() => AcceptClients(_cancellationTokenSource.Token));
                _listenerThread.Start();
            }
        }

        private void AcceptClients(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = _tcpListener.AcceptTcpClient();
                var clientThread = new Thread(() => HandleClient(client, cancellationToken));
                clientThread.Start();
            }
        }

        private void HandleClient(TcpClient client, CancellationToken cancellationToken)
        {
            for (int n = 0; n < WaveInEvent.DeviceCount; n++)
            {
                var capabilities = WaveInEvent.GetCapabilities(n);
                Console.WriteLine($"Device {n}: {capabilities.ProductName}");
            }
            using (var networkStream = client.GetStream())
            {
                var waveIn = new WaveInEvent();
                try
                {
                    waveIn = new WaveInEvent
                    {
                        WaveFormat = new WaveFormat(44100, 16, 2)
                    };

                    waveIn.DataAvailable += (s, a) =>
                    {
                        networkStream.Write(a.Buffer, 0, a.BytesRecorded);
                    };

                    waveIn.StartRecording();
                }
                catch (NAudio.MmException ex)
                {
                    MessageBox.Show("Error initializing audio input device: " + ex.Message);
                }


                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                    }
                }
                finally
                {
                    waveIn.StopRecording();
                }
            }

        }

        private void SendClipboardText(TcpClient client)
        {
            //NetworkStream stream = client.GetStream();
            //while (true)
            //{
            //    string clipboardText = GetClipboardText();
            //    if (!string.IsNullOrEmpty(clipboardText))
            //    {
            //        byte[] data = Encoding.UTF8.GetBytes(clipboardText);
            //        stream.Write(data, 0, data.Length);
            //    }
            //    Thread.Sleep(100);
            //}
        }

        private string GetClipboardText()
        {
            string text = string.Empty;
            Thread startThread = new Thread(
            delegate ()
            {
                try
                {
                    text = Clipboard.GetText();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error accessing clipboard: " + ex.Message);
                }
            });
            startThread.SetApartmentState(ApartmentState.STA);
            startThread.Start();
            startThread.Join();
            return text;
        }

        private void StartListening()
        {
            server = new TcpListener(IPAddress.Any, 8000);
            try
            {
                server.Start();
                MessageBox.Show("Server started");

                Task.Run(() => CaptureScreen(cancellationTokenSource.Token));

                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        TcpClient client = server.AcceptTcpClient();
                        lock (clients)
                        {
                            clients.Add(client);
                        }
                        Thread clientThread = new Thread(HandleClient)
                        {
                            IsBackground = true
                        };
                        clientThread.Start(client);
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Error accepting client: " + ex.Message);
                        StopServer();
                        Application.Current.Shutdown();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server stopped: " + ex.Message);
                StopServer();
                Application.Current.Shutdown();
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream networkStream = client.GetStream();
            Thread sendThread = new Thread(() => SendClipboardText(client))
            {
                IsBackground = true
            };
            sendThread.Start();
            try
            {
                using (StreamReader reader = new StreamReader(networkStream))
                {
                    while (true)
                    {
                        string eventMessage = reader.ReadLine();
                        if (eventMessage != null)
                        {

                            HandleRemoteEvent(eventMessage);
                        }
                        else
                        {
                            // If no data, the client might have disconnected
                            break;
                        }
                    }
                }
            }
            catch (IOException ex) when (ex.InnerException is SocketException socketEx &&
                                        (socketEx.SocketErrorCode == SocketError.ConnectionReset ||
                                         socketEx.SocketErrorCode == SocketError.ConnectionAborted))
            {
                StopServer();
                Application.Current.Shutdown();
            }
            finally
            {
                lock (clients)
                {
                    clients.Remove(client);
                }
                client.Close();
            }
        }
        private void ReceiveInviteRequest()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            // Console.WriteLine("Server started...");

            while (true)
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string json = reader.ReadToEnd();
                    DeskConnectionInformation deskConnectionInformation = JsonConvert.DeserializeObject<DeskConnectionInformation>(json, JsonSettings);
                    if (deskConnectionInformation.ReceiverDesk.MacAddress == DeskProfileManager.UserDesk.MacAddress  )
                    {
                        if (deskConnectionInformation.ConnectionType == ConnectionType.Invite)
                        {
                            InviteRequestReceivedInvoke?.Invoke(this, deskConnectionInformation);
                        }
                        else if (deskConnectionInformation.ConnectionType == ConnectionType.Connect)
                        {
                            ConnectRequestReceivedInvoke?.Invoke(this, deskConnectionInformation);
                        }
                    }


                    if (deskConnectionInformation.SenderDesk.MacAddress == DeskProfileManager.UserDesk.MacAddress)
                    {

                        if (deskConnectionInformation.ConnectionType == ConnectionType.Invite)
                        {
                            InviteRequestStatusInvoke?.Invoke(this, deskConnectionInformation);
                        }
                        else if (deskConnectionInformation.ConnectionType == ConnectionType.Connect)
                        {
                            ConnectRequestStatusInvoke?.Invoke(this, deskConnectionInformation);
                        }
                    }
                }
            }
        }



        private void CaptureScreen(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (Bitmap bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                        }

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] imageData = memoryStream.ToArray();

                            // Send the image data to all connected clients
                            lock (clients)
                            {
                                foreach (TcpClient client in clients.ToList())
                                {
                                    if (client.Connected)
                                    {
                                        NetworkStream stream = client.GetStream();
                                        BinaryWriter writer = new BinaryWriter(stream);
                                        writer.Write(imageData.Length);
                                        writer.Write(imageData);
                                        writer.Flush();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error capturing screen: " + ex.Message);

                }
                //Thread.Sleep(3); // Capture the screen every 100ms
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isStarted)
            {
                StopServer();
            }
        }

        private void StopServer()
        {
            lock (clients)
            {
                foreach (var client in clients)
                {
                    client.Close();
                }
                clients.Clear();
            }
            server.Stop();
            listenerThread.Join();
            isStarted = false;
            _cancellationTokenSource.Cancel();
            _tcpListener.Stop();
            _listenerThread.Join();
        }

        private void SendClipboard()
        {
            lock (clients)
            {
                foreach (var client in clients.ToList())
                {
                    if (client.Connected)
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
            }
        }


        private void HandleRemoteEvent(string eventMessage) //"MouseDown:223.777777777778,232:1920,108j0" //"KeyDown:0,0:1920:1080:68"
        {
            string[] parts = eventMessage.Split(':');
            string eventType = parts[0];
            double delta = 0;
            byte keyCode = 0;
            double clientScreenWidth = 0, clientScreenHeight = 0, x = 0, y = 0;
            if (eventType.Contains("Mouse") && isMouseAcess)
            {
                string[] coords = parts[1].Split(',');
                x = double.Parse(coords[0]);
                y = double.Parse(coords[1]);
                string[] screenSize = parts[2].Split(',');
                clientScreenWidth = double.Parse(screenSize[0]);
                clientScreenHeight = double.Parse(screenSize[1]);
                double scaleX = SystemParameters.PrimaryScreenWidth / clientScreenWidth;
                double scaleY = SystemParameters.PrimaryScreenHeight / clientScreenHeight;
                if (eventType.Contains("Scroll"))
                {
                    delta = double.Parse(parts[3]);
                }
                int screenX = (int)(x * scaleX);
                int screenY = (int)(y * scaleY);
                System.Drawing.Point screenPos = new System.Drawing.Point(screenX, screenY);

                switch (eventType)
                {
                    case "MouseDown":
                        System.Windows.Forms.Cursor.Position = screenPos;
                        mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
                        break;
                    case "MouseMove":
                        System.Windows.Forms.Cursor.Position = screenPos;
                        break;
                    case "MouseUp":
                        System.Windows.Forms.Cursor.Position = screenPos;
                        mouse_event(MOUSEEVENTF_LEFTUP, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
                        break;
                    case "MouseRightDown":
                        System.Windows.Forms.Cursor.Position = screenPos;
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
                        break;
                    case "MouseRightUp":
                        System.Windows.Forms.Cursor.Position = screenPos;
                        mouse_event(MOUSEEVENTF_RIGHTUP, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
                        break;
                    case "MouseScroll":
                        mouse_event(MOUSEEVENTF_WHEEL, (uint)screenX, (uint)screenY, (uint)delta, UIntPtr.Zero);
                        break;
                }
            }
            else if (eventType.Contains("Key") && isKeyboardAcess)
            {
                keyCode = byte.Parse(parts[4]);
                string[] coords = parts[1].Split(',');
                x = double.Parse(coords[0]);
                y = double.Parse(coords[1]);
                if (eventType == "KeyDown") keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                else if (eventType == "KeyUp") keybd_event(keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            }
            else if (eventType == "ClipboardText")
            {
                string clipboardText = parts[1];
                Clipboard.SetText(clipboardText);
                return;
            }
            else if (eventType == "ClipBoardOpen" && isClipboardAcess)
            {
                keybd_event(0x5B, 0, 0, IntPtr.Zero);
                keybd_event(0x56, 0, 0, IntPtr.Zero);
                keybd_event(0x56, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                keybd_event(0x5B, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }
            switch (eventType)
            {
                case "AltTab":
                    keybd_event(VK_MENU, 0, 0, IntPtr.Zero);
                    keybd_event(VK_TAB, 0, 0, IntPtr.Zero);
                    break;
                case "WindowKey":
                    keybd_event(0x5B, 0, 0, IntPtr.Zero);
                    keybd_event(0x5B, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                    break;
            }
        }
    }
}



