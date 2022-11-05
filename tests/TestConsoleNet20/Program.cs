using System;
using System.Collections.Generic;
using System.Text;
using TG.Common;

namespace ConsoleNet20
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This Assembly");
            Console.WriteLine("-------------");
            Console.WriteLine(AssemblyInfo.Product);
            Console.WriteLine(AssemblyInfo.FileVersion);
            Console.WriteLine(AssemblyInfo.Description);
            Console.WriteLine(AssemblyInfo.Configuration);
            Console.WriteLine("TG.Common");
            Console.WriteLine("-------------");
            AssemblyInfo.ReferenceAssembly = typeof(TG.Common.Crypto).Assembly;
            Console.WriteLine(AssemblyInfo.Product);
            Console.WriteLine(AssemblyInfo.FileVersion);
            Console.WriteLine(AssemblyInfo.Description);
            Console.WriteLine(AssemblyInfo.Configuration);

            Console.ReadLine();
        }
    }
}
