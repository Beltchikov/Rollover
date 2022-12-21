﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using SsbHedger.MediatorCommands;
using System.Linq;

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
            UpdateConfigurationCommand = new RelayCommand<string>(async (data) =>
            {
                if(data == null) { throw new ApplicationException("Unexpected! data is null"); }
                var dataArray = data.Split(";").Select(m => m.Trim()).ToList();
                await _mediator.Publish(new UpdateConfigurationMediatorCommand(
                    dataArray[0], 
                    Convert.ToInt32(dataArray[1]),
                    Convert.ToInt32(dataArray[2])));
            }); 

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

        public ICommand UpdateConfigurationCommand { get; }
    }
}
