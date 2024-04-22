using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalOpener.Opener
{
    internal class SeekingAlphaOpener : IOpener
    {
        public string Execute(string[] symbols)
        {
           // MessageBox.Show("SeekingAlphaOpener");

            var process = new Process
            {
                StartInfo =
                {
                    FileName = Program.CHROME_PATH,
                    //WorkingDirectory = @"C:\myproject",
                    Arguments = ""
                }
            };

            process.Start();

            return "";
        }
    }
}
