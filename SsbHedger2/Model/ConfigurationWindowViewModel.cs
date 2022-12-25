using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SsbHedger2.MediatorCommands;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SsbHedger2.Model
{
    public class ConfigurationWindowViewModel : ObservableObject
    {
        IMediator? _mediator;
        private string host = "";
        private int port;
        private int clientId;
       
        public ConfigurationWindowViewModel()
        {
            _mediator = ((App)Application.Current).Services.GetService<IMediator>()
                ?? throw new ApplicationException("Unexpected! mediator is null");
         
            UpdateConfigurationCommand = new RelayCommand<string>(async (data) =>
            {
                if(data == null) { throw new ApplicationException("Unexpected! data is null"); }
                var dataArray = data.Split(";").Select(m => m.Trim()).ToList();
                Host = dataArray[0];
                Port = Convert.ToInt32(dataArray[1]);
                ClientId= Convert.ToInt32(dataArray[2]);
                if (CloseAction == null) { throw new ApplicationException("Unexpected! CloseAction is null"); }
                await _mediator.Publish(new UpdateConfigurationMediatorCommand(
                    Host, Port, ClientId, CloseAction));
            }); 

        }

        public string Host
        {
            get => host;
            set
            {
                SetProperty(ref host, value);
            }
        }

        public int Port
        {
            get => port;
            set
            {
                SetProperty(ref port, value);
            }
        }

        public int ClientId
        {
            get => clientId;
            set
            {
                SetProperty(ref clientId, value);
            }
        }

        public ICommand UpdateConfigurationCommand { get; }

        public Action? CloseAction { get; set; }
    }
}
