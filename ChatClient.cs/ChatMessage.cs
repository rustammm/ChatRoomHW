using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatNetwork;

namespace ChatClient
{
    public class ChatMessage
    {
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DataType Type { get; set; }

        public ChatMessage()
        {
            SenderName = null;
            Message = null;
            Type = DataType.Null;
        }

        public ChatMessage(Packet packet)
        {
            SenderName = packet.SenderName;
            Message = packet.Message;
            Type = packet.Type;
        }

        Packet ToPacket()
        {
            Packet p = new Packet();
            p.SenderName = this.SenderName;
            p.Message = this.Message;
            p.Type = this.Type;
            return p;
        }

        override public string ToString()
        {
            if (this.Type == DataType.Message)
                return string.Format("{0}: {1}", this.SenderName, this.Message);
            else
                return this.Message;
        }
    }
}
