using System;
using System.Collections.Generic;
using System.Linq;

namespace Rollover.Ib
{
    public class ConnectedCondition : IConnectedCondition
    {
        private List<string> _inputList = new List<string>();
        
        public void AddInput(string input)
        {
            _inputList.Add(input);  
        }

        public bool IsConnected()
        {
            bool condition = _inputList.Any(i => i.Contains("id=-1"));
            condition = condition && _inputList.Any(i => i.Contains("errorCode=2104"));
            condition = condition && _inputList.Any(i => i.Contains("Market data"));
            condition = condition && _inputList.Any(i => i.Contains("OK"));

            return condition;
       }
    }
}
