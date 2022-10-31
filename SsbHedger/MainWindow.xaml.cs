﻿using IbClient;
using SsbHedger.ResponseProcessing;
using System;
using System.Collections.Concurrent;
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

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ILogic _logic;
        
        public MainWindow()
        {
            InitializeComponent();


            var readerQueue = new ReaderThreadQueue();
            _logic = new Logic(
                IBClient.CreateClient(),
                new ConsoleWrapper(),
                new ResponseLoop(),
                new ResponseHandler(readerQueue));
            _logic.Execute();
        }
    }
}
