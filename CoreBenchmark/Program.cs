using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Benchmarks;

namespace CoreBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ManualConfig
                .Create(DefaultConfig.Instance)
                .With(Job.Core);

            BenchmarkRunner.Run<SystemMathBenchmark>();
            BenchmarkRunner.Run<FastMathBenchmark>();
        }
    }
}