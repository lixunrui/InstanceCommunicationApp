using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    internal class Server
    {
        AutoResetEvent _serverEvent;
        int _msgSequence;
        internal static List<Client> clientList;

        TcpListener _serverListener;
        TcpClient _clientSocket;

        IPAddress _serverIP;

        const int PORT = 60000;

        internal Server()
        {
            _msgSequence = 0;
            clientList = new List<Client>();

            if (!IPAddress.TryParse("0.0.0.0", out _serverIP))
            {
                Console.WriteLine("Server Init Failed");
                return;
            }
            _serverListener = new TcpListener(_serverIP, PORT);

            _serverListener.ExclusiveAddressUse = false;

            _serverListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            _clientSocket = new TcpClient();
        }

        internal void Start(AutoResetEvent _serverEvent)
        {
            this._serverEvent = _serverEvent;

            StartServer();
        }

        private void StartServer()
        {
            if (_serverListener != null)
            {
                _serverListener.Start();
                Console.WriteLine("Server Started");
            }

            Thread listenClientConnectionThread = new Thread(()=> StartListening());

            listenClientConnectionThread.Start();

            _serverEvent.WaitOne();

            if (_serverListener != null)
            {
                _serverListener.Stop();
                listenClientConnectionThread.Abort(); 
            }

            Console.WriteLine("Server Thread Ends");
        }

        private void StartListening()
        {
            while (true)
            {
                try
                {
                    _clientSocket = _serverListener.AcceptTcpClient();
                    Console.WriteLine("Received connecting request.");

                    Thread clientHandleThread = new Thread(() => ClientHandler(_clientSocket));
                    clientHandleThread.Start();
                }
                catch (System.Exception ex)
                {
                }
            }
        }

        private void ClientHandler(TcpClient mySocket)
        {
            byte[] receivedBytes;

            MessageHeader header;
            int _clientID = 0 ;

            while (!_serverEvent.WaitOne(200) && (mySocket != null))
            {
                try
                {
                    receivedBytes = new byte[10025];

                    NetworkStream stream = mySocket.GetStream();
                    stream.Read(receivedBytes, 0, Utility.HeaderSize);

                    header = Utility.GetStructFromBytes(receivedBytes);

                    // workaround
                    if (!Utility.CheckHeaderValid(header))
                    {
                        if (header.msgFromID == 0)
                        {
                            throw new Exception();
                        }
                        continue;
                    }
                    if (_clientID != header.msgFromID)
                    {
                        Console.WriteLine("Client ID changed from {0} to {1}", _clientID, header.msgFromID);
                        _clientID = header.msgFromID;
                    }
                    
                    int _clientToID = header.msgToID;

                    Client c = isClientExist(_clientID);
                    if (c == null)
                    {
                        Console.WriteLine("Client {0} online", _clientID);
                        c = new Client(_clientID, DateTime.Now);
                        c.MyServer = this;
                        c.MyStream = stream;
                        clientList.Add(c);
                        Console.WriteLine("Client List added {0}", c.ClientID);
                    }

                    if (header.msgSize == 0)
                        continue;

                    stream.Read(receivedBytes, 0, header.msgSize);
                    string data = Encoding.ASCII.GetString(receivedBytes);
                    data = data.Substring(0, header.msgSize);

                    Message message = new Message() 
                    {
                        MsgFrom = _clientID,
                        MsgTo = header.msgToID,
                        ReceivedTime = DateTime.Now,
                        TransmitMsg = data
                    };
                    Console.WriteLine("Received data is: {0}", data);

                    c.TransmitMessage(message);

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    mySocket.Close();
                    mySocket = null;
                    Client c = isClientExist(_clientID);
                    if (c != null)
                    {
                        Console.WriteLine("Client {0} offline", _clientID);
                        clientList.Remove(c);
                    }
                }
                
            }// while

            Console.WriteLine("Server Listener Ends");
        }

        private Client isClientExist(int _clientID)
        {
            Client c = (Client)clientList.Find(x => x.ClientID == _clientID);
            if (c == null)
            {
                return null;
            }
            else
            {
                c.LastOnlineTime = DateTime.Now;
                return c;
            }
        }

        //internal bool SendTo(Message message)
        //{
        //    // check if the target client is online

        //    // if yes, then send and return true

        //    // if no, return false
        //}
    }
}
