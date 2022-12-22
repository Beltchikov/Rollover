using System;

namespace SsbHedger2.WpfIbClient
{
    public class ResponseLoop : IResponseLoop
    {
        public Action Actions { get; set; } = null!;
        public Func<bool> BreakCondition { get; set; } = null!;

        public void Start()
        {
            while (!BreakCondition())
            {
                Actions();
            }
        }
    }
}
