using SignalAdvisor.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SignalAdvisor.Controls
{
    /// <summary>
    /// Interaction logic for InstrumentsControl.xaml
    /// </summary>
    public partial class InstrumentsControl : UserControl
    {

        public InstrumentsControl()
        {
            InitializeComponent();

            TradeAction = null!;
        }

        public static readonly DependencyProperty InstrumentsProperty =
           DependencyProperty.Register(
               "Instruments",
               typeof(ObservableCollection<Instrument>),
               typeof(InstrumentsControl),
               new PropertyMetadata(new ObservableCollection<Instrument>()));
        public ObservableCollection<Instrument> Instruments
        {
            get { return (ObservableCollection<Instrument>)GetValue(InstrumentsProperty); }
            set { SetValue(InstrumentsProperty, value); }
        }

        public event EventHandler<TradeActionEventArgs> TradeAction;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var instrument = ((Button)sender).Tag as Instrument ?? throw new Exception();

            if (TradeAction != null)
                TradeAction(this, new TradeActionEventArgs(instrument));
        }
    }
}
