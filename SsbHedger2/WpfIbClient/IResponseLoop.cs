using System;

namespace SsbHedger2.WpfIbClient
{
    public interface IResponseLoop
    {
        Action Actions { get; set; }
        Func<bool> BreakCondition { get; set; }
        void Start();
    }
}