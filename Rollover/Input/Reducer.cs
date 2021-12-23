namespace Rollover.Input
{
    public class Reducer : IReducer
    {
        public event IReducer.StateChangedHandler StateChanged;

        public string GetState(string stateBefore, string input)
        {
            if (stateBefore == "Connected")
            {
                if (input == null)
                {
                    return stateBefore;
                }
                else if (input == "Enter a symbol to track:")
                {
                    string state = "WaitingForSymbol";
                    StateChanged?.Invoke(this, new StateChangedEventArgs(state, stateBefore));
                    return state;
                }
            }
            else if (stateBefore == "WaitingForSymbol")
            {
                if (input != null)
                {
                    return "ContractInfo";
                }
            }
            else if (stateBefore == "ContractInfo")
            {
                if (input != null)
                {
                    return "WaitingForSymbol";
                }
            }

            return stateBefore;
        }
    }
}
