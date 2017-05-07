using System;
using System.Runtime.CompilerServices;
using FastMath.Core;

namespace FastMath.Interpolated
{
    public sealed class MemoizedInterpolatedPow : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => true;

        public float Step { get; }

        public float[] Values { get; }

        public float Power { get; }

        public Func<float, float> BaseMethod => (x => (float)Math.Pow(x, Power));

        private readonly float _argumentMultiplier;

        private const float MinArgumentValue = 1e-3f;

        public MemoizedInterpolatedPow(float minArgument, float maxArgument, float power, int valuesCount)
        {
            Power = power;
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 2);
            this.ProduceValuesArray(2);
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedInterpolatedPow ConstructByStep(float minArgument, float maxArgument, float power, float step)
        {
            var valuesCount = (int)((maxArgument - minArgument) / step) + 2;
            return new MemoizedInterpolatedPow(minArgument, maxArgument, power, valuesCount);
        }

        public static MemoizedInterpolatedPow ConstructByMaxError(float minArgument, float maxArgument, float power, float maxError)
        {
            var step = GetStepByMaxError(minArgument, maxArgument, power, maxError);
            var valuesCount = (int)((maxArgument - minArgument) / step) + 3;
            return new MemoizedInterpolatedPow(minArgument, maxArgument, power, valuesCount);
        }

        private static float GetStepByMaxError(float minArgument, float maxArgument, float power, float maxError)
        {
            float CalculateStep(float argument)
            {
                var methodDerivative2 = (power - 1) * power * Math.Pow(argument, power - 2);
                return (float) Math.Pow(Math.Abs(8 * maxError / methodDerivative2), 0.5) * 0.9f;
            }

            if (Math.Abs(power - 1) < MinArgumentValue)
            {
                return 1;
            }
            if (power < 0)
            {
                if ((minArgument < 0 && 0 < maxArgument)
                    || Math.Abs(minArgument) < MinArgumentValue
                    || Math.Abs(maxArgument) < MinArgumentValue)
                {
                    throw new ArgumentException("Can't calculate values count: power is less then zero and arguments interval containts zero");
                }
            }

            float step;
            if (power >= 2)
            {
                var arg = Math.Max(Math.Abs(minArgument), Math.Abs(maxArgument));
                step = CalculateStep(arg);
            }
            else
            {
                var arg = Math.Min(Math.Abs(minArgument), Math.Abs(maxArgument));
                step = CalculateStep(arg);
            }
            return step;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var floatIndex = (argument - MinArgument) * _argumentMultiplier;
            var index = (int) floatIndex;
            var alpha = floatIndex - index;
            return (1 - alpha) * Values[index] + alpha * Values[index + 1];
        }
    }
}
