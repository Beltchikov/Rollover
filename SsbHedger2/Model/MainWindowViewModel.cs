﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SsbHedger2.CommandHandler;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace SsbHedger2.Model
{
    [ExcludeFromCodeCoverage]
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableCollection<Message> messages;
        private string host = "";
        private int port;
        private int clientId;
        private bool connected;
        
        public MainWindowViewModel()
        {
            var initializeCommandHandlerBilder = new InitializeCommandHandlerBilder();
            InitializeCommandHandler initializeCommandHandler = initializeCommandHandlerBilder.Build(this);
            InitializeCommand = new RelayCommand(() => initializeCommandHandler.Handle());

            var updateConfigurationCommandHandlerBilder = new UpdateConfigurationCommandHandlerBuilder();
            UpdateConfigurationCommandHandler updateConfigurationCommandHandler = updateConfigurationCommandHandlerBilder.Build(this); 
            UpdateConfigurationCommand = new RelayCommand<object[]>((p) => updateConfigurationCommandHandler.Handle(p));

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

        public ICommand InitializeCommand { get; }
        public ICommand UpdateConfigurationCommand { get; }
    }
}
