// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Help
{
    using System;

    enum DoubleTypeValue { NaN, PosInf, NegInf, Valid };

    internal static class CheckDouble
    {
        /// <summary>
        /// Get type  value of the <paramref name="Value"/>. 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>
        /// <see cref="DoubleTypeValue.NaN"/> if <paramref name="Value"/> is NaN.
        /// <see cref="DoubleTypeValue.PosInf"/> if <paramref name="Value"/> is PositiveInfinity.
        /// <see cref="DoubleTypeValue.NegInf"/> if <paramref name="Value"/> is NegativeInfinity.
        /// Otherwise, <see cref="DoubleTypeValue.Valid"/>.
        /// </returns>
        public static DoubleTypeValue GetTypeValue(double Value)
        {
            if (Double.IsNaN(Value))
                return DoubleTypeValue.NaN;
            else if (Double.IsNegativeInfinity(Value))
                return DoubleTypeValue.NegInf;
            else if (Double.IsPositiveInfinity(Value))
                return DoubleTypeValue.PosInf;
            else
                return DoubleTypeValue.Valid;
        }
    }
}