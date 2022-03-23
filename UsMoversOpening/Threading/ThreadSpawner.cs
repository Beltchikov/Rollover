using System;
using System.Diagnostics.CodeAnalysis;
using UsMoversOpening.IBApi;

namespace UsMoversOpening.Threading
{
    public class ThreadSpawner : IThreadSpawner
    {
        private IUmoAgent _umoAgent;
        private IIbClientWrapper _ibClientWrapper;
        private IEReaderWrapper _eReaderWrapper;

        public ThreadSpawner(
            IUmoAgent umoAgent,
            IIbClientWrapper ibClientWrapper, 
            IEReaderWrapper eReaderWrapper)
        {
            _umoAgent = umoAgent;
            _ibClientWrapper = ibClientWrapper;
            _eReaderWrapper = eReaderWrapper;
        }

        public bool ExitFlagInputThread { get; set; }

        public void Run()
        {
            var inputThread = InputThreadFactory();
            var ibThread = IbThreadFactory();

            _umoAgent.Run(this, inputThread, ibThread);
        }

        [ExcludeFromCodeCoverage]
        private ThreadWrapper InputThreadFactory()
        {
            return new ThreadWrapper(
                () =>
                {
                    while (true)
                    {
                        if (Console.ReadLine() == "q")
                        {
                            ExitFlagInputThread = true;
                        }
                    }
                },
                true);
        }

        [ExcludeFromCodeCoverage]
        private ThreadWrapper IbThreadFactory()
        {
            return new ThreadWrapper(() =>
            {
                while (_ibClientWrapper.ClientSocket.IsConnected())
                {
                    _ibClientWrapper.Signal.waitForSignal();
                    _eReaderWrapper.processMsgs();
                }
            }, true);
        }
    }
}
