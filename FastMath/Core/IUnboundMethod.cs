namespace FastMath.Core
{
    public interface IUnboundMethod : IMemoizedMethod
    {
        float CalculateUnbound(float argument);
    }
}