using System;
using FastMath;

namespace FastMathSampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var method = MemoizedSin.ConstructByStep(0.001f);

            Console.WriteLine($"Values count: {method.Values.Length}");
            Console.WriteLine($"Mean error: {method.MeanError()}");
            Console.WriteLine($"Max error: {method.MaxError()}");

            Console.WriteLine(method.Error(0.0015f + (float) Math.PI * 100000));
            Console.ReadLine();
        }
    }
}
