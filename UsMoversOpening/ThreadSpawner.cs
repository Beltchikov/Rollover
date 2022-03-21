using System;
using System.Diagnostics.CodeAnalysis;

namespace UsMoversOpening
{
    public class ThreadSpawner : IThreadSpawner
    {
        private IUmoAgent _umoAgent;

        public ThreadSpawner(IUmoAgent umoAgent)
        {
            _umoAgent = umoAgent;
        }

        public bool ExitFlagInputThread { get; set; }

        public void Run()
        {
            var inputThread = InputThreadFactory();
            _umoAgent.Run(this, inputThread);
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
    }
}
