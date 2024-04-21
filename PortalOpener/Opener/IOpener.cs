using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalOpener.Opener
{
    internal interface IOpener
    {
        string Execute(string[] symbols);  
    }
}
