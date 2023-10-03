﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Eomn.UserControls
{
    /// <summary>
    /// Interaction logic for TwsConnection.xaml
    /// </summary>
    public partial class TwsConnection : UserControl
    {
        public TwsConnection()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        public static readonly DependencyProperty ConnectedProperty =
           DependencyProperty.Register("Connected", typeof(bool), typeof(TwsConnection), new PropertyMetadata(false));
        public bool Connected
        {
            get { return (bool)GetValue(ConnectedProperty); }
            set { SetValue(ConnectedProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TwsConnection), new PropertyMetadata(null));
        public ICommand? Command
        {
            get { return (ICommand?)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public bool TextFieldsAreEnabled
        {
            get
            {
                return !Connected;
            }
        }

        public string ButtonContent
        {
            get
            {
                return Connected
                    ? "Disconnect"
                    :"Connect to TWS";
            }
        }

        
    }
}
