using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace SsbHedger.Model
{
    public class MainWindowViewModel : ObservableObject
    {
        IMediator? _mediator;
        private ObservableCollection<Message> messages;
        private string host = "";
        private int port;
        private int clientId;
        private bool connected;

        public MainWindowViewModel()
        {
            _mediator = ((App)Application.Current).Services.GetService<IMediator>()
                ?? throw new ApplicationException("Unexpected! mediator is null");
         
            messages = new ObservableCollection<Message>();
        }

        public string Host
        {
            get => host;
            set
            {
                SetProperty(ref host, value);
                OnPropertyChanged(nameof(ConnectionMessage));
            }
        }

        public int Port
        {
            get => port;
            set
            {
                SetProperty(ref port, value);
                OnPropertyChanged(nameof(ConnectionMessage));
            }
        }

        public int ClientId
        {
            get => clientId;
            set
            {
                SetProperty(ref clientId, value);
                OnPropertyChanged(nameof(ConnectionMessage));
            }
        }

        public string ConnectionMessage
        {
            get
            {
                return connected
                    ? $"CONNECTED! {host}, {port}, client ID: {clientId}"
                    : $"NOT CONNECTED! {host}, {port}, client ID: {clientId}";
            }
        }

        public bool Connected
        {
            get => connected;
            set
            {
                SetProperty(ref connected, value);
                OnPropertyChanged(nameof(ConnectionMessage));
            }
        }

        public ObservableCollection<Message> Messages
        {
            get => messages;
            set => SetProperty(ref messages, value);
        }
    }
}
