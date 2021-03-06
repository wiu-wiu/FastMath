﻿using System;
using System.Runtime.CompilerServices;
using FastMath.Core;
using static System.Math;

namespace FastMath
{
    public sealed class MemoizedLog : IMemoizedMethod
    {
        public float MinArgument { get; }

        public float MaxArgument { get; }

        public bool IsLinearInterpolated => false;

        public float Step { get; }

        public float Base { get; }

        public float[] Values { get; }

        public Func<float, float> BaseMethod => (x => (float) Log(x, Base));

        private readonly float _argumentMultiplier;

        private MemoizedLog(float minArgument, float maxArgument, float @base, int valuesCount)
        {
            MinArgument = minArgument;
            MaxArgument = maxArgument;
            Base = @base;
            Values = new float[valuesCount];
            Step = (MaxArgument - MinArgument) / (valuesCount - 1);
            this.ProduceValuesArray();
            _argumentMultiplier = 1 / Step;
        }

        public static MemoizedLog ConstructByValuesCount(float minArgument, float maxArgument, float @base, int valuesCount)
        {
            return new MemoizedLog(minArgument, maxArgument, @base, valuesCount + 1);
        }

        public static MemoizedLog ConstructByMaxError(float minArgument, float maxArgument, float @base, float maxError)
        {
            if (Abs(@base - 1) < 1e-3f)
            {
                throw new ArgumentException("Can't create log with base equal to 1");
            }
            var step = Abs(Pow(@base, maxError + Log(minArgument, @base)) - minArgument) * 0.95f;
            var valuesCount = (int)((maxArgument - minArgument) / step) + 2;
            return new MemoizedLog(minArgument, maxArgument, @base, valuesCount);
        }

        public static MemoizedLog ConstructByStep(float minArgument, float maxArgument, float @base, float step)
        {
            var valuesCount = (int)Math.Round((maxArgument - minArgument) / step);
            if (valuesCount == 1)
            {
                ++valuesCount;
            }
            return new MemoizedLog(minArgument, maxArgument, @base, valuesCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Calculate(float argument)
        {
            var index = (int)((argument - MinArgument) * _argumentMultiplier);
            return Values[index];
        }
    }
}