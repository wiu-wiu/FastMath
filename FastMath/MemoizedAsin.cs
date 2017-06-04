using System;
using System.Runtime.CompilerServices;
using FastMath.Core;

namespace FastMath
{
    public sealed class MemoizedAsin : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Asin(x));

        private readonly float _argumentMultiplier;

        public MemoizedAsin(int valuesCount)
        {
            MinArgument = -1;
            MaxArgument = 1;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedAsin ConstructByMaxError(float maxError)
        {
            var step = 1 - Math.Sin(Math.PI / 2 - maxError);
            step *= 0.95f;
            var valuesCount = (int)(2 / step) + 2;
            return new MemoizedAsin(valuesCount);
        }

        public static MemoizedAsin ConstructByStep(float step)
        {
            var valuesCount = (int)Math.Round(2 / step) + 1;
            return new MemoizedAsin(valuesCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}