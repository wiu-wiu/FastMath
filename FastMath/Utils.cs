using System;
using System.Threading.Tasks;

namespace FastMath
{
    public static class Utils
    {
        internal static float[] ProduceValuesArray(this IMemoizedMethod memoizedMethod)
        {
            var minArgument = memoizedMethod.MinArgument;
            var maxArgument = memoizedMethod.MaxArgument;
            var step = memoizedMethod.Step;

            var valuesCount = (int) Math.Round((maxArgument - minArgument) / step) + 1;
            var values = new float[valuesCount];

            if (valuesCount < 10 * 1024 * 1024)
            {
                for (var i = 0; i < values.Length; ++i)
                {
                    var argument = i * step + minArgument;
                    values[i] = memoizedMethod.BaseMethod(argument);
                }
            }
            else
            {
                Parallel.For(0, values.Length, i =>
                {
                    var argument = i * step + minArgument;
                    values[i] = memoizedMethod.BaseMethod(argument);
                });
            }
            return values;
        }

        public static float MaxError(this IMemoizedMethod method)
        {
            return method.IsLinearInterpolationEnabled
                ? InterpolatedMaxError(method)
                : SimpleMaxError(method);
        }

        public static float Error(this IMemoizedMethod method, float argument)
        {
            return Math.Abs(method.Calculate(argument) - method.BaseMethod(argument));
        }

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
