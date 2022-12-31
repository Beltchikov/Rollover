using System.ComponentModel;

namespace SsbHedger.Abstractions
{
    public interface IBackgroundWorkerAbstraction
    {
        void SetDoWorkEventHandler(DoWorkEventHandler handler);
        void RunWorkerAsync();
    }
}