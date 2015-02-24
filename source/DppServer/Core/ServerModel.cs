using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace DistPoServer.Core
{
    public class ServerModel
    {
        private readonly TcpListener _listener;
        private readonly List<Client> _clientList = new List<Client>();

        public ServerModel()
        {
            _listener = new TcpListener(IPAddress.Any, 1457);
        }

        public void Start()
        {
            _listener.Start();
            _listener.BeginAcceptTcpClient(AcceptClient, null);
        }

        private void AcceptClient(IAsyncResult ar)
        {
            var newClient = _listener.EndAcceptTcpClient(ar);
            // Listen for the next Client
            _listener.BeginAcceptSocket(AcceptClient, null);
            // Initialize the client
            var client = new Client();
            client.ClientAddedEvent += client1 => { _clientList.Add(client1); OnClientAddedEvent(client1); };
            client.ClientRemovedEvent += client1 => { _clientList.Remove(client1); OnClientRemovedEvent(client1); };
            client.Initialize(newClient);
        }

        public event Action<Client> ClientAddedEvent;
        public event Action<Client> ClientRemovedEvent;

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
    }
}
