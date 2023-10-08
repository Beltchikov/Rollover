using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SsbHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new(); 
            mainWindow.Show();
        }

        //private void Application_Startup(object sender, StartupEventArgs e)
        //{
        //    MainWindow mainWindow = new()
        //    {
        //        //DataContext = new ViewModel(
        //        //    Services.GetRequiredService<IInvestingProvider>(),
        //        //    Services.GetRequiredService<IYahooProvider>(),
        //        //    Services.GetRequiredService<ITwsProvider>(),
        //        //    Services.GetRequiredService<IIbHost>())
        //    };
        //    mainWindow.Show();
        //}
    }
}
