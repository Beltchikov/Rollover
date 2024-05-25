using SignalAdvisor.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SignalAdvisor
{
    /// <summary>
    /// Interaction logic for InstrumentsControl.xaml
    /// </summary>
    public partial class InstrumentsControl : UserControl
    {

        public InstrumentsControl()
        {
            InitializeComponent();

            Instruments.Add(new Instrument() { Symbol = "NVDA" });
            Instruments.Add(new Instrument() { Symbol = "MSFT" });

            DataContext = this;
        }

        public ObservableCollection<Instrument> Instruments
        {
            get { return (ObservableCollection<Instrument>)GetValue(InstrumentsProperty); }
            set { SetValue(InstrumentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Instruments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstrumentsProperty =
            DependencyProperty.Register(
                "Instruments",
                typeof(ObservableCollection<Instrument>),
                typeof(InstrumentsControl),
                new PropertyMetadata(new ObservableCollection<Instrument>()));
    }
}
