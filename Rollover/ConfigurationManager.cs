namespace Rollover
{
    public class ConfigurationManager : IConfigurationManager
    {
        private IFileHelper _fileHelper;

        public ConfigurationManager(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public void CheckConfiguration()
        {
                
            
            //throw new NotImplementedException();
        }
    }
}
