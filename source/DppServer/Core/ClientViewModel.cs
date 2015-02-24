namespace DistPoServer.Core
{
    public class ClientViewModel : ObservableObject
    {
        public string Name
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }
        public string Value
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }
        public bool ReceivedData
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        private readonly Client _client;
        private string _internalValue;

        public ClientViewModel(Client client)
        {
            _client = client;
            Name = _client.Name;
            _client.MessageReceivedEvent += (client1, text) => { ReceivedData = true; _internalValue = text; };
        }

        public void Reveil()
        {
            Value = _internalValue;
        }
    }
}
