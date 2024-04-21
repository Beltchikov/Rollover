using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalOpener.Opener
{
    internal class SeekingAlphaOpener : IOpener
    {
        public string Execute(string[] symbols)
        {
            MessageBox.Show("SeekingAlphaOpener");
            return "";
        }
    }
}
