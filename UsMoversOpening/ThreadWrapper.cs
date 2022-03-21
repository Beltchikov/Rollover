using System.Threading;

namespace UsMoversOpening
{
    public class ThreadWrapper : IThreadWrapper
    {
        private Thread _thread;

        public ThreadWrapper(ThreadStart threadStart, bool isBackground)
        {
            _thread = new Thread(threadStart);
            _thread.IsBackground = isBackground;
        }
        
        public void Start()
        {
            _thread.Start();
        }
    }
}
