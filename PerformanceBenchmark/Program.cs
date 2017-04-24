using BenchmarkDotNet.Running;
using Benchmark = PerformanceBenchmark.Benchmark;

namespace PerformanceBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }
}
