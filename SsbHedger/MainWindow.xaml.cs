﻿using IbClient;
using SsbHedger.Abstractions;
using SsbHedger.Model;
using SsbHedger.ResponseProcessing;
using SsbHedger.ResponseProcessing.Mapper;
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
            var dispatcher = new DispatcherAbstraction(Dispatcher);

            _logic = new Logic(
                IBClient.CreateClient(),
                new ResponseLoop() { BreakCondition = () => 1 == 0 },
                new ResponseHandler(readerQueue),
                new ResponseMapper(),
                new ResponseProcessor(dispatcher),
                new BackgroundWorkerAbstraction());
            _logic.Execute();

            _logic.Error += _logic_Error;
        }

        private void _logic_Error(int reqId, string message)
        {
            ((ViewModelMain)DataContext).Messages.Add(new Message { ReqId=reqId, Body= message });
        }
    }
}
