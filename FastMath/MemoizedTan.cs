using System;
using System.Runtime.CompilerServices;
using FastMath.Core;

namespace FastMath
{
    public sealed class MemoizedTan : IUnboundMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Tan(x));

        private readonly float _argumentMultiplier;

        private readonly int _valuesCycleLength;

        public MemoizedTan(int valuesCount)
        {
            MinArgument = (float) -Math.PI / 2;
            MaxArgument = (float) Math.PI / 2;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray();
            Values[0] = (float) Math.Tan(-Math.PI / 2);
            Values[Values.Length - 1] = (float)Math.Tan(Math.PI / 2);
            _argumentMultiplier = 1 / Step;
            _valuesCycleLength = Values.Length - 1;
        }

        public static MemoizedTan ConstructByStep(float step)
        {
            var valuesCount = (int) (Math.PI / step) + 1;
            return new MemoizedTan(valuesCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
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
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return multiplier * Values[index % _valuesCycleLength];
        }
    }
}