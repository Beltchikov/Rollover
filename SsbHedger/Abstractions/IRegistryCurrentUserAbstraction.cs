namespace SsbHedger.Abstractions
{
    public interface IRegistryCurrentUserAbstraction
    {
        IRegistryKeyAbstraction CreateSubKey(string path);
        IRegistryKeyAbstraction? OpenSubKey(string path);
    }
}