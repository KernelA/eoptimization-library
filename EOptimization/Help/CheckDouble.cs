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
        /// <param name="Value"></param>
        /// <exception cref="ArithmeticException"></exception>
        public static void CheckInvalidValue(double Value)
        {
            if (Double.IsNaN(Value))
                throw new ArithmeticException("Value is NaN.");
            if (Double.IsNegativeInfinity(Value))
                throw new ArithmeticException("Value is NegativeInfinity.");
            if (Double.IsPositiveInfinity(Value))
                throw new ArithmeticException("Value is PositiveInfinity.");            
        }
    }
}
