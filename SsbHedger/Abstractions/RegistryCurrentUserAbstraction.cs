using Microsoft.Win32;

namespace SsbHedger.Abstractions
{
    public class RegistryCurrentUserAbstraction : IRegistryCurrentUserAbstraction
    {
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
