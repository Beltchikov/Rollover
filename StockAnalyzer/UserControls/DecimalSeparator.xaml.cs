using System;
using System.Windows.Controls;

namespace StockAnalyzer.UserControls
{
    /// <summary>
    /// Interaction logic for DecimalSeparator.xaml
    /// </summary>
    public partial class DecimalSeparator : UserControl
    {
        public DecimalSeparator()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public int SelectedIndex { get; set; }

        public event Action<object, SelectionChangedEventArgs> SelectionChanged = null!;    

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);       
        }
    }
}
