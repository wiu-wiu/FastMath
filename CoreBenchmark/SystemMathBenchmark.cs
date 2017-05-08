using System;
using System.Collections.Generic;
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
        private double[] _result;

        public IReadOnlyList<double> Result => _result;

        [Setup]
        public void Setup()
        {
            var rnd = new Random(DateTime.Now.Millisecond);

            _args = Enumerable.Range(0, ArrayLength).Select(e => rnd.NextDouble() * Math.PI * 2).ToArray();
            _argsLog = Enumerable.Range(0, ArrayLength).Select(e => 1 + rnd.NextDouble() * 100).ToArray();
            _argsAtan2 = new double[ArrayLength, 2];
            _result = new double[ArrayLength];

            for (var i = 0; i < ArrayLength; ++i)
            {
                _argsAtan2[i, 0] = rnd.NextDouble() * 100 * 2 - 100;
                _argsAtan2[i, 1] = rnd.NextDouble() * 100 * 2 - 100;
            }
        }

        [Benchmark(Description = "Sin")]
        public void Sin()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Sin(arg);
            }
        }

        [Benchmark(Description = "Cos")]
        public void Cos()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Cos(arg);
            }
        }

        [Benchmark(Description = "Tan")]
        public void Tan()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Tan(arg);
            }
        }

        [Benchmark(Description = "Asin")]
        public void Asin()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Asin(arg);
            }
        }

        [Benchmark(Description = "Acos")]
        public void Acos()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Acos(arg);
            }
        }

        [Benchmark(Description = "Atan")]
        public void Atan()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Atan(arg);
            }
        }

        [Benchmark(Description = "Atan2")]
        public void Atan2()
        {
            for (var i = 0; i < ArrayLength; ++i)
            {
                _result[i] = Math.Atan2(_argsAtan2[i, 0], _argsAtan2[i, 1]);
            }
        }

        [Benchmark(Description = "Sqrt")]
        public void Sqrt()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Sqrt(arg);
            }
        }

        [Benchmark(Description = "Sinh")]
        public void Sinh()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Sinh(arg);
            }
        }

        [Benchmark(Description = "Cosh")]
        public void Cosh()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Cosh(arg);
            }
        }

        [Benchmark(Description = "Tanh")]
        public void Tanh()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Tanh(arg);
            }
        }

        [Benchmark(Description = "Pow")]
        public void Pow()
        {
            for (var i = 0; i < _args.Length; i++)
            {
                var arg = _args[i];
                _result[i] = Math.Pow(arg, 2.5);
            }
        }

        [Benchmark(Description = "Log")]
        public void Log()
        {
            for (var i = 0; i < _argsLog.Length; i++)
            {
                var arg = _argsLog[i];
                _result[i] = Math.Log(arg, 10);
            }
        }
    }
}
