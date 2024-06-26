﻿using System;
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
            var url = "https://seekingalpha.com/symbol/";

            var arguments = symbols
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => url+s.Trim())
                .Aggregate((r,n)=>r+ " " +n);

            var process = new Process
            {
                StartInfo =
                {
                    FileName = Program.CHROME_PATH,
                    Arguments = arguments
                }
            };

            process.Start();

            return "";
        }
    }
}
