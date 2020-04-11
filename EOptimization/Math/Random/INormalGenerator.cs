// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Random
{
    /// <summary>
    /// Interface for a random generator of the normal distribution.
    /// </summary>
    public interface INormalGen
    {
        /// <summary>
        /// Get random value which has normal distribution with mean is equal <paramref name="Mean"/>
        /// and standard deviation is equal <paramref name="StdDev"/>.
        /// </summary>
        /// <param name="Mean">   Mean value. </param>
        /// <param name="StdDev"> Standard deviation. </param>
        /// <returns></returns>
        double NRandVal(double Mean, double StdDev);
    }
}