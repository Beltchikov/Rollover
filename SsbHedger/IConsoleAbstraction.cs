using System;

namespace SsbHedger
{
    public interface IConsoleAbstraction
    {
        ConsoleKeyInfo ReadKey();
    }
}