using System;
using System.Runtime.CompilerServices;
using FastMath.Core;

namespace FastMath
{
    public class MemoizedTanh : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Tanh(x));

        private readonly float _argumentMultiplier;

        public MemoizedTanh(float minArgument, float maxArgument, int valuesCount)
        {
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedTanh ConstructByMaxError(float minArgument, float maxArgument, float maxError)
        {
            var valuesCount = GetValuesCountByMaxError(minArgument, maxArgument, maxError);
            return new MemoizedTanh(minArgument, maxArgument, valuesCount);
        }

        public static MemoizedTanh ConstructByStep(float minArgument, float maxArgument, float step)
        {
            var valuesCount = (int)Math.Round(Math.PI * 2 / step);
            return new MemoizedTanh(minArgument, maxArgument, valuesCount);
        }

        private static int GetValuesCountByMaxError(float minArgument, float maxArgument, float maxError)
        {
            var step = maxError * 0.95f;
            return (int) Math.Round((maxArgument - minArgument) / step) + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}