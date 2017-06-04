using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public class MemoizedAtan2
    {
        private const float Pi = (float) Math.PI;

        private const float MinArgumentValue = 1e-3f;

        private readonly MemoizedAtan _atan;

        private MemoizedAtan2(int valuesCount, float maxArgument)
        {
            _atan = MemoizedAtan.ConstructByValuesCount(valuesCount, maxArgument);
        }

        public MemoizedAtan2(MemoizedAtan atan)
        {
            _atan = atan;
        }

        public static MemoizedAtan2 ConstructByValuesCount(int valuesCount, float maxArgument)
        {
            return new MemoizedAtan2(valuesCount, maxArgument);
        }

        public static MemoizedAtan2 ConstructByMaxError(float maxError)
        {
            return new MemoizedAtan2(MemoizedAtan.ConstructByMaxError(maxError));
        }

        public static MemoizedAtan2 ConstructByStep(float step)
        {
            return new MemoizedAtan2(MemoizedAtan.ConstructByStep(step));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float y, float x)
        {
            if (Math.Abs(x) < MinArgumentValue)
            {
                return y < 0 ? -Pi / 2 : Pi / 2;
            }
            if (x > 0)
            {
                return _atan.CalculateUnbound(y / x);
            }
            else
            {
                return y < 0 
                    ? _atan.CalculateUnbound(y / x) - Pi
                    : _atan.CalculateUnbound(y / x) + Pi;
            }
        }
    }
}
