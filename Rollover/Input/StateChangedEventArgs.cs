namespace Rollover.Input
{
    public class StateChangedEventArgs
    {
        public StateChangedEventArgs(string state, string stateBefore)
        {
            State = state;
            StateBefore = stateBefore;
        }
        public string State { get; }
        public string StateBefore { get; }
    }
}