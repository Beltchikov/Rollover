using System;

namespace SsbHedger
{
    public interface IConsoleWrapper
    {
        ConsoleKeyInfo ReadKey();
    }
}