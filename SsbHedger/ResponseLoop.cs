using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger
{
    public class ResponseLoop : IResponseLoop
    {
        Action _action;
        Func<bool> _breakCondition;

        public ResponseLoop(Action action, Func<bool> breakCondition)
        {
            _breakCondition = breakCondition;
            _action = action;
        }

        public void Start()
        {
            while(!_breakCondition())
            { 
                _action(); 
            }
        }
    }
}
