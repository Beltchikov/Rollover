namespace Rollover
{
    public class ConfigurationManager : IConfigurationManager
    {
        private IFileHelper _fileHelper;
        private ISerializer _serializer;
        public const string CONFIGURATION_FILE = "Configuration.json";

        public ConfigurationManager(IFileHelper fileHelper, ISerializer serializer)
        {
            _fileHelper = fileHelper;
            _serializer = serializer;
        }

        public void CheckConfiguration()
        {
            var configurationAsText = _fileHelper.ReadAllText(CONFIGURATION_FILE);
            var configuration = _serializer.Deserialize<Configuration>(configurationAsText);

            //throw new NotImplementedException();
        }
    }
}
