namespace Rollover.Input
{
    public interface IReducer
    {
        string GetState(string stateBefore, string input);

        public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

        public event StateChangedHandler StateChanged;
    }
}