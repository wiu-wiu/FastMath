using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public sealed class MemoizedAcos : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Acos(x));

        private readonly float _argumentMultiplier;

        public MemoizedAcos(int valuesCount)
        {
            MinArgument = -1;
            MaxArgument = 1;
            Step = (MaxArgument - MinArgument) / valuesCount;
            Values = Utils.ProduceValuesArray(this);
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedAcos ConstructByMaxError(float maxError)
        {
            return ConstructByStep((float)(1 - Math.Sin(Math.PI / 2 - maxError)));
        }

        public static MemoizedAcos ConstructByStep(float step)
        {
            var valuesCount = (int)(2 / step);
            return new MemoizedAcos(valuesCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}