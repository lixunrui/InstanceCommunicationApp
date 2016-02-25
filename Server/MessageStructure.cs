using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Server
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct MessageHeader
    {
        internal int msgFromID;
        internal int msgToID;
        internal int msgSize;
    }

    internal static class Utility
    {
        internal static int HeaderSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MessageHeader));

        internal static byte[] GetBytesFromStruct(MessageHeader header)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            writer.Write(header.msgFromID);
            writer.Write(header.msgToID);
            writer.Write(header.msgSize);

            return stream.ToArray();
        }

        internal static MessageHeader GetStructFromBytes(byte[] data)
        {
            //HeaderMsg header = new HeaderMsg();
            //int size = Marshal.SizeOf(header);
            //IntPtr ptr = Marshal.AllocHGlobal(size);
            //Marshal.Copy(data, 0, ptr, size);
            //header = (HeaderMsg)Marshal.PtrToStructure(ptr, data.GetType());
            //Marshal.FreeHGlobal(ptr);
            //return header;

            var reader = new BinaryReader(new MemoryStream(data));

            MessageHeader header = new MessageHeader();

            header.msgFromID = reader.ReadInt32();
            header.msgToID = reader.ReadInt32();
            header.msgSize = reader.ReadInt32();
          
            return header;
        }

        internal static bool CheckHeaderValid(MessageHeader header)
        {
            // both from and to id should not be zero
            if (header.msgFromID == 0 || header.msgToID == 0)
            {
                Console.WriteLine("Invalid Header From {0}, To {1}, drop off", header.msgFromID, header.msgToID);
                return false;
            }
            return true;
        }
    }

    internal class Message
    {
        int _msgFrom;

        public int MsgFrom
        {
            get { return _msgFrom; }
            set { _msgFrom = value; }
        }

        int _msgTo;

        public int MsgTo
        {
            get { return _msgTo; }
            set { _msgTo = value; }
        }

        DateTime _receivedTime;

        public DateTime ReceivedTime
        {
            get { return _receivedTime; }
            set { _receivedTime = value; }
        }

        string _transmitMsg;

        public string TransmitMsg
        {
            get { return _transmitMsg; }
            set { _transmitMsg = value; }
        }
    }

}
