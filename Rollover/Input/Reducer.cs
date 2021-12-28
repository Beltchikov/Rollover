namespace Rollover.Input
{
    public class Reducer : IReducer
    {
        public event IReducer.StateChangedHandler StateChanged;

        public string GetState(string stateBefore, string input)
        {
            if (stateBefore == "WaitingForSymbol")
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
