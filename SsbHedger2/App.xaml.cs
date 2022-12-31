using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SsbHedger2.Abstractions;
using SsbHedger2.Configuration;
using System;
using System.Windows;

namespace SsbHedger2
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
