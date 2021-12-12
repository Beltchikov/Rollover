using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Ib
{
    public class IbClientWrapper : IIbClientWrapper
    {
        public event Action<int, int, string, Exception> Error;
    }
}
