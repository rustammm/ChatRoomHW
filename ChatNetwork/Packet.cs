using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatNetwork
{
    public enum DataType
    {
        Message,
        LogIn,
        LogOut,
        Null
    }

    public class Packet 
    {

        public DataType Type { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }

        public Packet()
        {
            Type = DataType.Null;
            SenderName = null;
            Message = null;
        }

        public Packet(byte[] dataStream)
        {
            this.Type = (DataType)BitConverter.ToInt32(dataStream, 0);
            int senderNameLength = BitConverter.ToInt32(dataStream, 4);
            int messageLength = BitConverter.ToInt32(dataStream, 8);
            this.SenderName = senderNameLength > 0 ?
                Encoding.UTF8.GetString(dataStream, 12, senderNameLength) : null;

            this.Message = messageLength > 0 ?
                Encoding.UTF8.GetString(dataStream, 12 + senderNameLength , messageLength) : null;
        }

        public byte[] toBytes()
        {
            List<byte> dataStream = new List<byte>();
            int senderNameLength = this.SenderName == null ? 0 : this. SenderName.Length;
            int messageLength = this.Message == null ? 0 : this.Message.Length;

            dataStream.AddRange(BitConverter.GetBytes((int)this.Type));
            dataStream.AddRange(BitConverter.GetBytes((int)senderNameLength));
            dataStream.AddRange(BitConverter.GetBytes((int)messageLength));

            if (senderNameLength != 0)
            {
                dataStream.AddRange(Encoding.UTF8.GetBytes(this.SenderName));
            }
            if (messageLength != 0)
            {
                dataStream.AddRange(Encoding.UTF8.GetBytes(this.Message));
            }
            return dataStream.ToArray();
        }

    }
}
