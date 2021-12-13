namespace Rollover.Input
{
    public interface IInputLoop
    {
        void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue);
    }
}