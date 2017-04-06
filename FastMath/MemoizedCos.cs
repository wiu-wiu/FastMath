﻿using System;
using System.Runtime.CompilerServices;

namespace FastMath
{
    public sealed class MemoizedCos : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolationEnabled => false;

        public float Step { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Math.Cos(x));

        private readonly float _argumentMultiplier;

        public MemoizedCos(int valuesCount)
        {
            MinArgument = 0;
            MaxArgument = (float)(Math.PI * 2);
            Step = MaxArgument / valuesCount;
            Values = this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedCos ConstructByMaxError(float maxError)
        {
            return new MemoizedCos(GetValuesCountByMaxError(maxError));
        }

        public static MemoizedCos ConstructByStep(float step)
        {
            var valuesCount = (int)(Math.PI * 2 / step);
            return new MemoizedCos(valuesCount);
        }

        private static int GetValuesCountByMaxError(float maxError)
        {
            return (int)Math.Round(2 * Math.PI / maxError + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)(argument * _argumentMultiplier);
            return Values[index];
        }
    }
}
