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
    }
}
