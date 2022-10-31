using System;

namespace SsbHedger
{
    public class ResponseLoop : IResponseLoop
    {
        public Action Actions { get; set; } = null!;
        public Func<bool> BreakCondition { get; set; } = null!;

        public void Start()
        {
            while(!BreakCondition())
            {
                Actions(); 
            }
        }
    }
}
