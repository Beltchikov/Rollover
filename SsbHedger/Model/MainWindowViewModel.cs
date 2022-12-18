using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace SsbHedger.Model
{
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableCollection<Message> messages;
        private string host = "";
        private int port;
        private int clientId;
        private bool connected;

        public MainWindowViewModel()
        {
            messages = new ObservableCollection<Message>();
        }

        public string Host
        {
            get => host;
            set => SetProperty(ref host, value);
        }

        public int Port
        {
            get => port;
            set => SetProperty(ref port, value);
        }

        public int ClientId
        {
            get => clientId;
            set => SetProperty(ref clientId, value);
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
            set => SetProperty(ref connected, value);
        }

        public ObservableCollection<Message> Messages
        {
            get => messages;
            set => SetProperty(ref messages, value);
        }
    }
}
