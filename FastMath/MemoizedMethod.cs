using System;
using System.Runtime.CompilerServices;
using FastMath.Core;
using FastMath.Interpolated;

namespace FastMath
{
    public sealed class MemoizedMethod : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod { get; }

        private readonly float _argumentMultiplier;

        public MemoizedMethod(Func<float, float> baseMethod, float minArgument, float maxArgument, int valuesCount, bool useParallelValueProduction = true)
        {
            BaseMethod = baseMethod;
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray(1, useParallelValueProduction);
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedInterpolatedPow ConstructByStep(float minArgument, float maxArgument, float power, float step)
        {
            var valuesCount = (int)((maxArgument - minArgument) / step) + 1;
            if (valuesCount == 1)
            {
                valuesCount = 2;
            }
            return new MemoizedInterpolatedPow(minArgument, maxArgument, power, valuesCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}
