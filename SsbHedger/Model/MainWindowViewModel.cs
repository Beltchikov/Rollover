using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SsbHedger.CommandHandler;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace SsbHedger.Model
{
    [ExcludeFromCodeCoverage]
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableCollection<Message> messages;
        private string connectionMessage = "Connecting...";
        private bool connected;
        
        public MainWindowViewModel(
            IInitializeCommandHandler initializeCommandHandler,
            IUpdateConfigurationCommandHandler updateConfigurationCommandHandler)
        {
            InitializeCommand = new RelayCommand(() => initializeCommandHandler.HandleAsync(this));
            UpdateConfigurationCommand = new RelayCommand<object[]>((p) => updateConfigurationCommandHandler.Handle(this, p));

            messages = new ObservableCollection<Message>();
        }

        public string ConnectionMessage
        {
            get => connectionMessage;
            set
            {
                SetProperty(ref connectionMessage, value);
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
        public ICommand UpdateConfigurationCommand { get; }
    }
}
