using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public class MemoizedInterpolatedAtan : IUnboundMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => true;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Atan(x));

        private readonly float _argumentMultiplier;

        private static readonly float Max2DerivativeValue = (float) (3 * Math.Sqrt(3) / 8);

        private const float MinValue = (float) -Math.PI / 2;

        private const float MaxValue = (float) Math.PI / 2;

        private const int AdditionalValueCount = 2;

        public MemoizedInterpolatedAtan(int valuesCount, float maxArgument)
        {
            MinArgument = -maxArgument;
            MaxArgument = maxArgument;
            Values = new float[valuesCount];
            Step = 2 * MaxArgument / (valuesCount - AdditionalValueCount);
            this.ProduceValuesArray(AdditionalValueCount);
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedInterpolatedAtan ConstructByMaxError(float maxError)
        {
            if (maxError < 1e-3)
            {
                throw new ArgumentException("Max error is to small. 1e-3 is the best supported quality");
            }

            maxError *= 0.95f;
            var maxArgument = (float) Math.Tan(Math.PI / 2 - maxError);
            var step = (float) Math.Sqrt(8 * maxError / Max2DerivativeValue);
            var valuesCount = (int) (2 * maxArgument / step + AdditionalValueCount + 1);
            return new MemoizedInterpolatedAtan(valuesCount, maxArgument);
        }

        public static MemoizedInterpolatedAtan ConstructByStep(float step)
        {
            return ConstructByMaxError(Max2DerivativeValue * step * step / 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var floatIndex = (argument - MinArgument) * _argumentMultiplier;
            var index = (int) floatIndex;
            var alpha = floatIndex - index;
            try
            {
                return (1 - alpha) * Values[index] + alpha * Values[index + 1];
            }
            catch (Exception e)
            {

                return 0;
                throw;
            }
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float CalculateUnbound(float argument)
        {
            if (argument < MinArgument)
                return MinValue;
            if (argument > MaxArgument)
                return MaxValue;
            var floatIndex = (argument - MinArgument) * _argumentMultiplier;
            var index = (int) floatIndex;
            var alpha = floatIndex - index;
            return (1 - alpha) * Values[index] + alpha * Values[index + 1];
        }
    }
}