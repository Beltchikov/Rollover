namespace Rollover.Input
{
    public class Reducer : IReducer
    {
        public event IReducer.StateChangedHandler StateChanged;

        public string GetState(string stateBefore, string input)
        {
            if(stateBefore == "Connected")
            {
                if(input == "Enter a symbol to track:")
                {
                    string state = "WaitingForSymbol";
                    StateChanged.Invoke(this, new StateChangedEventArgs(state, stateBefore));
                    return state;
                }
            }

            return stateBefore;
        }
    }
}
