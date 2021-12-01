namespace Rollover
{
    public class ConfigurationManager : IConfigurationManager
    {
        private IFileHelper _fileHelper;
        private const string CONFIGURATION_FILE = "Configuration.json";

        public ConfigurationManager(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public void CheckConfiguration()
        {
            var configurationAsText = _fileHelper.ReadAllText(CONFIGURATION_FILE);
            
            //throw new NotImplementedException();
        }
    }
}
