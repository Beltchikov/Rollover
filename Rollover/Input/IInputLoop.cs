using Rollover.Ib;

namespace Rollover.Input
{
    public interface IInputLoop
    {
        void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue, IRequestSender requestSender);
        bool CheckConnectionMessages(IConsoleWrapper consoleWrapper, IInputQueue inputQueue, int timeout);
    }
}