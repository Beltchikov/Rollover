using System;
using System.Threading;

namespace EventTrader.Requests
{
    public class RequestLoop : IInfiniteLoop
    {
        public bool Stopped { get; set; }
        public bool IsRunning { get ; set; }

        public event Action<string> Status = null!;

        public void Start(Action action, object[] parameters)
        {
            if(!IsRunning)
            {
                int i = 0;
                int frequency = 1000;  // TODO from parameters

                while (!Stopped)
                {
                    action();
                    Status?.Invoke($"Loop {i}");
                    i++;
                    Thread.Sleep(frequency);    
                }
            }
        }
    }
}
