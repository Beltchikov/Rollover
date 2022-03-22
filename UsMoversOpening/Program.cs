﻿using System;
using System.Diagnostics.CodeAnalysis;
using UsMoversOpening.Configuration;
using UsMoversOpening.Helper;

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
            //IConsoleWrapper consoleWrapper = new ConsoleWrapper();

            IUmoAgent umoAgent = new UmoAgent(
                configurationManager,
                stocksBuyer);

            //umoAgent.Run();

            IThreadSpawner threadSpawner = new ThreadSpawner(umoAgent, configurationManager);
            threadSpawner.Run();

            Console.WriteLine("UsMoversOpening - finished!");
        }
    }
}
