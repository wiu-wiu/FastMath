using System;
using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using FastMath;

namespace PerformanceBenchmarks
{
    [LegacyJitX86Job, RyuJitX64Job]
    public class Benchmark
    {
        private int _index;

        private float[] _array;

        private double[] _doubleArray;

        private MemoizedSin _memSin;

        private MemoizedCos _memCos;

        private MemoizedSqrt _memSqrt;

        private MemoizedPow _memPow;

        private MemoizedInterpolatedAtan _memInterpolatedAtan;

        private MemoizedAtan _memAtan;

        [Setup]
        public void Setup()
        {
            _index = 1;
            var rnd = new Random(DateTime.Now.Millisecond);

            _array = Enumerable.Range(0, 100).Select(e => (float) (rnd.NextDouble() * Math.PI * 2)).ToArray();
            _doubleArray = Enumerable.Range(0, 100).Select(e => (rnd.NextDouble() * Math.PI * 2)).ToArray();
            _memSin = new MemoizedSin(10000);
            _memCos = new MemoizedCos(10000);
            _memSqrt = new MemoizedSqrt(0, (float) (Math.PI * 2), 10000);
            _memPow = new MemoizedPow(0, (float) (Math.PI * 2), 2.5f, 1000);
            _memInterpolatedAtan = MemoizedInterpolatedAtan.ConstructByMaxError(0.01f);
            _memAtan = MemoizedAtan.ConstructByMaxError(0.01f);
        }

        private void Increment()
        {
            ++_index;
            if (_index > 99)
                _index = 0;
        }

        [Benchmark(Description = "Sin")]
        public double Sin()
        {
            Increment();
            return Math.Sin(_doubleArray[_index]);
        }

        [Benchmark(Description = "Atan")]
        public double Atan()
        {
            Increment();
            return Math.Atan(_doubleArray[_index]);
        }

        [Benchmark(Description = "MemoizedAtan")]
        public float MemAtan()
        {
            Increment();
            return _memAtan.Calculate(_array[_index]);
        }

        [Benchmark(Description = "MemoizedUnboundAtan")]
        public float MemUnboundAtan()
        {
            Increment();
            return _memAtan.CalculateUnbound(_array[_index]);
        }

        [Benchmark(Description = "MemoizedInterpolatedAtan")]
        public float MemInterpolatedAtan()
        {
            Increment();
            return _memInterpolatedAtan.Calculate(_array[_index]);
        }

        [Benchmark(Description = "MemoizedInterpolatedUnboundAtan")]
        public float MemInterpolatedUnboundAtan()
        {
            Increment();
            return _memInterpolatedAtan.CalculateUnbound(_array[_index]);
        }

        [Benchmark(Description = "MemoizedSin")]
        public float MemSin()
        {
            Increment();    
            return _memSin.Calculate(_array[_index]);
        }

        [Benchmark(Description = "MemoizedCos")]
        public float MemCos()
        {
            Increment();
            return _memCos.Calculate(_array[_index]);
        }
        
        [Benchmark(Description = "Sqrt")]
        public double Sqrt()
        {
            Increment();
            return Math.Sqrt(_doubleArray[_index]);
        }

        [Benchmark(Description = "MemoizedSqrt")]
        public float MemSqrt()
        {
            Increment();
            return _memSqrt.Calculate(_array[_index]);
        }

        [Benchmark(Description = "Pow")]
        public double Pow()
        {
            Increment();
            return Math.Pow(_doubleArray[_index], 2.5);
        }

        [Benchmark(Description = "MemoizedPow")]
        public float MemPow()
        {
            Increment();
            return _memPow.Calculate(_array[_index]);
        }
    }
}
