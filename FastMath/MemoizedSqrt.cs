using System;
using System.Runtime.CompilerServices;
using FastMath.Core;

namespace FastMath
{
    public sealed class MemoizedSqrt : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Sqrt(x));

        private readonly float _argumentMultiplier;

        private MemoizedSqrt(float minArgument, float maxArgument, int valuesCount)
        {
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedSqrt ConstructByValuesCount(float minArgument, float maxArgument, int valuesCount)
        {
            return new MemoizedSqrt(minArgument, maxArgument, valuesCount + 1);
        }

        public static MemoizedSqrt ConstructByMaxError(float minArgument, float maxArgument, float maxError)
        {
            var valuesCount = GetValuesCountByMaxError(minArgument, maxArgument, maxError);
            return new MemoizedSqrt(minArgument, maxArgument, valuesCount);
        }

        public static MemoizedSqrt ConstructByStep(float minArgument, float maxArgument, float step)
        {
            var valuesCount = (int)Math.Round((maxArgument - minArgument) / step) + 1;
            return new MemoizedSqrt(minArgument, maxArgument, valuesCount);
        }

        private static int GetValuesCountByMaxError(float minArgument, float maxArgument, float maxError)
        {
            double CalculateStep(float arg) => arg - Math.Pow(Math.Abs(Math.Sqrt(arg) - maxError), 2);

            var step = Math.Min(CalculateStep(minArgument), CalculateStep(maxArgument));
            step = Math.Abs(step);

            return (int) Math.Round((maxArgument - minArgument) / step + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int) ((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}
