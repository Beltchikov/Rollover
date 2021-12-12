namespace Rollover.Input
{
    public interface IConsoleWrapper
    {
        public string ReadLine();
        public void WriteLine(string message);
    }
}