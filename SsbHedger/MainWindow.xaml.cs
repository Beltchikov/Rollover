using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IConfiguration _configuration;

        public MainWindow(IConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration;

            var listBoxMessagesSource = (INotifyCollectionChanged)listBoxMessages.Items.SourceCollection;
            listBoxMessagesSource.CollectionChanged += ListBoxMessagesSource_CollectionChanged;
        }

        private void btConfiguration_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow? _configurationWindow = new(_configuration);
            _configurationWindow.Owner= this;   

            bool ? configurationChanged = _configurationWindow.ShowDialog();
            if (configurationChanged == true)
            {
                object[] commandParams = new object[]
                {
                    _configurationWindow.txtHost.Text,
                    Convert.ToInt32(_configurationWindow.txtPort.Text),
                    Convert.ToInt32(_configurationWindow.txtClientId.Text),
                    _configurationWindow.txtUnderlyingSymbol.Text,
                    _configurationWindow.txtSessionStart.Text,
                    _configurationWindow.txtSessionEnd.Text,
                };
                ((MainWindowViewModel)DataContext).UpdateConfigurationCommand.Execute(commandParams);
            }
        }

        private void ListBoxMessagesSource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var border = (Decorator)VisualTreeHelper.GetChild(listBoxMessages, 0);
            var scrollViewer = (ScrollViewer)border.Child;
            scrollViewer.ScrollToEnd();
        }
    }
}
