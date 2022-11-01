using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger
{
    public class ConsoleAbstraction : IConsoleAbstraction
    {
        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();   
        }
    }
}
