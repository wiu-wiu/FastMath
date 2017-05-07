using System;

namespace FastMath.Core
{
    public interface IMemoizedMethod
    {
        float MinArgument { get; }
        float MaxArgument { get; }
        float Step { get; }
        float[] Values { get; }
        bool IsLinearInterpolated { get; }
        Func<float, float> BaseMethod { get; }

        /// <summary>
        /// Caclulate method. Note that argument should be in range from MinArgument to MaxArgument.
        /// </summary>
        float Calculate(float argument);
    }
}
