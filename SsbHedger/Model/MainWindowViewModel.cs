using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SsbHedger.CommandHandler;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Ink;
using System.Windows.Input;

namespace SsbHedger.Model
{
    [ExcludeFromCodeCoverage]
    public class MainWindowViewModel : ObservableObject
    {
        public const double MULTIPLIER = 100;

        private ObservableCollection<Message> messages;
        private ObservableCollection<Bar> bars;
        private string connectionMessage = "Connecting...";
        private bool connected;
        private string sessionStart;
        private string sessionEnd;
        private int size;
        private int size1;
        private int size2;
        private double strike1;
        private double strike2;
        private double putShortStrike;
        private double putShortPrice;
        private double callShortStrike;
        private double callShortPrice;
        private double spyPrice;
        private double bearHedgeStrike;
        private double bearHedgePrice;
        private double bullHedgeStrike;
        private double bullHedgePrice;
        private string positionsInfoMessage;
        private double spreadWidthBear;
        private double premiumOf3Bear;
        private double maxLossBear;
        private double spreadWidthBull;
        private double premiumOf3Bull;
        private double maxLossBull;


        public MainWindowViewModel(
            IInitializeCommandHandler initializeCommandHandler,
            IUpdateConfigurationCommandHandler updateConfigurationCommandHandler)
        {
            InitializeCommand = new RelayCommand(() => initializeCommandHandler.HandleAsync(this));
            UpdateConfigurationCommand = new RelayCommand<object[]>((p) => updateConfigurationCommandHandler.Handle(this, p));

            messages = new ObservableCollection<Message>();
            bars = new ObservableCollection<Bar>();

            size = 10;
            putShortStrike = 95;
            putShortPrice = 3;
            callShortStrike = 105;
            callShortPrice = 2;

            positionsInfoMessage = "No positions!";
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
                OnPropertyChanged(nameof(PremiumOnePosition));
                OnPropertyChanged(nameof(Premium));
            }
        }

        public int Size1
        {
            get => size1;
            set
            {
                SetProperty(ref size1, value);
                OnPropertyChanged(nameof(Size1));
                //OnPropertyChanged(nameof(PremiumOnePosition));
                //OnPropertyChanged(nameof(Premium));
            }
        }

        public int Size2
        {
            get => size2;
            set
            {
                SetProperty(ref size2, value);
                OnPropertyChanged(nameof(Size2));
                //OnPropertyChanged(nameof(PremiumOnePosition));
                //OnPropertyChanged(nameof(Premium));
            }
        }

        public double Strike1
        {
            get => strike1;
            set
            {
                SetProperty(ref strike1, value);
                OnPropertyChanged(nameof(Strike1));
                //OnPropertyChanged(nameof(PremiumOnePosition));
                //OnPropertyChanged(nameof(Premium));
            }
        }

        public double Strike2
        {
            get => strike2;
            set
            {
                SetProperty(ref strike2, value);
                OnPropertyChanged(nameof(Strike2));
                //OnPropertyChanged(nameof(PremiumOnePosition));
                //OnPropertyChanged(nameof(Premium));
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
                OnPropertyChanged(nameof(PremiumOnePosition));
                OnPropertyChanged(nameof(Premium));
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
                OnPropertyChanged(nameof(PremiumOnePosition));
                OnPropertyChanged(nameof(Premium));
            }
        }

        public double SpyPrice
        {
            get => spyPrice;
            set
            {
                SetProperty(ref spyPrice, value);
                OnPropertyChanged(nameof(SpyPrice));
            }
        }

        public double PremiumOnePosition
        {
            get => Math.Round(putShortPrice + callShortPrice,3);
            set
            {
                OnPropertyChanged(nameof(PremiumOnePosition));
            }
        }

        public double Premium
        {
            get => Math.Round((putShortPrice + callShortPrice) * size * MULTIPLIER, 2);
            set
            {
                OnPropertyChanged(nameof(Premium));
            }
        }

        public double BearHedgeStrike
        {
            get => bearHedgeStrike;
            set
            {
                SetProperty(ref bearHedgeStrike, value);
                OnPropertyChanged(nameof(BearHedgeStrike));
            }
        }

        public double BearHedgePrice
        {
            get => bearHedgePrice;
            set
            {
                SetProperty(ref bearHedgePrice, value);
                OnPropertyChanged(nameof(BearHedgePrice));
            }
        }

        public double BullHedgeStrike
        {
            get => bullHedgeStrike;
            set
            {
                SetProperty(ref bullHedgeStrike, value);
                OnPropertyChanged(nameof(BullHedgeStrike));
            }
        }

        public double BullHedgePrice
        {
            get => bullHedgePrice;
            set
            {
                SetProperty(ref bullHedgePrice, value);
                OnPropertyChanged(nameof(BullHedgePrice));
            }
        }

        public string PositionsInfoMessage
        {
            get => positionsInfoMessage;
            set
            {
                SetProperty(ref positionsInfoMessage, value);
                OnPropertyChanged(nameof(PositionsInfoMessage));
            }
        }

        public double SpreadWidthBear
        {
            get => spreadWidthBear;
            set
            {
                SetProperty(ref spreadWidthBear, value);
                OnPropertyChanged(nameof(SpreadWidthBear));
            }
        }

        public double PremiumOf3Bear
        {
            get => premiumOf3Bear;
            set
            {
                SetProperty(ref premiumOf3Bear, value);
                OnPropertyChanged(nameof(PremiumOf3Bear));
            }
        }

        public double MaxLossBear
        {
            get => maxLossBear;
            set
            {
                SetProperty(ref maxLossBear, value);
                OnPropertyChanged(nameof(MaxLossBear));
            }
        }

        public double SpreadWidthBull
        {
            get => spreadWidthBull;
            set
            {
                SetProperty(ref spreadWidthBull, value);
                OnPropertyChanged(nameof(SpreadWidthBull));
            }
        }

        public double PremiumOf3Bull
        {
            get => premiumOf3Bull;
            set
            {
                SetProperty(ref premiumOf3Bull, value);
                OnPropertyChanged(nameof(PremiumOf3Bull));
            }
        }

        public double MaxLossBull
        {
            get => maxLossBull;
            set
            {
                SetProperty(ref maxLossBull, value);
                OnPropertyChanged(nameof(MaxLossBull));
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
