using System;

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

            for (var i = 0; i < values.Length; ++i)
            {
                var argument = i * step + minArgument;
                values[i] = memoizedMethod.BaseMethod(argument);
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
            var i = 0;
            var maxI = 0;
            for (var argument = method.MinArgument + method.Step * 0.5f;
                argument < method.MaxArgument;
                argument += method.Step, ++i)
            {
                var difference = Math.Abs(method.Calculate(argument) - method.BaseMethod(argument));
                if (difference > maxError)
                {
                    maxError = difference;
                    maxI = i;
                }
            }
            return maxError;
        }

        public static float MeanError(this IMemoizedMethod method)
        {
            var step = method.Step;
            var startArgument = method.MinArgument + step * 0.5f;
            var sumError = 0f;
            for (var argument = startArgument; argument <= method.MaxArgument; argument += step)
            {
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
