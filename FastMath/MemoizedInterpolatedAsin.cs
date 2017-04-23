using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public sealed class MemoizedInterpolatedAsin : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => true;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Asin(x));

        private readonly float _argumentMultiplier;

        public MemoizedInterpolatedAsin(int valuesCount)
        {
            MinArgument = -1;
            MaxArgument = 1;
            Step = (MaxArgument - MinArgument) / valuesCount;
            Values = Utils.ProduceValuesArray(this);
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedInterpolatedAsin ConstructByMaxError(float maxError)
        {
            return ConstructByStep((float) Math.Pow(3 * maxError, 2));
        }

        public static MemoizedInterpolatedAsin ConstructByStep(float step)
        {
            var valuesCount = (int) (2 / step);
            return new MemoizedInterpolatedAsin(valuesCount);
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