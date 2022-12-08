using System.Collections.ObjectModel;

namespace SsbHedger.Model
{
    public class MainWindowViewModel : NotifyPropertyChangedImplementation
    {
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();
        private string host = "";
        private int port;
        private int clientId;
        private string connectionMessage = "";
        private bool connected;

        public string Host
        {
            get => host;
            set
            {
                host = value;
                RaisePropertyChanged();
                RaisePropertyChanged("ConnectionMessage");
            }
        }

        public int Port
        {
            get => port;
            set
            {
                port = value;
                RaisePropertyChanged();
                RaisePropertyChanged("ConnectionMessage");
            }
        }

        public int ClientId
        {
            get => clientId;
            set
            {
                clientId = value;
                RaisePropertyChanged();
                RaisePropertyChanged("ConnectionMessage");
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
                connected = value;
                RaisePropertyChanged();
                RaisePropertyChanged("ConnectionMessage");
            }
        }

        public ObservableCollection<Message> Messages
        {
            get { return messages; }

            set
            {
                if (messages != value)
                {
                    messages = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
