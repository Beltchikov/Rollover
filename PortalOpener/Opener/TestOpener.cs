using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalOpener.Opener
{
    internal class TestOpener : IOpener
    {
        public string Execute(string[] symbols)
        {
            MessageBox.Show("TestOpener");
            return "";
        }
    }
}
