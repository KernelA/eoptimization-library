// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math
{
    using Random;

    /// <summary>
    /// Clipping methods. 
    /// </summary>
    public static class ClampDouble
    {
        /// <summary>
        /// If <paramref name="Value"/> out of range [ <paramref name="MinValue"/>;
        /// <paramref name="MaxValue"/>], then clipping to the boundary.
        /// </summary>
        /// <param name="Value">   </param>
        /// <param name="MinValue"> A minimum value of the range. </param>
        /// <param name="MaxValue"> A maximum value of the range. </param>
        /// <returns>
        /// <para> <paramref name="MaxValue"/> if <paramref name="Value"/> greater than <paramref name="MaxValue"/>. </para>
        /// <para> <paramref name="MinValue"/> if <paramref name="Value"/> less than <paramref name="MinValue"/>. </para>
        /// <para> Otherwise <paramref name="Value"/>. </para>
        /// </returns>
        public static double Clamp(double Value, double MinValue, double MaxValue)
        {
            if (Value < MinValue)
            {
                return MinValue;
            }
            else if (Value > MaxValue)
            {
                return MaxValue;
            }
            else
            {
                return Value;
            }
        }

        /// <summary>
        /// If <paramref name="Value"/> out of range [ <paramref name="MinValue"/>;
        /// <paramref name="MaxValue"/>], then clipping to a random value in the range.
        /// </summary>
        /// <param name="Value">     </param>
        /// <param name="MinValue">   A minimum value of the range. </param>
        /// <param name="MaxValue">   A maximum value of the range. </param>
        /// <param name="UniformGen"> A random generator. </param>
        /// <returns>
        /// <para>
        /// A random value in the range [ <paramref name="MinValue"/>; <paramref name="MaxValue"/>]
        /// if <paramref name="Value"/> greater than <paramref name="MaxValue"/> or less than <paramref name="MinValue"/>.
        /// </para>
        /// <para> Otherwise <paramref name="Value"/>. </para>
        /// </returns>
        public static double RandomClamp(double Value, double MinValue, double MaxValue, IContUniformGen UniformGen)
        {
            if (Value < MinValue || Value > MaxValue)
            {
                return UniformGen.URandVal(MinValue, MaxValue);
            }
            else
            {
                return Value;
            }
        }
    }
}