using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace DistPoClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient clientSocket = new TcpClient();
        private StreamWriter swSender;
        private StreamReader srReceiver;
        private Thread thrMessaging;

        public MainWindow()
        {
            InitializeComponent();
            SetStatus("Welcome");
        }

        private void SetStatus(string message)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(new Action<string>(SetStatus), message);
                return;
            }
            StatusLabel.Text = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IPAddress serverAddress;
            if (!IPAddress.TryParse(IpText.Text, out serverAddress))
            {
                var entry = Dns.GetHostEntry(IpText.Text);
                foreach (var add in entry.AddressList)
                {
                    if (add.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        continue;
                    }
                    if (!add.ToString().StartsWith("192."))
                    {
                        continue;
                    }
                    serverAddress = add;
                    break;
                }
            }

            var port = Int32.Parse(PortText.Text);

            try
            {
                clientSocket.Connect(serverAddress, port);
            }
            catch (Exception ex)
            {
                SetStatus(String.Format("Connection Failed: {0}", ex.Message));
                return;
            }

            SetStatus(String.Format("Connected to: {0}", IpText.Text));

            swSender = new StreamWriter(clientSocket.GetStream());
            swSender.WriteLine(NameText.Text);
            swSender.Flush();

            // Start the thread for receiving messages and further communication
            thrMessaging = new Thread(ReceiveMessages);
            thrMessaging.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            swSender.WriteLine(SendText.Text);
            swSender.Flush();
        }

        private void ReceiveMessages()
        {
            // Receive the response from the server
            srReceiver = new StreamReader(clientSocket.GetStream());
            // If the first character of the response is 1, connection was successful
            var ConResponse = srReceiver.ReadLine();
            // If the first character is a 1, connection was successful
            if (ConResponse[0] == '1')
            {
                // Update the form to tell it we are now connected
                Console.WriteLine("Connected Successfully!");
            }
            else // If the first character is not a 1 (probably a 0), the connection was unsuccessful
            {
                var Reason = "Not Connected: ";
                // Extract the reason out of the response message. The reason starts at the 3rd character
                Reason += ConResponse.Substring(2, ConResponse.Length - 2);
                // Update the form with the reason why we couldn't connect
                Console.WriteLine(Reason);
                // Exit the method
                return;
            }
            // While we are successfully connected, read incoming lines from the server
            while (true)
            {
                // Show the messages in the log TextBox
                Console.WriteLine(srReceiver.ReadLine());
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (clientSocket != null)
            {
                clientSocket.Close();
            }

            if (thrMessaging != null)
            {
                thrMessaging.Abort();
            }
        }
    }
}
