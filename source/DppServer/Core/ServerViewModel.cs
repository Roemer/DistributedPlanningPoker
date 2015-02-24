using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace DistPoServer.Core
{
    public class ServerViewModel : ObservableObject
    {
        private readonly ServerModel _model;
        private readonly Dispatcher _dispatcher;

        public ICommand StartCommand { get; private set; }
        public ICommand NewRoundCommand { get; private set; }
        public ICommand RevealCommand { get; private set; }
        public BindingList<ClientViewModel> Clients { get; set; }
        public bool Stretch
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        public ServerViewModel(ServerModel model, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            Clients = new BindingList<ClientViewModel>();

            // Initialize model
            _model = model;
            _model.ClientAddedEvent += AddClient;
            _model.ClientRemovedEvent += RemoveClient;
            // Initialize commands
            StartCommand = new RelayCommand(x => _model.Start());
            NewRoundCommand = new RelayCommand(x =>
            {
                foreach (var client in Clients)
                {
                    client.Value = "?";
                    client.ReceivedData = false;
                }
            });
            RevealCommand = new RelayCommand(x =>
            {
                foreach (var client in Clients)
                {
                    client.Reveil();
                }
            });
        }

        private void AddClient(Client client)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.InvokeAsync(() => AddClient(client));
                return;
            }
            Clients.Add(new ClientViewModel(client));
        }

        private void RemoveClient(Client client)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.InvokeAsync(() => RemoveClient(client));
                return;
            }
            var itemToRemove = Clients.Where(x => x.Name == client.Name).ToList();
            foreach (var item in itemToRemove) { Clients.Remove(item); }
        }
    }
}
