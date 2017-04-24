using FastMath;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        public void TestError()
        {
            var method = new MemoizedMethod(x => x, minArgument: -100, maxArgument: 900, valuesCount: 1000);

            Assert.AreEqual(method.Error(0), 0, 1e-5);
            Assert.AreEqual(method.Error(100), 0, 1e-5);
            Assert.AreEqual(method.Error(method.MinArgument), 0, 1e-5);
            Assert.AreEqual(method.Error(method.MaxArgument), 0, 1e-5);

            Assert.AreEqual(method.Error(0.9f), 0, 1);
            Assert.AreEqual(method.Error(100.999f), 0, 1);
            Assert.AreEqual(method.Error(method.MinArgument + 1 - 1e-3f), 0, 1);
            Assert.AreEqual(method.Error(method.MaxArgument - 1 - 1e-3f), 0, 1);
        }

        [Test]
        public void TestMaxError()
        {
            var valuesCounts = new[] {1, 10, 100, 1000, 10000};

            foreach (var valuesCount in valuesCounts)
            {
                var method = new MemoizedMethod(x => x, minArgument: -100, maxArgument: 900, valuesCount: valuesCount);
                Assert.AreEqual(method.MaxError(), method.Step, 1e-3);
            }

            foreach (var valuesCount in valuesCounts)
            {
                var method = new MemoizedInterpolatedMethod(x => x, minArgument: -100, maxArgument: 900, valuesCount: valuesCount);
                Assert.AreEqual(method.MaxError(), 0, 1e-3f);
            }
        }

        [Test]
        public void TestMeanError()
        {
            var valuesCounts = new[] {1, 10, 100, 1000, 10000};

            foreach (var valuesCount in valuesCounts)
            {
                var method = new MemoizedMethod(x => x, minArgument: -100, maxArgument: 900, valuesCount: valuesCount);
                Assert.IsTrue(method.MeanError() <= method.MaxError());
            }

            foreach (var valuesCount in valuesCounts)
            {
                var method = new MemoizedInterpolatedMethod(x => x, minArgument: -100, maxArgument: 900, valuesCount: valuesCount);
                Assert.IsTrue(method.MeanError() <= method.MaxError());
            }
        }
    }
}
