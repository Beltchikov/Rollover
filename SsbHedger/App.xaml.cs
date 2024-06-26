﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SsbHedger.Abstractions;
using SsbHedger.CommandHandler;
using SsbHedger.IbModel;
using SsbHedger.Model;
using SsbHedger.SsbChartControl;
using SsbHedger.SsbConfiguration;
using SsbHedger.Utilities;
using System;
using System.Globalization;
using System.Windows;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = new ServiceCollection()
                .AddSingleton<IRegistryCurrentUserAbstraction, RegistryCurrentUserAbstraction>()
                .AddScoped<IRegistryKeyAbstraction, RegistryKeyAbstraction>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<IRegistryManager, RegistryManager>()
                .AddSingleton<IIbHost, IbHost>()
                .AddSingleton<IInitializeCommandHandler, InitializeCommandHandler>()
                .AddSingleton<IUpdateConfigurationCommandHandler, UpdateConfigurationCommandHandler>()
                .AddSingleton<IDeltaAlertActivateCommandHandler, DeltaAlertActivateCommandHandler>()
                .AddSingleton<IVolatilityAlertActivateCommandHandler, VolatilityAlertActivateCommandHandler>()
                .AddSingleton<IFindStrikesCommandHandler, FindStrikesCommandHandler>()
                .AddSingleton<IUpdateReqMktDataAtmStrikePutCommandHandler, UpdateReqMktDataAtmStrikePutCommandHandler>()
                .AddSingleton<IUpdateReqMktDataAtmStrikeCallCommandHandler, UpdateReqMktDataAtmStrikeCallCommandHandler>()
                .AddSingleton<IPositionMessageBuffer, PositionMessageBuffer>()
                .AddSingleton<IStrikeUtility, StrikeUtility>()
                .AddSingleton<ILastTradeDateConverter, LastTradeDateConverter>()
                .AddSingleton<IContractSpy, ContractSpy>()
                .AddMediatR(GetType().Assembly)
                .BuildServiceProvider();
        }
        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var viewModel = new MainWindowViewModel(
                    Services.GetRequiredService<IInitializeCommandHandler>(),
                    Services.GetRequiredService<IUpdateConfigurationCommandHandler>(),
                    Services.GetRequiredService<IDeltaAlertActivateCommandHandler>(),
                    Services.GetRequiredService<IVolatilityAlertActivateCommandHandler>(),
                    Services.GetRequiredService<IFindStrikesCommandHandler>(),
                    Services.GetRequiredService<IUpdateReqMktDataAtmStrikePutCommandHandler>(),
                    Services.GetRequiredService<IUpdateReqMktDataAtmStrikeCallCommandHandler>());

            var _registryManager = Services.GetRequiredService<IRegistryManager>();
            var _configuration = Services.GetRequiredService<IConfiguration>();
            var configurationdata = _registryManager.ReadConfiguration(new ConfigurationData(
                (string)_configuration.GetValue(Configuration.HOST),
                (int)_configuration.GetValue(Configuration.PORT),
                (int)_configuration.GetValue(Configuration.CLIENT_ID),
                (string)_configuration.GetValue(Configuration.UNDERLYING_SYMBOL),
                (string)_configuration.GetValue(Configuration.SESSION_START),
                (string)_configuration.GetValue(Configuration.SESSION_END),
                (int)_configuration.GetValue(Configuration.DTE),
                (int)_configuration.GetValue(Configuration.NUMBER_OF_STRIKES),
                (string)_configuration.GetValue(Configuration.STRIKE_STEP)));

            _configuration.SetValue(Configuration.HOST, configurationdata.Host);
            _configuration.SetValue(Configuration.PORT, configurationdata.Port);
            _configuration.SetValue(Configuration.CLIENT_ID, configurationdata.ClientId);
            _configuration.SetValue(Configuration.UNDERLYING_SYMBOL, configurationdata.UnderlyingSymbol);
            _configuration.SetValue(Configuration.SESSION_START, configurationdata.SessionStart);
            _configuration.SetValue(Configuration.SESSION_END, configurationdata.SessionEnd);
            _configuration.SetValue(Configuration.DTE, configurationdata.Dte);
            _configuration.SetValue(Configuration.NUMBER_OF_STRIKES, configurationdata.NumberOfStrikes);
            _configuration.SetValue(Configuration.STRIKE_STEP, configurationdata.StrikeStep);

            viewModel.SessionStart= configurationdata.SessionStart; 
            viewModel.SessionEnd= configurationdata.SessionEnd; 

            MainWindow mainWindow = new(_configuration)
            {
                DataContext = viewModel
            };
            mainWindow.Show();
        }
    }
}
