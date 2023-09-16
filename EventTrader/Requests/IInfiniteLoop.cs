using System;
using System.Threading.Tasks;

namespace Dsmn.Requests
{
    public interface IInfiniteLoop
    {
        Task StartAsync(Func<object> function, object[] parameters);
        
        event Action<string> Status;
        bool Stopped { get; set; }
        bool IsRunning { get; set; }


    }
}