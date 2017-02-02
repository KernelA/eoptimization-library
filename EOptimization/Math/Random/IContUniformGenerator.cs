namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// Interface for random generator uniform distribution.
    /// </summary>
    public interface IContUniformGenerator :IRandomGeneartor<double>
    {
        /// <summary>
        /// Get random value from continuous uniform distribution on [a;b].
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        double URandVal(double a, double b);

    }
}
