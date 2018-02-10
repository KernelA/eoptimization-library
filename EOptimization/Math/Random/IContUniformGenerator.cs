// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Random
{
    /// <summary>
    /// Interface for a random generator of the uniform distribution. 
    /// </summary>
    public interface IContUniformGen
    {
        /// <summary>
        /// Get random value from continuous uniform distribution on [ <paramref name="LowBound"/>; <paramref name="UpperBound"/>]. 
        /// </summary>
        /// <param name="LowBound">  </param>
        /// <param name="UpperBound"></param>
        /// <returns></returns>
        double URandVal(double LowBound, double UpperBound);
    }
}