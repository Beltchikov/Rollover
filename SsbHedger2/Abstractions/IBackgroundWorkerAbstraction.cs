using System.ComponentModel;

namespace SsbHedger2.Abstractions
{
    public interface IBackgroundWorkerAbstraction
    {
        void SetDoWorkEventHandler(DoWorkEventHandler handler);
        void RunWorkerAsync();
    }
}