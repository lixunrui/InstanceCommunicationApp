﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ClientApplication
{
    internal enum EventType
    {
        ServerEvent,
        ClientEvent
    }

    internal class Client
    {
        int _clientID;

        public int ClientID
        {
            get { return _clientID; }
            set { _clientID = value; }
        }

        Transmitter _transmitter;

        internal Transmitter Transmitter
        {
            get { return _transmitter; }
            set { _transmitter = value; }
        }

        Queue<Message> sendQueue;

        Thread sendMessageThread;

        ManualResetEvent _clientClosedEvent;

        internal delegate void ClientEventDelegate(object sender, EventType Type, object response);

        internal event ClientEventDelegate ClientEvent;

        internal Client(ManualResetEvent clientClosedEvent) 
        {
            _clientClosedEvent = clientClosedEvent;
            sendQueue = new Queue<Message>();
            _transmitter = new ClientApplication.Transmitter(clientClosedEvent);
            _transmitter.ServerEvent += ServerEventHandler;
            _transmitter.InternalEvent += InternalEventHandler;
        }

        private void TransmitMessage()
        {
            while (!_clientClosedEvent.WaitOne(1))
            {
                if (sendQueue.Count > 0)
                {
                    Message message = sendQueue.Peek();
                    try
                    {
                        _transmitter.TransmitMessage(message);
                        sendQueue.Dequeue();
                    }
                    catch (System.Exception ex)
                    {
                        ServerEventHandler(null, ex.Message);
                    }
                }
                Thread.Sleep(200);
            }
        }

        // to receive any event from server
        void ServerEventHandler(object sender, object response)
        {
            if (ClientEvent != null)
            {
                ClientEvent(this, EventType.ServerEvent, response);
            }
        }

        private void InternalEventHandler(object sender, object response)
        {
            if (ClientEvent != null)
            {
                ClientEvent(this, EventType.ClientEvent, response);
            }
        }

        internal bool ConnectToServer(int clientID, string serverIP, int port, string password)
        {
            if (_transmitter.ConenctToServer(serverIP))
            {
                _clientID = clientID;

                // send server a message indicate online
                _transmitter.TransmitMessage(new Message()
                {
                    MsgFrom = clientID,
                    MsgTo = -1,
                    TransmitMsg=""
                });

                sendMessageThread = new Thread(() => TransmitMessage());
                sendMessageThread.Name = "Hello";

                sendMessageThread.Start();
                return true;
            }
            return false;
        }

        internal void SendMessage(int toClientID, string message)
        {
            sendQueue.Enqueue(new Message()
            {
                MsgFrom = _clientID,
                MsgTo = toClientID,
                TransmitMsg = message
            });
        }

        internal void Close()
        {
            if (sendMessageThread!=null)
            {
                if (sendMessageThread.IsAlive)
                {
                    sendMessageThread.Join();
                }   
            }
            
            _transmitter.Close();
        }
    }
}
