using System;

namespace SsbHedger.WpfIbClient
{
    public interface IResponseLoop
    {
        Action Actions { get; set; }
        Func<bool> BreakCondition { get; set; }
        void Start();
    }
}