namespace Rollover
{
    public class RolloverAgent : IRolloverAgent
    {
        private IConfigurationManager _configurationManager;

        public RolloverAgent(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }


        public void Run()
        {
            throw new System.NotImplementedException();
        }
    }
}