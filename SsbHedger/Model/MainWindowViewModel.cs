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
        private ObservableCollection<Bar> bars;
        private string connectionMessage = "Connecting...";
        private bool connected;
        private string sessionStart;
        private string sessionEnd;
        private int size;
        private double putShortStrike;
        private double putShortPrice;
        private double callShortStrike;
        private double callShortPrice;

        public MainWindowViewModel(
            IInitializeCommandHandler initializeCommandHandler,
            IUpdateConfigurationCommandHandler updateConfigurationCommandHandler)
        {
            InitializeCommand = new RelayCommand(() => initializeCommandHandler.HandleAsync(this));
            UpdateConfigurationCommand = new RelayCommand<object[]>((p) => updateConfigurationCommandHandler.Handle(this, p));

            messages = new ObservableCollection<Message>();
            bars = new ObservableCollection<Bar>();
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

        public string SessionStart
        {
            get => sessionStart;
            set
            {
                SetProperty(ref sessionStart, value);
                OnPropertyChanged(nameof(SessionStart));
            }
        }

        public string SessionEnd
        {
            get => sessionEnd;
            set
            {
                SetProperty(ref sessionEnd, value);
                OnPropertyChanged(nameof(SessionEnd));
            }
        }

        public int Size
        {
            get => size;
            set
            {
                SetProperty(ref size, value);
                OnPropertyChanged(nameof(Size));
            }
        }

        public double PutShortStrike
        {
            get => putShortStrike;
            set
            {
                SetProperty(ref putShortStrike, value);
                OnPropertyChanged(nameof(PutShortStrike));
            }
        }

        public double PutShortPrice
        {
            get => putShortPrice;
            set
            {
                SetProperty(ref putShortPrice, value);
                OnPropertyChanged(nameof(PutShortPrice));
            }
        }

        public double CallShortStrike
        {
            get => callShortStrike;
            set
            {
                SetProperty(ref callShortStrike, value);
                OnPropertyChanged(nameof(CallShortStrike));
            }
        }

        public double CallShortPrice
        {
            get => callShortPrice;
            set
            {
                SetProperty(ref callShortPrice, value);
                OnPropertyChanged(nameof(CallShortPrice));
            }
        }

        public ObservableCollection<Bar> Bars
        {
            get => bars;
            set => SetProperty(ref bars, value);
        }

        public ICommand InitializeCommand { get; }
        public ICommand UpdateConfigurationCommand { get; }
    }
}
