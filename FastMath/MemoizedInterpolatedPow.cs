using System;
using System.Runtime.CompilerServices;
using static System.Math;

namespace FastMath
{
    public sealed class MemoizedInterpolatedPow : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => true;

        public float Step { get; }

        public float[] Values { get; }

        public float Power { get; }

        public Func<float, float> BaseMethod => (x => (float)Pow(x, Power));

        private readonly float _argumentMultiplier;

        private const float MinArgumentValue = 1e-3f;

        public MemoizedInterpolatedPow(float minArgument, float maxArgument, float power, int valuesCount)
        {
            Power = power;
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Step = (MaxArgument - MinArgument) / valuesCount;
            MaxArgument += Step;
            Values = this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedInterpolatedPow ConstructByStep(float minArgument, float maxArgument, float power, float step)
        {
            var valuesCount = (int)((maxArgument - minArgument) / step);
            return new MemoizedInterpolatedPow(minArgument, maxArgument, power, valuesCount);
        }

        public static MemoizedInterpolatedPow ConstructByMaxError(float minArgument, float maxArgument, float power, float maxError)
        {
            var valuesCount = GetValuesCountByMaxError(minArgument, maxArgument, power, maxError);
            return new MemoizedInterpolatedPow(minArgument, maxArgument, power, valuesCount);
        }

        private static int GetValuesCountByMaxError(float minArgument, float maxArgument, float power, float maxError)
        {
            float CalculateStep(float argument)
            {
                var methodDerivative2 = (power - 1) * power * Pow(argument, power - 2);
                return (float) Pow(Abs(8 * maxError / methodDerivative2), 0.5);
            }

            if (Abs(power - 1) < MinArgumentValue)
            {
                return 1;
            }
            if (power < 0)
            {
                if ((minArgument < 0 && 0 < maxArgument)
                    || Abs(minArgument) < MinArgumentValue
                    || Abs(maxArgument) < MinArgumentValue)
                {
                    throw new ArgumentException("Can't calculate values count: power is less then zero and arguments interval containts zero");
                }
            }

            float step;
            if (power > 2)
            {
                var arg = Max(Abs(minArgument), Abs(maxArgument));
                step = CalculateStep(arg);
            }
            else
            {
                var arg = 0f;
                if (maxArgument < 0)
                {
                    arg = maxArgument;
                }
                else if (minArgument > 0)
                {
                    arg = minArgument;
                }
                step = CalculateStep(arg);
            }
            return (int)Round((maxArgument - minArgument) / step);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var floatIndex = (argument - MinArgument) * _argumentMultiplier;
            var index = (int) floatIndex;
            var alpha = floatIndex - index;
            return alpha * Values[index] + (1 - alpha) * Values[index + 1];
        }
    }
}
