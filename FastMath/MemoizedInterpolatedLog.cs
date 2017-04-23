using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public sealed class MemoizedInterpolatedLog : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => true;

        public float Step { get; }

        public float Base { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Log(x, Base));

        private readonly float _argumentMultiplier;

        public MemoizedInterpolatedLog(float minArgument, float maxArgument, float @base, int valuesCount)
        {
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Step = (MaxArgument - MinArgument) / valuesCount;
            Base = @base;
            Values = Utils.ProduceValuesArray(this);
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedInterpolatedLog ConstructByMaxError(float minArgument, float maxArgument, float @base, float maxError)
        {
            var step = Math.Sqrt(8 * maxError) * minArgument;
            return ConstructByStep(minArgument, maxArgument, @base, (float)step);
        }

        public static MemoizedInterpolatedLog ConstructByStep(float minArgument, float maxArgument, float @base, float step)
        {
            var valuesCount = (int)((maxArgument - minArgument) / step);
            return new MemoizedInterpolatedLog(minArgument, maxArgument, @base, valuesCount);
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