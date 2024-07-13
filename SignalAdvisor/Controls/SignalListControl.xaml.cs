using SignalAdvisor.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SignalAdvisor.Controls
{
    /// <summary>
    /// Interaction logic for SignalListControl.xaml
    /// </summary>
    public partial class SignalListControl : UserControl
    {

        public SignalListControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty InstrumentsProperty =
           DependencyProperty.Register(
               "Instruments",
               typeof(ObservableCollection<Instrument>),
               typeof(SignalListControl),
               new PropertyMetadata(new ObservableCollection<Instrument>()));
        public ObservableCollection<Instrument> Instruments
        {
            get { return (ObservableCollection<Instrument>)GetValue(InstrumentsProperty); }
            set { SetValue(InstrumentsProperty, value); }
        }
    }
}
