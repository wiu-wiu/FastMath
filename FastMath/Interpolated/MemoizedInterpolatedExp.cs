using System;
using System.Runtime.CompilerServices;
using FastMath.Core;

namespace FastMath.Interpolated
{
    public sealed class MemoizedInterpolatedExp : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => true;

        public float Step { get; }

        public float[] Values { get; }

        public float Base { get; }

        public Func<float, float> BaseMethod => (x => (float)Math.Pow(Base, x));

        private readonly float _argumentMultiplier;

        private const float MinArgumentValue = 1e-3f;

        public MemoizedInterpolatedExp(float minArgument, float maxArgument, float @base, int valuesCount)
        {
            Base = @base;
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 2);
            Utils.ProduceValuesArray(this, 2);
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedInterpolatedExp ConstructByStep(float minArgument, float maxArgument, float @base, float step)
        {
            var valuesCount = (int)((maxArgument - minArgument) / step) + 3;
            return new MemoizedInterpolatedExp(minArgument, maxArgument, @base, valuesCount);
        }

        public static MemoizedInterpolatedExp ConstructByMaxError(float minArgument, float maxArgument, float @base, float maxError)
        {
            var step = GetStepByMaxError(minArgument, maxArgument, @base, maxError);
            return ConstructByStep(minArgument, maxArgument, @base, step);
        }

        private static float GetStepByMaxError(float minArgument, float maxArgument, float @base, float maxError)
        {
            float CalculateStep(float argument) => (float)Math.Abs(Math.Log(maxError + Math.Pow(@base, argument), @base) - argument);

            if (@base < 0)
            {
                throw new ArgumentException("Can't calculate values count: base is less then zero");
            }

            float step;
            if (@base < MinArgumentValue)
            {
                step = 1;
            }
            else if (@base < 1)
            {
                step = CalculateStep(minArgument);
            }
            else
            {
                step = CalculateStep(maxArgument);
            }
            step *= 10;
            return step;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var floatIndex = (argument - MinArgument) * _argumentMultiplier;
            var index = (int)floatIndex;
            var alpha = floatIndex - index;
            return (1 - alpha) * Values[index] + alpha * Values[index + 1];
        }
    }
}