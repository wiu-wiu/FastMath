using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public class MemoizedAtan : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float)Math.Atan(x));

        private readonly float _argumentMultiplier;

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
            var maxArgument = (float) Math.Tan(Math.PI / 2 - maxError);
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
    }
}
