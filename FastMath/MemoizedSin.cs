using System;
using System.Runtime.CompilerServices;
using FastMath.Core;

namespace FastMath
{
    public sealed class MemoizedSin : IUnboundMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Sin(x));

        private readonly float _argumentMultiplier;

        private readonly int _valuesCycleLength;

        public MemoizedSin(int valuesCount)
        {
            MinArgument = 0;
            MaxArgument = (float) (Math.PI * 2);
            Values = new float[valuesCount];
            Step = MaxArgument / (valuesCount - 1);
            this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
            _valuesCycleLength = Values.Length - 1;
        }

        public static MemoizedSin ConstructByMaxError(float maxError)
        {
            return new MemoizedSin(GetValuesCountByMaxError(maxError));
        }

        public static MemoizedSin ConstructByStep(float step)
        {
            var valuesCount = (int)Math.Round(Math.PI * 2 / step) + 1;
            return new MemoizedSin(valuesCount);
        }

        private static int GetValuesCountByMaxError(float maxError)
        {
            return (int) Math.Round(3 * Math.PI / maxError + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int) (argument * _argumentMultiplier);
            return Values[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float CalculateUnbound(float argument)
        {
            var multiplier = 1;
            if (argument < 0)
            {
                argument = -argument;
                multiplier = -1;
            }
            var index = (int)(argument * _argumentMultiplier);
            return multiplier * Values[index % _valuesCycleLength];
        }
    }
}
