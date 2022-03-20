using UsMoversOpening.Configuration;

namespace UsMoversOpening
{
    public class UmoAgent : IUmoAgent
    {
        private IConfigurationManager _configurationManager;

        public UmoAgent(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public void Run()
        {
            var configuration = _configurationManager.GetConfiguration();
        }
    }
}