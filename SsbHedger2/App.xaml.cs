using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SsbHedger.Abstractions;
using SsbHedger.Configuration;
using System;
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
                .AddSingleton<IIbHost, IbHost>()
                .AddSingleton<IRegistryManager, RegistryManager>()
                .AddMediatR(GetType().Assembly)
                .BuildServiceProvider();
        }
        public IServiceProvider Services { get; }
    }
}
