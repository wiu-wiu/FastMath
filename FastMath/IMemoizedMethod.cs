using System;

namespace FastMath
{
    public interface IMemoizedMethod
    {
        float MinArgument { get; }
        float MaxArgument { get; }
        float Step { get; }
        float[] Values { get; }
        bool IsLinearInterpolationEnabled { get; }
        Func<float, float> BaseMethod { get; }
        float Calculate(float argument);
    }
}
