using Microsoft.Win32;
using System;

namespace SsbHedger.Abstractions
{
    public interface IRegistryCurrentUserAbstraction
    {
        IRegistryKeyAbstraction? OpenSubKey(string path);
    }
}