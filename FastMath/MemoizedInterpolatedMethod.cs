using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public sealed class MemoizedInterpolatedMethod : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => true;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod { get; }

        private readonly float _argumentMultiplier;

        public MemoizedInterpolatedMethod(Func<float, float> baseMethod, float minArgument, float maxArgument, int valuesCount)
        {
            BaseMethod = baseMethod;
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Step = (MaxArgument - MinArgument) / valuesCount;
            Values = this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedInterpolatedPow ConstructByStep(float minArgument, float maxArgument, float power, float step)
        {
            var valuesCount = (int)((maxArgument - minArgument) / step);
            return new MemoizedInterpolatedPow(minArgument, maxArgument, power, valuesCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var floatIndex = (argument - MinArgument) * _argumentMultiplier;
            var index = (int)floatIndex;
            var alpha = floatIndex - index;
            return alpha * Values[index] + (1 - alpha) * Values[index + 1];
        }
    }
}
