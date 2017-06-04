using System;
using System.Threading.Tasks;

namespace FastMath.Core
{
    public static class Utils
    {
        internal static void ProduceValuesArray(this IMemoizedMethod memoizedMethod, int additionalValues = 1, bool useParallelProduction = true)
        {
            var minArgument = (double) memoizedMethod.MinArgument;
            var maxArgument = (double) memoizedMethod.MaxArgument;
            var step = (maxArgument - minArgument) / (memoizedMethod.Values.Length - additionalValues);

            if (memoizedMethod.Values.Length < 10 * 1024 * 1024 || !useParallelProduction)
            {
                for (var i = 0; i < memoizedMethod.Values.Length; ++i)
                {
                    var argument = i * step + minArgument;
                    memoizedMethod.Values[i] = memoizedMethod.BaseMethod((float) argument);
                }
            }
            else
            {
                Parallel.For(0, memoizedMethod.Values.Length, i =>
                {
                    var argument = i * step + minArgument;
                    memoizedMethod.Values[i] = memoizedMethod.BaseMethod((float) argument);
                });
            }
        }

        public static float MaxError(this IMemoizedMethod method)
        {
            return method.IsLinearInterpolated
                ? InterpolatedMaxError(method)
                : SimpleMaxError(method);
        }

        /// <summary>
        /// Caclulates error between memoized method and the original one with argument
        /// </summary>
        public static float Error(this IMemoizedMethod method, float argument)
        {
            return Math.Abs(method.Calculate(argument) - method.BaseMethod(argument));
        }

        /// <summary>
        /// Caclulates error between unbound method and the original one with argument
        /// </summary>
        public static float Error(this IUnboundMethod method, float argument)
        {
            return Math.Abs(method.CalculateUnbound(argument) - method.BaseMethod(argument));
        }

        private static float SimpleMaxError(IMemoizedMethod method)
        {
            var maxError = 0f;
            for (var i = 1; i < method.Values.Length; ++i)
            {
                var difference = Math.Abs(method.Values[i] - method.Values[i - 1]);
                if (difference > maxError)
                {
                    maxError = difference;
                }
            }
            return maxError;
        }

        private static float InterpolatedMaxError(IMemoizedMethod method)
        {
            var maxError = 0f;
            for (var i = 0; i < method.Values.Length - 1; ++i)
            {
                var argument = method.MinArgument + method.Step * 0.5f + i * method.Step;
                if (argument > method.MaxArgument)
                {
                    break;
                }
                var difference = Math.Abs(method.Calculate(argument) - method.BaseMethod(argument));
                if (difference > maxError)
                {
                    maxError = difference;
                }
            }
            return maxError;
        }

        public static float MeanError(this IMemoizedMethod method)
        {
            var sumError = 0f;
            for (var i = 0; i < method.Values.Length - 1; ++i)
            {
                var argument = method.MinArgument + method.Step * 0.5f + i * method.Step;
                if (argument > method.MaxArgument)
                {
                    break;
                }
                var error = Math.Abs(method.BaseMethod(argument) - method.Calculate(argument));
                if (!float.IsInfinity(error) && !float.IsNaN(error))
                {
                    sumError += error;
                }
            }
            return sumError / (method.Values.Length - 1);
        }
    }
}
