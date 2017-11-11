// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Help
{
    using System;

    internal static class CheckDouble
    {
        /// <summary>
        /// Throw exception, if the value is  NaN, PositiveInfinity or NegativeInfinity. 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArithmeticException"></exception>
        internal static void CheckInvalidValue(double value)
        {
            if (Double.IsNaN(value))
                throw new ArithmeticException("Value is NaN.");
            if (Double.IsNegativeInfinity(value))
                throw new ArithmeticException("Value is NegativeInfinity.");
            if (Double.IsPositiveInfinity(value))
                throw new ArithmeticException("Value is PositiveInfinity.");            
        }
    }
}
