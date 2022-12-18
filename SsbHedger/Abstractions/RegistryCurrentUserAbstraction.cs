using Microsoft.Win32;

namespace SsbHedger.Abstractions
{
    public class RegistryCurrentUserAbstraction : IRegistryCurrentUserAbstraction
    {
        public IRegistryKeyAbstraction CreateSubKey(string path)
        {
            var subKey = Registry.CurrentUser.CreateSubKey(path);
            return new RegistryKeyAbstraction(subKey);
        }

        public IRegistryKeyAbstraction? OpenSubKey(string path)
        {
            var subKey = Registry.CurrentUser.OpenSubKey(path);
            if(subKey == null)
            { 
                return null; 
            }

            return new RegistryKeyAbstraction(subKey);
        }
    }
}
