namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    ///  Interface for random generator normal distribution.
    /// </summary>
    public interface INormalGenerator : IRandomGeneartor<double>
    {
        /// <summary>
        /// Get random value which has normal distribution with mean equal <paramref name="Mean"/> and standart deviation equal <paramref name="StdDev"/>.
        /// </summary>
        /// <param name="Mean"></param>
        /// <param name="StdDev"></param>
        /// <returns></returns>
        double NRandVal(double Mean, double StdDev);
    }
}
