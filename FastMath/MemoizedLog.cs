using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public sealed class MemoizedLog : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => false;

        public float Step { get; }

        public float Base { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Log(x, Base));

        private readonly float _argumentMultiplier;

        public MemoizedLog(float minArgument, float maxArgument, float @base, int valuesCount)
        {
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Step = (MaxArgument - MinArgument) / valuesCount;
            Base = @base;
            Values = Utils.ProduceValuesArray(this);
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedLog ConstructByMaxError(float minArgument, float maxArgument, float @base, float maxError)
        {
            var step = Math.Pow(@base, maxError + Math.Log(minArgument, @base)) - minArgument;
            return ConstructByStep(minArgument, maxArgument, @base, (float) step);
        }

        public static MemoizedLog ConstructByStep(float minArgument, float maxArgument, float @base, float step)
        {
            var valuesCount = (int)((maxArgument - minArgument) / step);
            return new MemoizedLog(minArgument, maxArgument, @base, valuesCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}