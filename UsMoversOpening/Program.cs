using System;
using System.Diagnostics.CodeAnalysis;
using UsMoversOpening.Configuration;
using UsMoversOpening.Helper;
using UsMoversOpening.IBApi;
using UsMoversOpening.Threading;

namespace UsMoversOpening
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        static void Main(string[] args)
        {
            IFileHelper fileHelper = new FileHelper();
            ISerializer serializer = new Serializer();
            IConfigurationManager configurationManager = new ConfigurationManager(
                fileHelper, serializer);

            IStocksBuyer stocksBuyer = new StocksBuyer();
            IUmoAgent umoAgent = new UmoAgent(
                configurationManager,
                stocksBuyer);

            IEReaderMonitorSignalWrapper eReaderMonitorSignalWrapper = new EReaderMonitorSignalWrapper();
            IIbClientWrapper ibClientWrapper = new IbClientWrapper(eReaderMonitorSignalWrapper);

            IThreadSpawner threadSpawner 
                = new ThreadSpawner(umoAgent, configurationManager, ibClientWrapper);
            threadSpawner.Run();

            Console.WriteLine("UsMoversOpening - finished!");
        }
    }
}
