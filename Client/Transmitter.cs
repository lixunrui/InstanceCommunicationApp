using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace ClientApplication
{
    internal class Transmitter
    {
        TcpClient clientTcp;
        NetworkStream workStream;

        internal delegate void ServerkEventDelegate(object sender, object response);

        internal event ServerkEventDelegate ServerEvent;

        internal event ServerkEventDelegate InternalEvent;
        Thread listener;


        internal Transmitter()
        {
            clientTcp = new TcpClient();
            //Thread 
        }

        internal bool ConenctToServer()
        {
            try
            {
                TransmitInternalEvent("Connecting to server...");

                clientTcp.Connect("127.0.0.1", 60000);

                workStream = clientTcp.GetStream();

                TransmitInternalEvent("Connected to server");

                BeginMonitorNetwork();

                return true;

            }
            catch (System.Exception ex)
            {
                TransmitInternalEvent(ex.Message);
                return false;
            }
        }

        private void TransmitInternalEvent(string eventMessage)
        {
            if (InternalEvent != null)
            {
                InternalEvent(this, eventMessage);
            }
        }

        private void BeginMonitorNetwork()
        {
            listener = new Thread(()=>ListenNetWork());
            listener.Name = "WOrld";
            listener.Start();
        }

        private void ListenNetWork()
        {
            MessageHeader header;
            while (true)
            {
                try
                {
                	byte[] receivedBytes = new byte[10240];

                    workStream.Read(receivedBytes, 0, Utility.HeaderSize);

                    header = Utility.GetStructFromBytes(receivedBytes);
                    
                    workStream.Read(receivedBytes, 0, header.msgSize);

                    string data = Encoding.ASCII.GetString(receivedBytes);

                    data = data.Substring(0, header.msgSize);

                    Message message = new Message()
                    {
                        MsgFrom = header.msgFromID,
                        MsgTo = header.msgToID,
                        ReceivedTime = DateTime.Now,
                        TransmitMsg = data
                    };

                    NoticeClient(message);
                }
                catch (System.Exception ex)
                {
                    TransmitInternalEvent(ex.Message);
                }
            }
        }

        void NoticeClient(Message msg)
        {
            if (ServerEvent != null)
            {
                ServerEvent(this, msg);
            }
        }

        internal void TransmitMessage(Message message)
        {
            byte[] outBytes;
            SendHeader(message, out outBytes);

            if (outBytes.Length > 0)
            {
                workStream.Write(outBytes, 0, outBytes.Length);
            }

            workStream.Flush();
            Console.WriteLine("Message Sent");
        }

        private void SendHeader(Message message, out byte[] outBytes)
        {
            MessageHeader header = new MessageHeader()
            {
                msgFromID = message.MsgFrom,
                msgToID = message.MsgTo
            };

            outBytes = new byte[10240];
            outBytes = Encoding.ASCII.GetBytes(message.TransmitMsg);

            header.msgSize = outBytes.Length;

            workStream.Write(Utility.GetBytesFromStruct(header), 0, Utility.HeaderSize);
        }

        internal void Close()
        {
            if (listener != null)
            {
                if (listener.IsAlive)
                {
                    Console.WriteLine("Stop the Listener thread...");
                    listener.Join();
                }
            }
            
            if (workStream != null)
            {
                Console.WriteLine("Stop the Sending stream");
                workStream.Close();
                workStream = null;
            }
            
            if (clientTcp != null)
            {
                Console.WriteLine("Stop the Tcp Client");
                clientTcp.Close();
                clientTcp = null;
            }
        }
    }
}
