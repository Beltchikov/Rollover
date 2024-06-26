﻿using Rollover.Ib;

namespace Rollover.Input
{
    public interface IInputLoop
    {
        void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue, IIbClientQueue ibClientQueue);
    }
}