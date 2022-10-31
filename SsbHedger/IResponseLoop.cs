using System;

namespace SsbHedger
{
    public interface IResponseLoop
    {
        Action Actions { get; set; }
        Func<bool> BreakCondition { get; set; }
        void Start();
    }
}