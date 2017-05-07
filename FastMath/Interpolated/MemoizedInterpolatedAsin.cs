using System;
using System.Runtime.CompilerServices;
using FastMath.Core;

namespace FastMath.Interpolated
{
    public sealed class MemoizedInterpolatedAsin : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => true;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Asin(x));

        private readonly float _argumentMultiplier;

        public MemoizedInterpolatedAsin(int valuesCount)
        {
            MinArgument = -1;
            MaxArgument = 1;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 2);
            this.ProduceValuesArray(2);
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedInterpolatedAsin ConstructByMaxError(float maxError)
        {
            return ConstructByStep((float)Math.Pow(3 * maxError, 2));
        }

        public static MemoizedInterpolatedAsin ConstructByStep(float step)
        {
            var valuesCount = (int)(2 / step) + 3;
            return new MemoizedInterpolatedAsin(valuesCount);
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