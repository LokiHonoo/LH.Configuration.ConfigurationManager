using Middleware;
using System;

namespace Test
{
    internal class Program
    {
        private static void Main()
        {
            TestAppSettings.Create();
            Console.WriteLine(TestAppSettings.Load());
            TestConnectionStrings.Create();
            Console.WriteLine(TestConnectionStrings.Load());
            TestSection.Create();
            Console.WriteLine(TestSection.Load());
            Console.ReadKey(true);
        }
    }
}