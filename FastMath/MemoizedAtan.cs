using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public class MemoizedAtan : IUnboundMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float)Math.Atan(x));

        private readonly float _argumentMultiplier;

        private const float MinValue = (float) -Math.PI / 2;

        private const float MaxValue = (float) Math.PI / 2;

        public MemoizedAtan(int valuesCount, float maxArgument)
        {
            MinArgument = -maxArgument;
            MaxArgument = maxArgument;
            Step = MaxArgument / valuesCount;
            Values = this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedAtan ConstructByMaxError(float maxError)
        {
            if (maxError < 1e-3)
            {
                throw new ArgumentException("Max error is to small. Try to use interpolated atan.");
            }
            maxError *= 0.95f;
            var maxArgument = (float)Math.Tan(Math.PI / 2 - maxError);
            var valuesCount = (int) (maxArgument / maxError + 1);
            return new MemoizedAtan(valuesCount, maxArgument);
        }

        public static MemoizedAtan ConstructByStep(float step)
        {
            return ConstructByMaxError((float) Math.Atan(step));
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
            if (argument < MinArgument)
                return MinValue;
            if (argument > MaxArgument)
                return MaxValue;
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}
