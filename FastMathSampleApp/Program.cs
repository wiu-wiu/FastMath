using System;
using FastMath.Core;
using FastMath.Interpolated;

namespace FastMathSampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var method = MemoizedInterpolatedAsin.ConstructByMaxError(0.001f);

            Console.WriteLine($"Values count: {method.Values.Length}");
            Console.WriteLine($"Mean error: {method.MeanError()}");
            Console.WriteLine($"Max error: {method.MaxError()}");

            Console.ReadLine();
        }
    }
}
