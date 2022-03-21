using System;

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
            // Create input thread
            var inputThread = new ThreadWrapper(
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
                true) ;


            _umoAgent.Run(this, inputThread);
        }
    }
}
