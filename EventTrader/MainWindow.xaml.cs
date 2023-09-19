using System;
using System.Windows;
using System.Windows.Controls;

namespace Dsmn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DOT = ".";
        const string COMMA = ",";
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(txtResultsYahoo.Text == string.Empty)
            {
                return;
            }

            var selectedItem = ((ComboBox)sender).SelectedItem;
            var decimalSeparator = ((ComboBoxItem)selectedItem).Content.ToString();
            if (decimalSeparator == DOT)
            {
                txtResultsYahoo.Text = txtResultsYahoo.Text.Replace(COMMA, DOT);
            }
            else if (decimalSeparator == ",")
            {
                txtResultsYahoo.Text = txtResultsYahoo.Text.Replace(DOT, COMMA);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
