namespace Rollover.Input
{
    public interface IInputLoop
    {
        void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue);
        bool CheckConnectionMessages(IConsoleWrapper consoleWrapper, IInputQueue inputQueue, int timeout);
    }
}