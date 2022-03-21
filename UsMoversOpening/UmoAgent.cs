using UsMoversOpening.Configuration;

namespace UsMoversOpening
{
    public class UmoAgent : IUmoAgent
    {
        private IConfigurationManager _configurationManager;
        private IStocksBuyer _stocksBuyer;
        private bool _ordersSent;

        public UmoAgent(
            IConfigurationManager configurationManager,
            IStocksBuyer stocksBuyer)
        {
            _configurationManager = configurationManager;
            _stocksBuyer = stocksBuyer;
        }

        public void Run(IThreadSpawner threadSpawner, IThreadWrapper inputThread)
        {
            inputThread.Start();

            // Read configuration
            var configuration = _configurationManager.GetConfiguration();

            // Working loop
            while (!threadSpawner.ExitFlagInputThread)
            {
                if (_stocksBuyer.Triggered(configuration.TimeToBuy) && !_ordersSent)
                {
                    _ordersSent = _stocksBuyer.SendOrders();
                }
                //_consoleWrapper.WriteLine("In loop");
            }
        }
    }
}