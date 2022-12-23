using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SsbHedger2.CommandHandler;
using SsbHedger2.IbHost;
using SsbHedger2.RegistryManager;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SsbHedger2.Model
{
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableCollection<Message> messages;
        private string host = "";
        private int port;
        private int clientId;
        private bool connected;
        
        IRegistryManagerBuilder registryManagerBuilder = null!;
        IIbHostBuilder ibHostBuilder = null!;
        IInitializeCommandHandlerBilder initializeCommandHandlerBilder = null!;

        public MainWindowViewModel()
        {
            registryManagerBuilder = new RegistryManagerBuilder();
            IRegistryManager registryManager = registryManagerBuilder.Build();

            ibHostBuilder = new IbHostBuilder();
            IIbHost ibHost = ibHostBuilder.Build(this, Host, Port, ClientId);

            initializeCommandHandlerBilder = new InitializeCommandHandlerBilder();
            InitializeCommandHandler initializeCommandHandler 
                = initializeCommandHandlerBilder.Build(registryManager, ibHost);

            InitializeCommand = new RelayCommand(()
                => initializeCommandHandler.Handle());

            messages = new ObservableCollection<Message>();
        }

        public string Host
        {
            get => host;
            set
            {
                SetProperty(ref host, value);
                OnPropertyChanged(nameof(ConnectionMessage));
            }
        }

        public int Port
        {
            get => port;
            set
            {
                SetProperty(ref port, value);
                OnPropertyChanged(nameof(ConnectionMessage));
            }
        }

        public int ClientId
        {
            get => clientId;
            set
            {
                SetProperty(ref clientId, value);
                OnPropertyChanged(nameof(ConnectionMessage));
            }
        }

        public string ConnectionMessage
        {
            get
            {
                return connected
                    ? $"CONNECTED! {host}, {port}, client ID: {clientId}"
                    : $"NOT CONNECTED! {host}, {port}, client ID: {clientId}";
            }
        }

        public bool Connected
        {
            get => connected;
            set
            {
                SetProperty(ref connected, value);
                OnPropertyChanged(nameof(ConnectionMessage));
            }
        }

        public ObservableCollection<Message> Messages
        {
            get => messages;
            set => SetProperty(ref messages, value);
        }

        public ICommand InitializeCommand { get; }
    }
}
