using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

namespace UdpTest
{
    public class Manager
    {
        //public class Server
        //{
        //    public event EventHandler<string> MessageServerInvoke;
        //    public const int PORT = 10000;
        //    private Socket _socket;
        //    private EndPoint _ep;
        //    private byte[] _buffer_recv;
        //    private ArraySegment<byte> _buffer_recv_segment;
        //    public void initialize()
        //    {
        //        _buffer_recv = new byte[4096];
        //        _buffer_recv_segment = new ArraySegment<byte>(_buffer_recv);
        //        _ep = new IPEndPoint(IPAddress.Any, PORT);
        //        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
        //        _socket.Bind(_ep);
        //    }
        //    public void StartMessageLoop()
        //    {
        //        _ = Task.Run(async () =>
        //        {
        //            SocketReceiveMessageFromResult res;
        //            while (true)
        //            {
        //                res = await _socket.ReceiveMessageFromAsync(_buffer_recv_segment, SocketFlags.None, _ep);
        //                string str = Encoding.UTF8.GetString(_buffer_recv, 0, res.ReceivedBytes);
        //                MessageServerInvoke?.Invoke(this, str);
        //                Console.WriteLine("Received:Server-" + Encoding.UTF8.GetString(_buffer_recv, 0, res.ReceivedBytes));
        //                await SendTo(res.RemoteEndPoint, Encoding.UTF8.GetBytes("Hello Back"));
        //            }
        //        });
        //    }
        //    public async Task SendTo(EndPoint Recipient, byte[] data)
        //    {
        //        var s = new ArraySegment<byte>(data);
        //        await _socket.SendToAsync(s, SocketFlags.None, Recipient);
        //    }
        //}
        //public class client
        //{
        //    public event EventHandler<string> MessageclientInvoke;
        //    private Socket _socket;
        //    private EndPoint _ep;
        //    private byte[] _buffer_recv;
        //    private ArraySegment<byte> _buffer_recv_segment;
        //    public void initialize(IPAddress iPAddress, int port)
        //    {
        //        _buffer_recv = new byte[4096];
        //        _buffer_recv_segment = new ArraySegment<byte>(_buffer_recv);
        //        _ep = new IPEndPoint(iPAddress, port);
        //        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
        //    }
        //    public void StartMessageLoop()
        //    {
        //        _ = Task.Run(async () =>
        //        {
        //            SocketReceiveMessageFromResult res;
        //            while (true)
        //            {
        //                res = await _socket.ReceiveMessageFromAsync(_buffer_recv_segment, SocketFlags.None, _ep);
        //                string str = Encoding.UTF8.GetString(_buffer_recv, 0, res.ReceivedBytes);
        //                MessageclientInvoke?.Invoke(this, str);
        //                Console.WriteLine("Received:client-" + Encoding.UTF8.GetString(_buffer_recv, 0, res.ReceivedBytes));
        //            }
        //        });
        //    }
        //    public async Task Send(byte[] data)
        //    {
        //        var s = new ArraySegment<byte>(data);
        //        await _socket.SendToAsync(s, SocketFlags.None, _ep);
        //    }
        //}

        //public class Client
        //{
        //    public event EventHandler<string> MessageClientInvoke;
        //    private Socket _socket;
        //    private EndPoint _ep;
        //    private byte[] _buffer_recv;
        //    private List<byte> _receivedBytes;

        //    public void Initialize(IPAddress iPAddress, int port)
        //    {
        //        _buffer_recv = new byte[4096]; // Initial buffer size
        //        _receivedBytes = new List<byte>();
        //        _ep = new IPEndPoint(iPAddress, port);
        //        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
        //    }

        //    public void StartMessageLoop()
        //    {
        //        _ = Task.Run(async () =>
        //        {
        //            while (true)
        //            {
        //                var res = await _socket.ReceiveMessageFromAsync(new ArraySegment<byte>(_buffer_recv), SocketFlags.None, _ep);
        //                _receivedBytes.AddRange(_buffer_recv.Take(res.ReceivedBytes));

        //                while (_receivedBytes.Count >= 4)
        //                {
        //                    int messageLength = BitConverter.ToInt32(_receivedBytes.ToArray(), 0);
        //                    if (_receivedBytes.Count >= messageLength + 4)
        //                    {
        //                        string str = Encoding.UTF8.GetString(_receivedBytes.Skip(4).Take(messageLength).ToArray());
        //                        MessageClientInvoke?.Invoke(this, str);
        //                        Console.WriteLine("Received: Client - " + str);
        //                        _receivedBytes.RemoveRange(0, messageLength + 4);
        //                    }
        //                    else
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //        });
        //    }

        //    public async Task Send(byte[] data)
        //    {
        //        var length = BitConverter.GetBytes(data.Length);
        //        var messageWithHeader = length.Concat(data).ToArray();
        //        var s = new ArraySegment<byte>(messageWithHeader);
        //        await _socket.SendToAsync(s, SocketFlags.None, _ep);
        //    }
        //}
  

public class Client
    {
        public event EventHandler<string> MessageClientInvoke;
        private Socket _socket;
        private EndPoint _ep;
        private byte[] _buffer_recv;
        private List<byte> _receivedBytes;

        public void Initialize(IPAddress ipAddress, int port)
        {
            _buffer_recv = new byte[4096]; // Initial buffer size
            _receivedBytes = new List<byte>();
            _ep = new IPEndPoint(ipAddress, port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
            _socket.Connect(_ep); // Ensure socket is connected to the server
        }

        public void StartMessageLoop()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var res = await _socket.ReceiveMessageFromAsync(new ArraySegment<byte>(_buffer_recv), SocketFlags.None, _ep);
                    _receivedBytes.AddRange(_buffer_recv.Take(res.ReceivedBytes));

                    try
                    {
                        while (true)
                        {
                            if (_receivedBytes.Count < 8) // sizeof(long) == 8
                            {
                                break; // Not enough data to read the message length
                            }

                            long messageLength = BitConverter.ToInt64(_receivedBytes.Take(8).ToArray(), 0);

                            // Check for a negative or unusually large message length
                            if (messageLength < 0 || messageLength > int.MaxValue)
                            {
                                Console.WriteLine("Invalid message length detected. Clearing buffer.");
                                _receivedBytes.Clear();
                                break;
                            }

                            if (_receivedBytes.Count < messageLength + 8)
                            {
                                break; // Not enough data to read the full message
                            }

                            string str = Encoding.UTF8.GetString(_receivedBytes.Skip(8).Take((int)messageLength).ToArray());
                            MessageClientInvoke?.Invoke(this, str);
                            Console.WriteLine("Received: Client - " + str);
                            _receivedBytes.RemoveRange(0, (int)messageLength + 8);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Exception: {e.Message}");
                        _receivedBytes.Clear(); // Clear buffer on exception to avoid potential infinite loops
                    }
                }
            });
        }

        public async Task Send(byte[] data)
        {
            var length = BitConverter.GetBytes((long)data.Length);
            var messageWithHeader = length.Concat(data).ToArray();
            var s = new ArraySegment<byte>(messageWithHeader);
            await _socket.SendToAsync(s, SocketFlags.None, _ep);
        }

      
    }

        public class Server
        {
            public event EventHandler<string> MessageServerInvoke;
            public const int PORT = 10000;
            private Socket _socket;
            private EndPoint _ep;
            private byte[] _buffer_recv;
            private List<byte> _receivedBytes;

            public void Initialize()
            {
                _buffer_recv = new byte[4096]; // Adjusted buffer size for larger messages
                _receivedBytes = new List<byte>();
                _ep = new IPEndPoint(IPAddress.Any, PORT);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
                _socket.Bind(_ep);
            }

            public void StartMessageLoop()
            {
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        var res = await _socket.ReceiveMessageFromAsync(new ArraySegment<byte>(_buffer_recv), SocketFlags.None, _ep);
                        _receivedBytes.AddRange(_buffer_recv.Take(res.ReceivedBytes));

                        try
                        {
                            while (true)
                            {
                                if (_receivedBytes.Count < 8) // sizeof(long) == 8
                                {
                                    break; // Not enough data to read the message length
                                }

                                long messageLength = BitConverter.ToInt64(_receivedBytes.Take(8).ToArray(), 0);

                                // Check for a negative or unusually large message length
                                if (messageLength < 0 || messageLength > int.MaxValue)
                                {
                                    Console.WriteLine("Invalid message length detected. Clearing buffer.");
                                    _receivedBytes.Clear();
                                    break;
                                }

                                //if (_receivedBytes.Count < messageLength + 8)
                                //{
                                //    break; // Not enough data to read the full message
                                //}

                                string str = Encoding.UTF8.GetString(_receivedBytes.Skip(8).Take((int)messageLength).ToArray());
                                MessageServerInvoke?.Invoke(this, str);
                                Console.WriteLine("Received: Server - " + str);
                                await SendTo(res.RemoteEndPoint, Encoding.UTF8.GetBytes("Hello Back"));
                                _receivedBytes.RemoveRange(0, (int)messageLength + 8);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Exception: {e.Message}");
                            _receivedBytes.Clear(); // Clear buffer on exception to avoid potential issues
                        }
                    }
                });
            }

            public async Task SendTo(EndPoint recipient, byte[] data)
            {
                var length = BitConverter.GetBytes((long)data.Length);
                var messageWithHeader = length.Concat(data).ToArray();
                var s = new ArraySegment<byte>(messageWithHeader);
                await _socket.SendToAsync(s, SocketFlags.None, recipient);
            }
        }

    }
}
