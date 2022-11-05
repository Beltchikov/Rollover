using System;

namespace SsbHedger.Abstractions
{
    public interface IConsoleAbstraction
    {
        ConsoleKeyInfo ReadKey();
    }
}