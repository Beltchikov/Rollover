using System;
using System.Diagnostics.CodeAnalysis;

namespace UsMoversOpening
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        static void Main(string[] args)
        {
            IUmoAgent umoAgent = new UmoAgent();
            umoAgent.Run();
            Console.WriteLine("Hello World!");
        }
    }
}
