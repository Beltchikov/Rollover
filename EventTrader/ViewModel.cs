using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventTrader.Requests;
using System;
using System.CodeDom;
using System.Windows;
using System.Windows.Input;

namespace EventTrader
{
    public class ViewModel: ObservableObject
    {
        private IInfiniteLoop _requestLoop;

        public ICommand StartSessionCommand { get; }
        public ICommand TestDataSourceCommand { get; }
        public ICommand TestConnectionCommand { get; }

        public ViewModel(IInfiniteLoop requestLoop)
        {
            _requestLoop = requestLoop;

            StartSessionCommand = new RelayCommand(() => _requestLoop.Start(() => { }, new object[] {}));
            TestDataSourceCommand = new RelayCommand(() => MessageBox.Show("TestDataSourceCommand"));
            TestConnectionCommand = new RelayCommand(() => MessageBox.Show("TestConnectionCommand"));
        }


    }
}
