using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    /// <summary>
    /// Use to handle client messages
    /// </summary>
    internal class Client
    {
        internal int ClientID;
       // internal int ClientToID;
       // internal bool ClintAlive;
        internal DateTime LastOnlineTime;
        Queue<Message> outgoingMessageQueue;
        Thread sendMessageThread;

        internal Server MyServer;
        public NetworkStream MyStream { get; set; }

        public Client(int _clientID, DateTime dateTime)
        {
            // TODO: Complete member initialization
            ClientID = _clientID;
            LastOnlineTime = dateTime;
            outgoingMessageQueue = new Queue<Message>();
            sendMessageThread = new Thread(() => sendTopMessage());
            sendMessageThread.Start();
        }

        internal void TransmitMessage(Message message)
        {
            // add the message into the queue
            outgoingMessageQueue.Enqueue(message);
            Console.WriteLine("Total message in queue:{0}", outgoingMessageQueue.Count);
        }


        private void sendTopMessage()
        {
            while (true)
            {
                if (outgoingMessageQueue.Count != 0)
                {
                    Message message = outgoingMessageQueue.Peek();

                    try
                    {
                        // for debug we send message back
                        if (CheckTargetClientExist(message.MsgFrom))
                        {
                            SendMessage(message);
                            outgoingMessageQueue.Dequeue();
                        }
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
               // else
                   // Console.WriteLine("No Message to be transmitted");
            }
        }

        private bool CheckTargetClientExist(int targetClientID)
        {
            Client targetClient = Server.clientList.Find(c => c.ClientID == targetClientID);

            if (targetClient != null)
            {
                return true;
            }
            return false;
        }

        private void SendMessage(Message msg)
        {
            lock (MyStream)
            {
                byte[] outStream = Encoding.ASCII.GetBytes(msg.TransmitMsg);

                MessageHeader header = new MessageHeader()
                {
                    msgFromID = ClientID,
                    msgToID = msg.MsgTo,
                    msgSize = outStream.Length
                };

                byte[] headerStream = Utility.GetBytesFromStruct(header);
                // transmit header
                MyStream.Write(headerStream, 0, headerStream.Length);

                MyStream.Write(outStream, 0, outStream.Length);

                MyStream.Flush();
            }
        }

        
    }
}
