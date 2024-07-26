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
//    public  class ServerNetworkManager
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
//        private bool  isCtrlpressed = false;
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
//                            MessageBox.Show("Error accepting client: " + ex.Message);
//                            Application.Current.Shutdown();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                    MessageBox.Show("Server stopped: " + ex.Message);
//                    Application.Current.Shutdown();
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
//                                HandleRemoteEvent(eventMessage);
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
//                        MessageBox.Show("Error capturing screen: " + ex.Message);
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
//                    Clipboard.SetText(clipboardText);
//                return;
//            }

//                // Calculate the scale factor based on the client's screen resolution
//                double scaleX = SystemParameters.PrimaryScreenWidth / clientScreenWidth;
//                double scaleY = SystemParameters.PrimaryScreenHeight / clientScreenHeight;

//                // Translate the image coordinates to screen coordinates
//                int screenX = (int)(x * scaleX);
//                int screenY = (int)(y * scaleY);

//                // Update the cursor position
//                System.Drawing.Point screenPos = new System.Drawing.Point(screenX, screenY);

//                switch (eventType)
//                {
//                    case "MouseDown":
//                        System.Windows.Forms.Cursor.Position = screenPos;
//                        mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
//                        break;
//                    case "MouseMove":
//                        System.Windows.Forms.Cursor.Position = screenPos;
//                        break;
//                    case "MouseUp":
//                        System.Windows.Forms.Cursor.Position = screenPos;
//                        mouse_event(MOUSEEVENTF_LEFTUP, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
//                        break;
//                    case "MouseRightDown":
//                        System.Windows.Forms.Cursor.Position = screenPos;
//                        mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
//                        break;
//                    case "MouseRightUp":
//                        System.Windows.Forms.Cursor.Position = screenPos;
//                        mouse_event(MOUSEEVENTF_RIGHTUP, (uint)screenX, (uint)screenY, 0, UIntPtr.Zero);
//                        break;
//                    case "KeyDown":
//                        keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
//                        break;
//                    case "KeyUp":
//                        keybd_event(keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
//                        break;
//                }
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace LucidDesk.Manager
{
    public class ServerNetworkManager
    {
        public Socket Socket;
        public int port=8000;
        private EndPoint EndPoint;
        private byte[] buffer;
        public event EventHandler OnclickServerStart;
        private UdpClient udpServer;
        private List<IPEndPoint> clients = new List<IPEndPoint>();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;
        private bool isCtrlpressed = false;
        public bool isStarted = false;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ToAscii(byte uVirtKey, byte uScanCode, byte[] pKeyState, out byte pChar, uint wFlags);

        public void StartListening()
        {
            udpServer = new UdpClient(8000);
            try
            {
                OnclickServerStart?.Invoke(this, EventArgs.Empty);
                Task.Run(() => ListenForClients(cts.Token), cts.Token);
                Task.Run(() => CaptureScreen(cts.Token), cts.Token);
                isStarted = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Server stopped: " + ex.Message);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private async Task ListenForClients(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult result = await udpServer.ReceiveAsync();
                    IPEndPoint clientEndPoint = result.RemoteEndPoint;
                    byte[] data = result.Buffer;

                    lock (clients)
                    {
                        if (!clients.Contains(clientEndPoint))
                        {
                            clients.Add(clientEndPoint);
                        }
                    }

                    string eventMessage = Encoding.UTF8.GetString(data);
                    HandleRemoteEvent(eventMessage);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error receiving data: " + ex.Message);
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
                                foreach (IPEndPoint client in clients.ToList())
                                {
                                    try
                                    {
                                        udpServer.Send(imageData, imageData.Length, client);
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Windows.MessageBox.Show("Error sending image data to client: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error capturing screen: " + ex.Message);
                }
                Thread.Sleep(100); // Capture the screen every 100ms
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
            cts.Cancel();
            lock (clients)
            {
                clients.Clear();
            }
            udpServer.Close();
            isStarted = false;
        }

        private void SendClipboard()
        {
            lock (clients)
            {
                foreach (IPEndPoint client in clients.ToList())
                {
                    if (System.Windows.Clipboard.ContainsText())
                    {
                        string clipboardText = System.Windows.Clipboard.GetText();
                        byte[] clipboardData = Encoding.UTF8.GetBytes($"ClipboardText:{clipboardText}");
                        udpServer.Send(clipboardData, clipboardData.Length, client);
                    }
                }
            }
        }

        private void HandleRemoteEvent(string eventMessage)
        {
            string[] parts = eventMessage.Split(':');
            string eventType = parts[0];
            byte keyCode = 0;
            double clientScreenWidth = 0, clientScreenHeight = 0, x = 0, y = 0;
            if (eventType.Contains("Mouse"))
            {
                string[] coords = parts[1].Split(',');
                x = double.Parse(coords[0]);
                y = double.Parse(coords[1]);
                string[] screenSize = parts[2].Split(',');
                clientScreenWidth = double.Parse(screenSize[0]);
                clientScreenHeight = double.Parse(screenSize[1]);
            }
            else if (eventType.Contains("Key"))
            {
                keyCode = byte.Parse(parts[4]);
                string[] coords = parts[1].Split(',');
                x = double.Parse(coords[0]);
                y = double.Parse(coords[1]);
                clientScreenWidth = double.Parse(parts[2]);
                clientScreenHeight = double.Parse(parts[3]);
            }
            else if (eventType == "ClipboardText")
            {
                string clipboardText = parts[1];
                System.Windows.Clipboard.SetText(clipboardText);
                return;
            }

            // Calculate the scale factor based on the client's screen resolution
            double scaleX = SystemParameters.PrimaryScreenWidth / clientScreenWidth;
            double scaleY = SystemParameters.PrimaryScreenHeight / clientScreenHeight;

            // Translate the image coordinates to screen coordinates
            int screenX = (int)(x * scaleX);
            int screenY = (int)(y * scaleY);

            // Update the cursor position
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
                case "KeyDown":
                    keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    break;
                case "KeyUp":
                    keybd_event(keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    break;
            }
        }
    }
}

