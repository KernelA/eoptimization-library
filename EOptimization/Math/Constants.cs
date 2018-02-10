// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math
{
    using System;

    /// <summary>
    /// Class for global constants. 
    /// </summary>
    public static class Constants
    {
        private static double ComputEps()
        {
            double eps, eps2, eps21;

            eps = 1.0;
            eps2 = eps * 0.5;

            eps21 = eps2 + 1.0;

            while (eps21 > 1.0)
            {
                eps = eps2;
                eps2 = eps * 0.5;
                eps21 = eps2 + 1.0;
            }
            return eps;
        }

        /// <summary>
        /// Machine's epsilon for double. 
        /// </summary>
        public static readonly double EPS;

        /// <summary>
        /// Exponent of machine epsilon. 
        /// </summary>
        public static readonly int EPS_EXPONENT;

        /// <summary>
        /// Number, which add for avoiding division by zero. 
        /// </summary>
        public static readonly double VALUE_AVOID_DIV_BY_ZERO;

        static Constants()
        {
            double eps = ComputEps();

            int exponnet = (int)Math.Floor(Math.Log10(eps));

            EPS_EXPONENT = exponnet;

            VALUE_AVOID_DIV_BY_ZERO = Math.Pow(10, exponnet / 2);

            EPS = eps;
        }
    }
}