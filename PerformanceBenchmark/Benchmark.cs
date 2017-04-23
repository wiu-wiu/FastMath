using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using FastMath;

namespace PerformanceBenchmarks
{
    [LegacyJitX86Job, RyuJitX64Job]
    public class Benchmark
    {
        private int _index;

        private const int ArrayLength = 1000;

        private float[] _array;

        private float[] _unboundArray;

        private float[,] _array2;

        private double[] _doubleArray;

        private double[,] _doubleArray2;

        private MemoizedSin _memSin;

        private MemoizedCos _memCos;

        private MemoizedSqrt _memSqrt;

        private MemoizedPow _memPow;

        private MemoizedInterpolatedAtan _memInterpolatedAtan;

        private MemoizedAtan _memAtan;

        private MemoizedAtan2 _memAtan2;

        [Setup]
        public void Setup()
        {
            _index = 1;
            var rnd = new Random(DateTime.Now.Millisecond);

            _array = Enumerable.Range(0, ArrayLength).Select(e => (float) (rnd.NextDouble() * Math.PI * 2)).ToArray();
            _unboundArray = Enumerable.Range(0, ArrayLength).Select(e => (float)(rnd.NextDouble() * Math.PI * 100 - Math.PI * 50)).ToArray();
            _doubleArray = Enumerable.Range(0, ArrayLength).Select(e => rnd.NextDouble() * Math.PI * 2).ToArray();

            _array2 = new float[ArrayLength, 2];
            _doubleArray2 = new double[ArrayLength, 2];

            for (var i = 0; i < ArrayLength; ++i)
            {
                _array2[i, 0] = (float) (rnd.NextDouble() * Math.PI * 2 - Math.PI);
                _array2[i, 1] = (float) (rnd.NextDouble() * Math.PI * 2 - Math.PI);

                _doubleArray2[i, 0] = rnd.NextDouble() * Math.PI * 2 - Math.PI;
                _doubleArray2[i, 1] = rnd.NextDouble() * Math.PI * 2 - Math.PI;
            }

            _memSin = new MemoizedSin(10000);
            _memCos = new MemoizedCos(10000);
            _memSqrt = new MemoizedSqrt(0, (float) (Math.PI * 2), 10000);
            _memPow = new MemoizedPow(0, (float) (Math.PI * 2), 2.5f, 1000);
            _memInterpolatedAtan = MemoizedInterpolatedAtan.ConstructByMaxError(0.01f);
            _memAtan = MemoizedAtan.ConstructByMaxError(0.01f);
            _memAtan2 = MemoizedAtan2.ConstructByMaxError(0.01f);
        }

        private void Increment()
        {
            ++_index;
            if (_index > ArrayLength - 1)
                _index = 0;
        }

        [Benchmark(Description = "Sin")]
        public double Sin()
        {
            Increment();
            return Math.Sin(_doubleArray[_index]);
        }

        [Benchmark(Description = "MemoizedSin")]
        public float MemSin()
        {
            Increment();
            return _memSin.Calculate(_array[_index]);
        }

        [Benchmark(Description = "MemoizedSinUnbounded")]
        public double SinUnbounded()
        {
            Increment();
            return _memSin.CalculateUnbound(_unboundArray[_index]);
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

        [Benchmark(Description = "Atan2")]
        public double Atan2()
        {
            Increment();
            return Math.Atan2(_doubleArray2[_index, 0], _doubleArray2[_index, 1]);
        }

        [Benchmark(Description = "MemoizedAtan2")]
        public float MemAtan2()
        {
            Increment();
            return _memAtan2.Calculate(_array2[_index, 0], _array2[_index, 1]);
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
