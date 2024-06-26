﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TwsUserControls
{
    /// <summary>
    /// Interaction logic for TwsInformation.xaml
    /// </summary>
    public partial class TwsInformation : UserControl
    {
        public TwsInformation()
        {
            InitializeComponent();


            var listBoxMessagesSource = (INotifyCollectionChanged)listBoxMessages.Items.SourceCollection;
            listBoxMessagesSource.CollectionChanged += ListBoxMessagesSource_CollectionChanged;

        }

        public static readonly DependencyProperty MessageCollectionProperty =
            DependencyProperty.Register("MessageCollection", typeof(ObservableCollection<string>), typeof(TwsInformation), new PropertyMetadata(new ObservableCollection<string>()));
        public ObservableCollection<string> MessageCollection
        {
            get { return (ObservableCollection<string>)GetValue(MessageCollectionProperty); }
            set { SetValue(MessageCollectionProperty, value); }
        }

        private void ListBoxMessagesSource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(listBoxMessages) > 0)
            {
                var border = (Decorator)VisualTreeHelper.GetChild(listBoxMessages, 0);
                var scrollViewer = (ScrollViewer)border.Child;
                scrollViewer.ScrollToEnd();
            }
        }
    }
}
