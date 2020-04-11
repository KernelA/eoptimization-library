﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math
{
    using System;

    /// <summary>
    /// </summary>
    public static class CmpDouble
    {
        /// <summary>
        /// Compares two doubles and determines if they are equal within the specified precision. The
        /// precision is calculating as: Constants.Eps * 10 ^ <paramref name="Exponent"/>.
        /// </summary>
        /// <param name="Value1">  </param>
        /// <param name="Value2">  </param>
        /// <param name="Exponent"> The exponent of 10. It must be &gt;= 0. </param>
        /// <returns></returns>
        public static bool AlmostEqual(double Value1, double Value2, int Exponent = 0)
        {
            if (Exponent < 0)
            {
                throw new ArgumentException($"{nameof(Exponent)} must be >= 0. Actual value: {Exponent}.", nameof(Exponent));
            }

            if (Double.IsInfinity(Value1) || Double.IsInfinity(Value2))
            {
                return Value1.CompareTo(Value2) == 0 ? true : false;
            }

            if (Double.IsNaN(Value1) || Double.IsNaN(Value2))
            {
                return false;
            }

            return Math.Abs(Value1 - Value2) < Math.Pow(10, Exponent) * Constants.EPS * (1.0 + Math.Max(Math.Abs(Value1), Math.Abs(Value2)));
        }

        /// <summary>
        /// Usual comparison of double. It needed for the non-dominated sorting.
        /// </summary>
        /// <param name="Value1"></param>
        /// <param name="Value2"></param>
        /// <returns></returns>
        internal static int DoubleCompare(double Value1, double Value2)
        {
            if (Value1 < Value2)
            {
                return -1;
            }
            else if (Value1 > Value2)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}