using System;

namespace Rollover
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationManager configurationManager = new ConfigurationManager();
            IRolloverAgent rolloverAgent = new RolloverAgent(configurationManager);
            
            rolloverAgent.Run();
        }
    }
}
