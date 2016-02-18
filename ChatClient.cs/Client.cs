using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatNetwork;
using System.Net.Sockets;

namespace ChatClient
{
    public delegate void EventHandler(ChatMessage message);



    public class ChatClient
    {


        private event EventHandler messageReceived;
        private string name;
        private Socket clientSocket;

        private IPAddress serverIP;
        private IPEndPoint server;
        private EndPoint epServer;
        private byte[] dataStream;




        public ChatClient()
        {
            this.serverIP = null;
            this.server = null;
            this.epServer = null;
            this.dataStream = new byte[1024];
        }

        public ChatClient(string serverIP, int serverPort, string userName)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.dataStream = new byte[1024];

            this.serverIP = IPAddress.Parse(serverIP);
            this.name = userName;
            IPEndPoint server = new IPEndPoint(this.serverIP, serverPort);
            this.epServer = (EndPoint)server;


            sendMessage(null, DataType.LogIn);

            //Packet sendData = new Packet();
            //sendData.SenderName = userName;
            //sendData.Type = DataType.LogIn;
            //sendData.Message = null;

            //byte[] data = sendData.toBytes();

            //clientSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, epServer, new AsyncCallback(this.SendData), null);

            //clientSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None, ref epServer, new AsyncCallback(this.ReceiveData), null);



        }

        public void sendMessage(string message, DataType type)
        {
            Packet sendMessage = new Packet();
            sendMessage.SenderName = this.name;
            sendMessage.Type = type;
            sendMessage.Message = message;

           
            byte[] data = sendMessage.toBytes();

            clientSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, epServer, new AsyncCallback(this.SendData), null);
            clientSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None, ref epServer, new AsyncCallback(this.ReceiveData), null);
        }

        private void SendData(IAsyncResult ar)
        {
            clientSocket.EndSend(ar);
            
        }

        private void ReceiveData(IAsyncResult ar)
        {
            this.clientSocket.EndReceive(ar);

            Packet receivedData = new Packet(this.dataStream);


            this.messageReceived.Invoke(new ChatMessage(receivedData));


            this.dataStream = new byte[1024];

            clientSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None, ref epServer, new AsyncCallback(this.ReceiveData), null);
            
        }


        void addMessageReceiveHandler(EventHandler eventHander) {
            messageReceived += eventHander;
        }





    }
}
