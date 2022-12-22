using System.ComponentModel;

namespace SsbHedger2.Abstractions
{
    public class BackgroundWorkerAbstraction : IBackgroundWorkerAbstraction
    {
        private BackgroundWorker _backgroundWorker = new();
        
        public void SetDoWorkEventHandler(DoWorkEventHandler handler)
        {
            _backgroundWorker.DoWork += handler;
        }

        public void RunWorkerAsync()
        {
            _backgroundWorker.RunWorkerAsync();
        }
    }
}
