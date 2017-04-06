using System;
using System.Runtime.CompilerServices;
using static System.Math;

namespace FastMath
{
    public sealed class MemoizedPow : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => false;

        public float Step { get; }

        public float[] Values { get; }

        public float Power { get; }

        public Func<float, float> BaseMethod => (x => (float) Pow(x, Power));

        private readonly float _argumentMultiplier;

        private const float MinArgumentValue = 1e-3f;

        public MemoizedPow(float minArgument, float maxArgument, float power, int valuesCount)
        {
            Power = power;
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Step = (MaxArgument - MinArgument) / valuesCount;
            Values = this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedPow ConstructByMaxError(float minArgument, float maxArgument, float power, float maxError)
        {
            var valuesCount = GetValuesCountByMaxError(minArgument, maxArgument, power, maxError);
            return new MemoizedPow(minArgument, maxArgument, power, valuesCount);
        }

        public static MemoizedPow ConstructByStep(float minArgument, float maxArgument, float power, float step)
        {
            var valuesCount = (int)(PI * 2 / step);
            return new MemoizedPow(minArgument, maxArgument, power, valuesCount);
        }

        private static int GetValuesCountByMaxError(float minArgument, float maxArgument, float power, float maxError)
        {
            float CalculateStep(float argument) => (float) Abs(argument - Pow(Pow(argument, power) - maxError, 1 / power));

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
            if (power > 1)
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
            return (int) Round((maxArgument - minArgument) / step);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}
