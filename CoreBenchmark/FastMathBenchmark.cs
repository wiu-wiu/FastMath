using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FastMath;
using FastMath.Interpolated;

namespace Benchmarks
{
    public class FastMathBenchmark
    {
        private const int ArrayLength = 1000;

        private float[] _args;
        private float[] _unboundArgs;
        private float[,] _argsAtan2;

        private MemoizedSin _memSin;
        private MemoizedInterpolatedAtan _memInterpolatedAtan;
        private MemoizedAtan _memAtan;
        private MemoizedAtan2 _memAtan2;

        [Setup]
        public void Setup()
        {
            var rnd = new Random(DateTime.Now.Millisecond);

            _args = Enumerable.Range(0, ArrayLength).Select(e => (float) (rnd.NextDouble() * Math.PI * 2)).ToArray();
            _unboundArgs = Enumerable.Range(0, ArrayLength).Select(e => (float)(rnd.NextDouble() * Math.PI * 100 - Math.PI * 50)).ToArray();

            _argsAtan2 = new float[ArrayLength, 2];

            for (var i = 0; i < ArrayLength; ++i)
            {
                _argsAtan2[i, 0] = (float) (rnd.NextDouble() * Math.PI * 2 - Math.PI);
                _argsAtan2[i, 1] = (float) (rnd.NextDouble() * Math.PI * 2 - Math.PI);
            }

            _memSin = new MemoizedSin(10000);
            _memInterpolatedAtan = MemoizedInterpolatedAtan.ConstructByMaxError(0.01f);
            _memAtan = MemoizedAtan.ConstructByMaxError(0.01f);
            _memAtan2 = MemoizedAtan2.ConstructByMaxError(0.01f);
        }

        [Benchmark(Description = "MemoizedSin")]
        public void MemSin()
        {
            foreach (var arg in _args)
            {
                _memSin.Calculate(arg);
            }
        }

        [Benchmark(Description = "MemoizedSinUnbounded")]
        public void SinUnbounded()
        {
            foreach (var arg in _unboundArgs)
            {
                _memSin.CalculateUnbound(arg);
            }
        }

        [Benchmark(Description = "MemoizedAtan")]
        public void MemAtan()
        {
            foreach (var arg in _args)
            {
                _memAtan.Calculate(arg);
            }
        }

        [Benchmark(Description = "MemoizedUnboundAtan")]
        public void MemUnboundAtan()
        {
            foreach (var arg in _unboundArgs)
            {
                _memAtan.CalculateUnbound(arg);
            }
        }

        [Benchmark(Description = "MemoizedInterpolatedAtan")]
        public void MemInterpolatedAtan()
        {
            foreach (var arg in _args)
            {
                _memInterpolatedAtan.Calculate(arg);
            }
        }

        [Benchmark(Description = "MemoizedInterpolatedUnboundAtan")]
        public void MemInterpolatedUnboundAtan()
        {
            foreach (var arg in _unboundArgs)
            {
                _memInterpolatedAtan.CalculateUnbound(arg);
            }
        }

        [Benchmark(Description = "MemoizedAtan2")]
        public void MemAtan2()
        {
            for (var i = 0; i < ArrayLength; ++i)
            {
                _memAtan2.Calculate(_argsAtan2[i, 0], _argsAtan2[i, 1]);
            }
        }
    }
}
