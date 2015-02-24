using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace DistPoServer.Core
{
    public class Client
    {
        private TcpClient _clientSocket;
        private Thread thrSender;
        private StreamReader srReceiver;
        private StreamWriter swSender;

        public string Name { get; set; }
        public string LastValue { get; set; }

        public event Action<Client> ClientAddedEvent;
        public event Action<Client> ClientRemovedEvent;
        public event Action<Client, string> MessageReceivedEvent;

        public void Initialize(TcpClient clientSocket)
        {
            _clientSocket = clientSocket;

            thrSender = new Thread(AcceptClient);
            thrSender.Start();
        }

        private void AcceptClient()
        {
            srReceiver = new StreamReader(_clientSocket.GetStream());
            swSender = new StreamWriter(_clientSocket.GetStream());

            // Read the name from the client
            Name = srReceiver.ReadLine();
            OnClientAddedEvent(this);

            try
            {
                string strResponse;
                // Keep waiting for a message from the user
                while ((strResponse = srReceiver.ReadLine()) != null)
                {
                    LastValue = strResponse;
                    OnMessageReceivedEvent(this, strResponse);
                }
            }
            catch (Exception ex)
            {
                OnClientRemovedEvent(this);
                return;
            }
            OnClientRemovedEvent(this);
        }

        private void OnClientAddedEvent(Client client)
        {
            var handler = ClientAddedEvent;
            if (handler != null) handler(client);
        }

        private void OnClientRemovedEvent(Client client)
        {
            var handler = ClientRemovedEvent;
            if (handler != null) handler(client);
        }

        private void OnMessageReceivedEvent(Client client, string text)
        {
            var handler = MessageReceivedEvent;
            if (handler != null) handler(client, text);
        }
    }
}
