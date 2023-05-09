using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SsbHedger.CommandHandler;
using SsbHedger.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace SsbHedger.Model
{
    [ExcludeFromCodeCoverage]
    public class MainWindowViewModel : ObservableObject
    {
        public const double MULTIPLIER = 100;
        public const double STRIKES_STEP = 0.5;
        public const int STRIKES_COUNT = 20;

        private ObservableCollection<Message> messages;
        private ObservableCollection<Bar> bars;
        private ObservableCollection<double> strikes;
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
        private double underlyingPrice;
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
        private string right0;
        private string right1;
        private string right2;
        private string right3;
        private string right4;
        private string right5;
        private string right6;
        private string right7;
        private string right8;
        private string right9;
        private double nextPutStrike;
        private double nextCallStrike;
        private double nextPutDelta;
        private double nextCallDelta;
        private bool deltaAlertActive;
        private double deltaThreshold;
        private bool volatilityAlertActive;
        private double atmStrikeUp;
        private double atmStrikeDown;
        

        public MainWindowViewModel(
            IInitializeCommandHandler initializeCommandHandler,
            IUpdateConfigurationCommandHandler updateConfigurationCommandHandler,
            IDeltaAlertActivateCommandHandler deltaAlertActivateCommandHandler,
            IVolatilityAlertActivateCommandHandler volatilityAlertActivateCommandHandler,
            IFindStrikesCommandHandler findAtmStrikesCommandHandler)
        {
            InitializeCommand = new RelayCommand(() => initializeCommandHandler.HandleAsync(this));
            UpdateConfigurationCommand = new RelayCommand<object[]>((p) => updateConfigurationCommandHandler.Handle(this, p));
            DeltaAlertActivateCommand = new RelayCommand<object[]>((p) => deltaAlertActivateCommandHandler.Handle(this, p));
            VolatilityAlertActivateCommand = new RelayCommand<object[]>((p) => volatilityAlertActivateCommandHandler.Handle(this, p));
            FindStrikesCommand = new RelayCommand<object[]>((p) => findAtmStrikesCommandHandler.Handle(this, p));

            messages = new ObservableCollection<Message>();
            bars = new ObservableCollection<Bar>();
            strikes = new ObservableCollection<double>();

            size = 10;
            putShortStrike = 95;
            putShortPrice = 3;
            callShortStrike = 105;
            callShortPrice = 2;

            positionsInfoMessage = "No positions!";
            deltaThreshold = 16;
            nextPutStrike = 0;
            nextCallStrike = 0;
            nextPutDelta = -50;
            nextCallDelta = 50;
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

        public double UnderlyingPrice
        {
            get => underlyingPrice;
            set
            {
                SetProperty(ref underlyingPrice, value);
                OnPropertyChanged(nameof(UnderlyingPrice));
                OnPropertyChanged(nameof(NextAtmStrike));
                
                FindStrikesCommand.Execute(new object[] { value });
            }
        }

        public double PremiumOnePosition
        {
            get => Math.Round(putShortPrice + callShortPrice, 3);
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

        public string Right0
        {
            get => right0;
            set
            {
                SetProperty(ref right0, value);
                OnPropertyChanged(nameof(Right0));
            }
        }

        public string Right1
        {
            get => right1;
            set
            {
                SetProperty(ref right1, value);
                OnPropertyChanged(nameof(Right1));
            }
        }

        public string Right2
        {
            get => right2;
            set
            {
                SetProperty(ref right2, value);
                OnPropertyChanged(nameof(Right2));
            }
        }

        public string Right3
        {
            get => right3;
            set
            {
                SetProperty(ref right3, value);
                OnPropertyChanged(nameof(Right3));
            }
        }

        public string Right4
        {
            get => right4;
            set
            {
                SetProperty(ref right4, value);
                OnPropertyChanged(nameof(Right4));
            }
        }

        public string Right5
        {
            get => right5;
            set
            {
                SetProperty(ref right5, value);
                OnPropertyChanged(nameof(Right5));
            }
        }

        public string Right6
        {
            get => right6;
            set
            {
                SetProperty(ref right6, value);
                OnPropertyChanged(nameof(Right6));
            }
        }

        public string Right7
        {
            get => right7;
            set
            {
                SetProperty(ref right7, value);
                OnPropertyChanged(nameof(Right7));
            }
        }

        public string Right8
        {
            get => right8;
            set
            {
                SetProperty(ref right8, value);
                OnPropertyChanged(nameof(Right8));
            }
        }

        public string Right9
        {
            get => right9;
            set
            {
                SetProperty(ref right9, value);
                OnPropertyChanged(nameof(Right9));
            }
        }

        public double NextPutStrike
        {
            get => nextPutStrike;
            set
            {
                SetProperty(ref nextPutStrike, value);
                OnPropertyChanged(nameof(NextPutStrike));
            }
        }

        public double NextCallStrike
        {
            get => nextCallStrike;
            set
            {
                SetProperty(ref nextCallStrike, value);
                OnPropertyChanged(nameof(NextCallStrike));
            }
        }

        public double NextPutDelta
        {
            get => nextPutDelta;
            set
            {
                SetProperty(ref nextPutDelta, value);
                OnPropertyChanged(nameof(NextPutDelta));
            }
        }

        public double NextCallDelta
        {
            get => nextCallDelta;
            set
            {
                SetProperty(ref nextCallDelta, value);
                OnPropertyChanged(nameof(NextCallDelta));
            }
        }

        public bool DeltaAlertActive
        {
            get => deltaAlertActive;
            set
            {
                SetProperty(ref deltaAlertActive, value);
                OnPropertyChanged(nameof(DeltaAlertActive));
            }
        }

        public double DeltaThreshold
        {
            get => deltaThreshold;
            set
            {
                SetProperty(ref deltaThreshold, value);
                OnPropertyChanged(nameof(DeltaThreshold));
            }
        }

        public bool VolatilityAlertActive
        {
            get => volatilityAlertActive;
            set
            {
                SetProperty(ref volatilityAlertActive, value);
                OnPropertyChanged(nameof(VolatilityAlertActive));
            }
        }

        public double AtmStrikeUp
        {
            get => atmStrikeUp;
            set
            {
                SetProperty(ref atmStrikeUp, value);
                OnPropertyChanged(nameof(AtmStrikeUp));
            }
        }

        public double AtmStrikeDown
        {
            get => atmStrikeDown;
            set
            {
                SetProperty(ref atmStrikeDown, value);
                OnPropertyChanged(nameof(AtmStrikeDown));
            }
        }

        public ObservableCollection<Bar> Bars
        {
            get => bars;
            set => SetProperty(ref bars, value);
        }

        public ObservableCollection<double> Strikes
        {
            get => strikes;
            set => SetProperty(ref strikes, value);
        }

        public double NextAtmStrike {get; set;}
        public double SecondAtmStrike {get; set;}

        public ICommand InitializeCommand { get; }
        public ICommand UpdateConfigurationCommand { get; }
        public ICommand DeltaAlertActivateCommand { get; }
        public ICommand VolatilityAlertActivateCommand { get; }
        public ICommand FindStrikesCommand { get; }
    }
}
