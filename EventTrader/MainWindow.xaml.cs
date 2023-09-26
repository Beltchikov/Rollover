using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Eomn
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

            var listBoxMessagesSource = (INotifyCollectionChanged)listBoxTwsMessages.Items.SourceCollection;
            listBoxMessagesSource.CollectionChanged += ListBoxMessagesSource_CollectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtResultsYahoo.Text == string.Empty)
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

        private void ListBoxMessagesSource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(listBoxTwsMessages) > 0)
            {
                var border = (Decorator)VisualTreeHelper.GetChild(listBoxTwsMessages, 0);
                var scrollViewer = (ScrollViewer)border.Child;
                scrollViewer.ScrollToEnd();
            }
        }
    }
}
