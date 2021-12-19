namespace Rollover.Input
{
    public interface IReducer
    {
        string GetState(string stateBefore, string input);
    }
}