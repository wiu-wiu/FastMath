namespace FastMath.Core
{
    /// <summary>
    /// Method that could be calculated with any argument, but with a bit lower performance
    /// </summary>
    public interface IUnboundMethod : IMemoizedMethod
    {
        /// <summary>
        /// Calculates method with any argument. Has a bit less performance than original calculate method
        /// Be care to use this method with very small or big arguments - possible accuracy problems
        /// </summary>
        float CalculateUnbound(float argument);
    }
}