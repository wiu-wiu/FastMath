using System;
using System.Runtime.CompilerServices;
using static System.Math;

namespace FastMath
{
    public class MemoizedExp : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => false;

        public float Step { get; }

        public float[] Values { get; }

        public float Base { get; }

        public Func<float, float> BaseMethod => (x => (float) Pow(Base, x));

        private readonly float _argumentMultiplier;

        private const float MinArgumentValue = 1e-5f;

        public MemoizedExp(float minArgument, float maxArgument, float @base, int valuesCount)
        {
            Base = @base;
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedExp ConstructByMaxError(float minArgument, float maxArgument, float @base, float maxError)
        {
            var valuesCount = GetValuesCountByMaxError(minArgument, maxArgument, @base, maxError);
            return new MemoizedExp(minArgument, maxArgument, @base, valuesCount);
        }

        public static MemoizedExp ConstructByStep(float minArgument, float maxArgument, float @base, float step)
        {
            var valuesCount = (int)((maxArgument - minArgument) / step);
            return new MemoizedExp(minArgument, maxArgument, @base, valuesCount);
        }

        private static int GetValuesCountByMaxError(float minArgument, float maxArgument, float @base, float maxError)
        {
            float CalculateStep(float argument) => (float) Abs(Log(maxError + Pow(@base, argument), @base) - argument);
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
            step *= 0.9f;
            return (int) Round((maxArgument - minArgument) / step);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int) ((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}
