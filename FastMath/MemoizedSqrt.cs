using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public sealed class MemoizedSqrt : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Sqrt(x));

        private readonly float _argumentMultiplier;

        public MemoizedSqrt(float minArgument, float maxArgument, int valuesCount)
        {
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedSqrt ConstructByMaxError(float minArgument, float maxArgument, float maxError)
        {
            var valuesCount = GetValuesCountByMaxError(minArgument, maxArgument, maxError);
            return new MemoizedSqrt(minArgument, maxArgument, valuesCount);
        }

        public static MemoizedSqrt ConstructByStep(float minArgument, float maxArgument, float step)
        {
            var valuesCount = (int)(Math.PI * 2 / step);
            return new MemoizedSqrt(minArgument, maxArgument, valuesCount);
        }

        private static int GetValuesCountByMaxError(float minArgument, float maxArgument, float maxError)
        {
            var a = maxError + Math.Sqrt(minArgument);
            var step = a * a - minArgument;
            return (int) Math.Round((maxArgument - minArgument) / step);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int) ((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}
