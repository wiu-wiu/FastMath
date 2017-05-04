using System;
using FastMath;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class MethodsTests
    {
        private readonly float[] _maxErrors = {1, 1e-1f, 1e-2f, 1e-3f, 1e-4f, 1e-5f};

        private static void BasicChecks(IMemoizedMethod method)
        {
            Assert.IsTrue(method.Values.Length > 0);
            Assert.IsTrue(method.BaseMethod != null);
            Assert.IsTrue(method.MinArgument < method.MaxArgument);
            Assert.IsTrue(method.Step > 0);
        }

        private static void TestUnboundMethod(IUnboundMethod method, float maxError)
        {
            BasicChecks(method);

            var interval = method.MaxArgument - method.MinArgument;
            var from = method.MinArgument - interval * 100;
            var to = method.MaxArgument + interval * 100;
            var step = interval / 10;
            var argument = from;

            for (var i = 0; argument <= to; ++i)
            {
                argument = from + step * i;
                var value = method.CalculateUnbound(argument);
                Assert.IsTrue(Math.Abs(value - method.BaseMethod(argument)) <= maxError,
                    message: $"unbound method check failed, max error is {maxError}, argument is {argument}" +
                             $", actual error is {Math.Abs(value - method.BaseMethod(argument))}");
            }
            Assert.IsTrue(method.MaxError() <= maxError,
                message: $"max error is {maxError}, but actual max error is {method.MaxError()}");
        }

        private static void CheckMethodStep(Func<float, IMemoizedMethod> createMethodByStep)
        {
            var steps = new[] {1f, 0.1f, 0.01f, 0.001f};
            foreach (var step in steps)
            {
                var method = createMethodByStep(step);
                Assert.IsTrue(Math.Abs(method.Step - step) <= 0.1f * step);
            }
        }

        [Test]
        public void TestSin()
        {
            foreach (var error in _maxErrors)
            {
                var method = MemoizedSin.ConstructByMaxError(error);
                TestUnboundMethod(method, 5 * error);
            }
        }

        [Test]
        public void TestCos()
        {
            foreach (var error in _maxErrors)
            {
                var method = MemoizedCos.ConstructByMaxError(error);
                TestUnboundMethod(method, 5 * error);
            }
        }

        [Test]
        public void TestTan()
        {
            var methodStep = 1e-5f;
            var method = MemoizedTan.ConstructByStep(methodStep);
            CheckMethodStep(MemoizedTan.ConstructByStep);

            var minArgument = method.MinArgument + 0.1f;
            var maxArgument = method.MaxArgument - 0.1f;
            var step = method.Step / 2;

            for (var argument = minArgument; argument <= maxArgument; argument += step)
            {
                Assert.IsTrue(method.Error(argument) < 1e-3f);
            }
        }

        [Test]
        public void TestAtan()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};

            foreach (var error in maxErrors)
            {
                var method = MemoizedAtan.ConstructByMaxError(error);
                TestUnboundMethod(method, error);
            }
        }

        [Test]
        public void TestInterpolatedAtan()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};

            foreach (var error in maxErrors)
            {
                var method = MemoizedInterpolatedAtan.ConstructByMaxError(error);
                TestUnboundMethod(method, error);
            }
        }

        [Test]
        public void TestLog()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};
            var arguments = new[] {1e-2f, 1, 10, 100, 1000};
            var bases = new[] {0.01f, 0.1f, (float) Math.E, 10};

            for (var i = 0; i < arguments.Length; ++i)
            {
                for (var j = i + 1; j < arguments.Length; ++j)
                {
                    foreach (var @base in bases)
                    {
                        foreach (var error in maxErrors)
                        {
                            var method = MemoizedLog.ConstructByMaxError(arguments[i], arguments[j], @base, error);
                            BasicChecks(method);
                            Assert.IsTrue(method.MaxError() <= error, 
                                message: $"max error is {error}, but actual error is {method.MaxError()}," +
                                         $"base is {@base}.");
                        }
                    }
                }
            }
        }

        [Test]
        public void TestPow()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};
            var arguments = new[] {-20, 1e-1f, 1, 5, 20};
            var powers = new[] {-2, -1, 0.1f, 2, (float) Math.E};

            for (var i = 0; i < arguments.Length; ++i)
            {
                for (var j = i + 1; j < arguments.Length; ++j)
                {
                    foreach (var power in powers)
                    {
                        foreach (var error in maxErrors)
                        {
                            if (power < 1 && arguments[i] < 0)
                            {
                                continue;
                            }
                            if (Math.Abs(power - (int) power) > 1e-5 && arguments[i] < 0)
                            {
                                continue;
                            }
                            var method = MemoizedPow.ConstructByMaxError(arguments[i], arguments[j], power, error);
                            BasicChecks(method);
                            Assert.IsTrue(method.MaxError() <= error,
                                message: $"max error is {error}, but actual error is {method.MaxError()}," +
                                         $"power is {power}, interval from {arguments[i]} to {arguments[j]}.");
                        }
                    }
                }
            }
        }

        [Test]
        public void TestExp()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};
            var arguments = new[] {-5, 1e-1f, 1, 5};
            var bases = new[] {0.5f, 2, (float) Math.E};

            for (var i = 0; i < arguments.Length; ++i)
            {
                for (var j = i + 1; j < arguments.Length; ++j)
                {
                    foreach (var @base in bases)
                    {
                        foreach (var error in maxErrors)
                        {
                            var method = MemoizedExp.ConstructByMaxError(arguments[i], arguments[j], @base, error);
                            BasicChecks(method);
                            Assert.IsTrue(method.MaxError() <= error,
                                message: $"max error is {error}, but actual error is {method.MaxError()}," +
                                         $"base is {@base}, interval from {arguments[i]} to {arguments[j]}.");
                        }
                    }
                }
            }
        }

        [Test]
        public void TestSqrt()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};
            var arguments = new[] {0, 1e-1f, 1, 5, 100};

            for (var i = 0; i < arguments.Length; ++i)
            {
                for (var j = i + 1; j < arguments.Length; ++j)
                {
                    foreach (var error in maxErrors)
                    {
                        var method = MemoizedSqrt.ConstructByMaxError(arguments[i], arguments[j], error);
                        BasicChecks(method);
                        Assert.IsTrue(method.MaxError() <= error,
                            message: $"max error is {error}, but actual error is {method.MaxError()}, " +
                                     $"interval from {arguments[i]} to {arguments[j]}.");
                    }
                }
            }
        }

        [Test]
        public void TestAsin()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};

            foreach (var error in maxErrors)
            {
                var method = MemoizedAsin.ConstructByMaxError(error);
                BasicChecks(method);
                Assert.IsTrue(method.MaxError() <= error,
                    message: $"max error is {error}, but actual error is {method.MaxError()}");
            }
        }

        [Test]
        public void TestAcos()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};

            foreach (var error in maxErrors)
            {
                var method = MemoizedAcos.ConstructByMaxError(error);
                BasicChecks(method);
                Assert.IsTrue(method.MaxError() <= error,
                    message: $"max error is {error}, but actual error is {method.MaxError()}");
            }
        }

        [Test]
        public void TestInterpolatedAcos()
        {
            var maxErrors = new[] { 1, 1e-1f, 1e-2f, 1e-3f };

            foreach (var error in maxErrors)
            {
                var method = MemoizedInterpolatedAcos.ConstructByMaxError(error);
                BasicChecks(method);
                Assert.IsTrue(method.MaxError() <= error,
                    message: $"max error is {error}, but actual error is {method.MaxError()}");
            }
        }

        [Test]
        public void TestInterpolatedAsin()
        {
            var maxErrors = new[] { 1, 1e-1f, 1e-2f, 1e-3f };

            foreach (var error in maxErrors)
            {
                var method = MemoizedInterpolatedAsin.ConstructByMaxError(error);
                BasicChecks(method);
                Assert.IsTrue(method.MaxError() <= error,
                    message: $"max error is {error}, but actual error is {method.MaxError()}");
            }
        }

        [Test]
        public void TestTanh()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};
            var arguments = new[] {-5, 0, 1e-1f, 1, 5, 100};

            for (var i = 0; i < arguments.Length; ++i)
            {
                for (var j = i + 1; j < arguments.Length; ++j)
                {
                    foreach (var error in maxErrors)
                    {
                        var method = MemoizedTanh.ConstructByMaxError(arguments[i], arguments[j], error);
                        BasicChecks(method);
                        Assert.IsTrue(method.MaxError() <= error,
                            message: $"max error is {error}, but actual error is {method.MaxError()}, " +
                                     $"interval from {arguments[i]} to {arguments[j]}.");
                    }
                }
            }
        }

        [Test]
        public void TestSinh()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};
            var arguments = new[] {-5, 0, 1e-1f, 1, 5};

            for (var i = 0; i < arguments.Length; ++i)
            {
                for (var j = i + 1; j < arguments.Length; ++j)
                {
                    foreach (var error in maxErrors)
                    {
                        var method = MemoizedCosh.ConstructByMaxError(arguments[i], arguments[j], error);
                        BasicChecks(method);
                        Assert.IsTrue(method.MaxError() <= error,
                            message: $"max error is {error}, but actual error is {method.MaxError()}, " +
                                     $"interval from {arguments[i]} to {arguments[j]}.");
                    }
                }
            }
        }

        [Test]
        public void TestCosh()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};
            var arguments = new[] {-5, 0, 1e-1f, 1, 5};

            for (var i = 0; i < arguments.Length; ++i)
            {
                for (var j = i + 1; j < arguments.Length; ++j)
                {
                    foreach (var error in maxErrors)
                    {
                        var method = MemoizedCosh.ConstructByMaxError(arguments[i], arguments[j], error);
                        BasicChecks(method);
                        Assert.IsTrue(method.MaxError() <= error,
                            message: $"max error is {error}, but actual error is {method.MaxError()}, " +
                                     $"interval from {arguments[i]} to {arguments[j]}.");
                    }
                }
            }
        }

        [Test]
        public void TestAtan2()
        {
            var maxErrors = new[] {1, 1e-1f, 1e-2f, 1e-3f};

            const int minArgument = -20;
            const int maxArgument = 20;
            const float step = 0.01f;
            const int count = (int) ((maxArgument - minArgument) / step + 1);

            foreach (var error in maxErrors)
            {
                var method = MemoizedAtan2.ConstructByMaxError(error);
                TestMethod(method, error);
            }

            void TestMethod(MemoizedAtan2 method, float error)
            {
                for (var i = 0; i < count; ++i)
                {
                    var x = minArgument + i * step;
                    for (var j = 0; j < count; ++j)
                    {
                        var y = minArgument + j * step;

                        if (Math.Abs(x) < 1e-3 && Math.Abs(y) < 1e-3)
                        {
                            continue;
                        }
                        var result = method.Calculate(y, x);
                        var realResult = Math.Atan2(y, x);
                        Assert.IsTrue(Math.Abs(realResult - result) <= error, 
                            message: $"max error is {error}, but actual error is {Math.Abs(realResult - result)}, " +
                                     $"arguments are ({x}, {y}).");
                    }
                }
            }
        }
    }
}

