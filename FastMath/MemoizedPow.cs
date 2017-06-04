using System;
using System.Runtime.CompilerServices;
using FastMath.Core;
using static System.Math;

namespace FastMath
{
    public sealed class MemoizedPow : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => false;

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
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedPow ConstructByMaxError(float minArgument, float maxArgument, float power, float maxError)
        {
            var valuesCount = GetValuesCountByMaxError(minArgument, maxArgument, power, maxError);
            return new MemoizedPow(minArgument, maxArgument, power, valuesCount);
        }

        public static MemoizedPow ConstructByStep(float minArgument, float maxArgument, float power, float step)
        {
            var valuesCount = (int)Math.Round((maxArgument - minArgument) / step) + 1;
            if (valuesCount == 1)
            {
                valuesCount = 2;
            }
            return new MemoizedPow(minArgument, maxArgument, power, valuesCount);
        }

        private static int GetValuesCountByMaxError(float minArgument, float maxArgument, float power, float maxError)
        {
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
                step = (float) Abs(Abs(Pow(Pow(arg, power) + maxError, 1 / power)) - arg) * 0.9f;
            }
            else
            {
                var arg = Min(Abs(minArgument), Abs(maxArgument));
                var partialResult = Abs(Pow(arg, power) - maxError);
                if (partialResult < 1e-5f)
                {
                    partialResult += 1e-3f;
                }
                step = (float) Abs(Pow(partialResult, 1 / power) - arg) * 0.8f;
            }
            return (int) Round((maxArgument - minArgument) / step) + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}
