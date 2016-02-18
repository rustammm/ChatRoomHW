using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatNetwork;

namespace ChatServer
{

    public class ChatServer
    {

        public const int PORT = 30000;

        private struct Client
        {
            public EndPoint endPoint;
            public string name;
        }

        private List<Client> clientList;
        private Socket serverSocket;
        private byte[] dataStream = new byte[1024];


        void start()
        {
            try
            {
                this.clientList = new List<Client>();
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint server = new IPEndPoint(IPAddress.Any, PORT);
                serverSocket.Bind(server);
                IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0); // ?????????????????????????
                EndPoint epSender = (EndPoint)clients;
                serverSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None, 
                    ref epSender, new AsyncCallback(ReceiveData), epSender);
                

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        void ReceiveData(IAsyncResult asyncResult)
        {
            byte[] data;
            Packet received = new Packet(this.dataStream);
            Packet sendData = new Packet();
            sendData.SenderName = received.SenderName;
            sendData.Type = received.Type;
            IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);
            EndPoint epSender = (EndPoint)clients;
            serverSocket.EndReceiveFrom(asyncResult, ref epSender);


            switch (received.Type)
            {
                case DataType.Message:
                    sendData.Message = received.Message;
                    break;
                case DataType.LogIn:
                    Client c = new Client();
                    c.name = received.SenderName;
                    c.endPoint = epSender;

                    this.clientList.Add(c);
                    sendData.Message = string.Format("{0} is now online.", c.name);
                    break;

                case DataType.LogOut:
                    foreach (Client client in this.clientList)
                    {
                        if (client.endPoint.Equals(received.SenderName))
                        {
                            clientList.Remove(client);
                        }
                    }

                    sendData.Message = string.Format("{0} is offline", received.SenderName);
                    break;
            }

            data = sendData.toBytes();

            foreach (Client client in this.clientList)
            {
                if (client.endPoint != epSender || sendData.Type != DataType.LogIn)
                {
                    serverSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, client.endPoint, 
                        new AsyncCallback(this.SendData), client.endPoint);

                }
            }

            serverSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None, ref epSender, new AsyncCallback(this.ReceiveData), epSender);
        }


        public void SendData(IAsyncResult asyncResult)
        {
            serverSocket.EndSend(asyncResult);
           
        }
    };
}
