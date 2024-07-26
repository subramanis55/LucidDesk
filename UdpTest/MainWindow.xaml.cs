using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace UdpTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        Manager.Client client1;
        Manager.Client client2;
        public MainWindow()
        {
           
            InitializeComponent();
            Manager.Server server = new Manager.Server();
            server.Initialize();
            server.StartMessageLoop();
            server.MessageServerInvoke += ServerMessageServerInvoke;
          client1 = new Manager.Client();
            client1.MessageClientInvoke += Client1_MessageclientInvoke;
            client1.Initialize(IPAddress.Parse("192.168.3.50"), 10000);
            client1.StartMessageLoop();
            client2 = new Manager.Client();
            client2.Initialize(IPAddress.Parse("192.168.3.50"), 10000);
            client2.StartMessageLoop();
        }

        private void Client1_MessageclientInvoke(object sender, string e)
        {
            Dispatcher.Invoke(() =>
            {
                outputTextBox.AppendText(">>" + e + "\n");
            });
        }

        private void ServerMessageServerInvoke(object sender, string e)
        {
            Dispatcher.Invoke(()=>
            {  
              outputTextBox.AppendText(">>"+e+"\n");
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(ClientTextBox.Document.ContentStart, ClientTextBox.Document.ContentEnd);
            client1.Send(Encoding.UTF8.GetBytes(textRange.Text+"Client1"));
            client2.Send(Encoding.UTF8.GetBytes(textRange.Text + "Client2"));
            ClientTextBox.Document.Blocks.Clear();
        }
    }
}
