using System;
using System.Collections.Generic;
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
        private float[] _result;

        public IReadOnlyList<float> Result => _result;

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
            _result = new float[ArrayLength];

            for (var i = 0; i < ArrayLength; ++i)
            {
                _argsAtan2[i, 0] = (float) (rnd.NextDouble() * Math.PI * 2 - Math.PI);
                _argsAtan2[i, 1] = (float) (rnd.NextDouble() * Math.PI * 2 - Math.PI);
            }

            _memSin = MemoizedSin.ConstructByValuesCount(10000);
            _memInterpolatedAtan = MemoizedInterpolatedAtan.ConstructByMaxError(0.01f);
            _memAtan = MemoizedAtan.ConstructByMaxError(0.01f);
            _memAtan2 = MemoizedAtan2.ConstructByMaxError(0.01f);
        }

        [Benchmark(Description = "MemoizedSin")]
        public void MemSin()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = _memSin.Calculate(arg);
            }
        }

        [Benchmark(Description = "MemoizedSinUnbounded")]
        public void SinUnbounded()
        {
            for (var i = 0; i < _unboundArgs.Length; i++)
            {
                var arg = _unboundArgs[i];
                _result[i] = _memSin.CalculateUnbound(arg);
            }
        }

        [Benchmark(Description = "MemoizedAtan")]
        public void MemAtan()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = _memAtan.Calculate(arg);
            }
        }

        [Benchmark(Description = "MemoizedUnboundAtan")]
        public void MemUnboundAtan()
        {
            for (var i = 0; i < _unboundArgs.Length; i++)
            {
                var arg = _unboundArgs[i];
                _result[i] = _memAtan.CalculateUnbound(arg);
            }
        }

        [Benchmark(Description = "MemoizedInterpolatedAtan")]
        public void MemInterpolatedAtan()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = _memInterpolatedAtan.Calculate(arg);
            }
        }

        [Benchmark(Description = "MemoizedInterpolatedUnboundAtan")]
        public void MemInterpolatedUnboundAtan()
        {
            for (var i = 0; i < _unboundArgs.Length; i++)
            {
                var arg = _unboundArgs[i];
                _result[i] = _memInterpolatedAtan.CalculateUnbound(arg);
            }
        }

        [Benchmark(Description = "MemoizedAtan2")]
        public void MemAtan2()
        {
            for (var i = 0; i < ArrayLength; ++i)
            {
                _result[i] = _memAtan2.Calculate(_argsAtan2[i, 0], _argsAtan2[i, 1]);
            }
        }
    }
}
