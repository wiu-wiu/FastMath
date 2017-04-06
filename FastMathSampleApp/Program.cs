using System;
using FastMath;

namespace FastMathSampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var method = MemoizedAtan.ConstructByStep(0.1f);

            Console.WriteLine($"Values count: {method.Values.Length}");
            Console.WriteLine($"Mean error: {method.MeanError()}");
            Console.WriteLine($"Max error: {method.MaxError()}");

            Console.ReadLine();
        }
    }
}
