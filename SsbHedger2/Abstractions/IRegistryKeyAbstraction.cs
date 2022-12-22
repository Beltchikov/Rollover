using Microsoft.Win32;

namespace SsbHedger2.Abstractions
{
    public interface IRegistryKeyAbstraction
    {
        public object? GetValue(string? name);
        public void SetValue(string? name, object value);
    }
}