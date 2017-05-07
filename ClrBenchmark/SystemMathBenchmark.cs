﻿using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class SystemMathBenchmark
    {
        private const int ArrayLength = 1000;

        private double[] _args;
        private double[] _argsLog;
        private double[,] _argsAtan2;

        [Setup]
        public void Setup()
        {
            var rnd = new Random(DateTime.Now.Millisecond);

            _args = Enumerable.Range(0, ArrayLength).Select(e => rnd.NextDouble() * Math.PI * 2).ToArray();
            _argsLog = Enumerable.Range(0, ArrayLength).Select(e => 1 + rnd.NextDouble() * 100).ToArray();
            _argsAtan2 = new double[ArrayLength, 2];

            for (var i = 0; i < ArrayLength; ++i)
            {
                _argsAtan2[i, 0] = rnd.NextDouble() * 100 * 2 - 100;
                _argsAtan2[i, 1] = rnd.NextDouble() * 100 * 2 - 100;
            }
        }

        [Benchmark(Description = "Sin")]
        public void Sin()
        {
            foreach (var arg in _args)
            {
                Math.Sin(arg);
            }
        }

        [Benchmark(Description = "Cos")]
        public void Cos()
        {
            foreach (var arg in _args)
            {
                Math.Cos(arg);
            }
        }

        [Benchmark(Description = "Tan")]
        public void Tan()
        {
            foreach (var arg in _args)
            {
                Math.Tan(arg);
            }
        }

        [Benchmark(Description = "Asin")]
        public void Asin()
        {
            foreach (var arg in _args)
            {
                Math.Asin(arg);
            }
        }

        [Benchmark(Description = "Acos")]
        public void Acos()
        {
            foreach (var arg in _args)
            {
                Math.Acos(arg);
            }
        }

        [Benchmark(Description = "Atan")]
        public void Atan()
        {
            foreach (var arg in _args)
            {
                Math.Atan(arg);
            }
        }

        [Benchmark(Description = "Atan2")]
        public void Atan2()
        {
            for (var i = 0; i < ArrayLength; ++i)
            {
                Math.Atan2(_argsAtan2[i, 0], _argsAtan2[i, 1]);
            }
        }
        
        [Benchmark(Description = "Sqrt")]
        public void Sqrt()
        {
            foreach (var arg in _args)
            {
                Math.Sqrt(arg);
            }
        }

        [Benchmark(Description = "Sinh")]
        public void Sinh()
        {
            foreach (var arg in _args)
            {
                Math.Sinh(arg);
            }
        }

        [Benchmark(Description = "Cosh")]
        public void Cosh()
        {
            foreach (var arg in _args)
            {
                Math.Cosh(arg);
            }
        }

        [Benchmark(Description = "Tanh")]
        public void Tanh()
        {
            foreach (var arg in _args)
            {
                Math.Tanh(arg);
            }
        }

        [Benchmark(Description = "Pow")]
        public void Pow()
        {
            foreach (var arg in _args)
            {
                Math.Pow(arg, 2.5);
            }
        }

        [Benchmark(Description = "Log")]
        public void Log()
        {
            foreach (var arg in _argsLog)
            {
                Math.Log(arg, 10);
            }
        }
    }
}
