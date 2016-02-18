using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatClient;
using ChatServer;

namespace ChatRoom
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int PORT = 30000;
        ChatClient.ChatClient client;
        ChatServer.ChatServer server;
        List<string> messages;
        
        public MainWindow()
        {
            InitializeComponent();
            messages = new List<string>();
            ChatRoomMessages.ItemsSource = messages;

        }



        private void ChatRoomConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = ChatRoomUserName.Text;
            string serverIP = ChatRoomServerIP.Text;
            try {
                this.client = new ChatClient.ChatClient(serverIP, PORT, name);
                ChatRoomConnectBtn.IsEnabled = false;
                ChatRoomServerIP.IsEnabled = false;
                ChatRoomUserName.IsEnabled = false;
                ChatRoomMessageBox.IsEnabled = true;
                ChatRoomMessageSend.IsEnabled = true;
                ChatRoomDisconnectBtn.IsEnabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error + \n" + exception.ToString());
            }



        }

        private void ChatRoomMessageSend_Click(object sender, RoutedEventArgs e)
        {
            string message = ChatRoomMessageBox.Text;
            client.sendMessage(message, ChatNetwork.DataType.Message);
            ChatRoomMessageBox.Text = null;
        }

        private void ChatRoomDisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            client.sendMessage(null, ChatNetwork.DataType.LogOut);
            client = new ChatClient.ChatClient();
            ChatRoomConnectBtn.IsEnabled = true;
            ChatRoomServerIP.IsEnabled = true;
            ChatRoomUserName.IsEnabled = true;
            ChatRoomMessageBox.IsEnabled = false;
            ChatRoomMessageSend.IsEnabled = false;
            ChatRoomDisconnectBtn.IsEnabled = false;
        }

        private void onMessageReceive(ChatClient.ChatMessage msg)
        {
            this.messages.Add(msg.ToString());
            ChatRoomMessages.ItemsSource = null;
            ChatRoomMessages.ItemsSource = messages;
        }

        private void ChatRoomCreateServer_Click(object sender, RoutedEventArgs e)
        {
            server = new ChatServer.ChatServer();
        }
    }
}
