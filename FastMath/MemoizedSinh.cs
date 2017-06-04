using System;
using System.Runtime.CompilerServices;
using FastMath.Core;

namespace FastMath
{
    public class MemoizedSinh : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Sinh(x));

        private readonly float _argumentMultiplier;

        private MemoizedSinh(float minArgument, float maxArgument, int valuesCount)
        {
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedSinh ConstructByValuesCount(float minArgument, float maxArgument, int valuesCount)
        {
            return new MemoizedSinh(minArgument, maxArgument, valuesCount + 1);
        }

        public static MemoizedSinh ConstructByMaxError(float minArgument, float maxArgument, float maxError)
        {
            var valuesCount = GetValuesCountByMaxError(minArgument, maxArgument, maxError);
            return new MemoizedSinh(minArgument, maxArgument, valuesCount);
        }

        public static MemoizedSinh ConstructByStep(float minArgument, float maxArgument, float step)
        {
            var valuesCount = (int)Math.Round((maxArgument - minArgument) / step) + 1;
            if (valuesCount == 1)
            {
                ++valuesCount;
            }
            return new MemoizedSinh(minArgument, maxArgument, valuesCount);
        }

        private static int GetValuesCountByMaxError(float minArgument, float maxArgument, float maxError)
        {
            var argument = Math.Max(Math.Abs(minArgument), Math.Abs(maxArgument));
            var step = (float) Math.Abs(Math.Log(1 - maxError * Math.Pow(Math.E, -argument), Math.E));
            return (int) Math.Round((maxArgument - minArgument) / step) + 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int) ((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}