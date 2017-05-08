using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Benchmarks;

namespace ClrBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ManualConfig
                .Create(DefaultConfig.Instance)
                .With(Job.LegacyJitX86)
                .With(Job.RyuJitX64)
                .With(Job.Mono);

            BenchmarkRunner.Run<SystemMathBenchmark>(config);
            BenchmarkRunner.Run<FastMathBenchmark>(config);
        }
    }
}
