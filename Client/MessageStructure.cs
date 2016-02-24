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
    }

}
