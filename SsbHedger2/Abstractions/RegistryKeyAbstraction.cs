using Microsoft.Win32;

namespace SsbHedger2.Abstractions
{
    public class RegistryKeyAbstraction : IRegistryKeyAbstraction
    {
        private RegistryKey _registryKey = null!;

        private RegistryKeyAbstraction(){}

        public RegistryKeyAbstraction(RegistryKey registryKey) 
        {
            _registryKey = registryKey;
        }

        public object? GetValue(string? name)
        {
            return _registryKey.GetValue(name);
        }

        public void SetValue(string? name, object value)
        {
            _registryKey.SetValue(name, value);
        }
    }
}
