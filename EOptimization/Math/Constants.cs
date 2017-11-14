namespace EOpt.Math
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for global constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Number, which add for avoiding division by zero.
        /// </summary>
        public static readonly double ValueForAvoidDivByZero;

        /// <summary>
        /// Machine's epsilon for double.
        /// </summary>
        public static readonly double Eps;

        /// <summary>
        /// Exponent of  machine epsilon.
        /// </summary>
        public static readonly int EpsExponent;

        static Constants()
        {
            double eps = ComputEps();

            int exponnet = (int)Math.Floor(Math.Log10(eps));

            EpsExponent = exponnet;

            ValueForAvoidDivByZero = Math.Pow(10, exponnet / 2);

            Eps = eps;
        }

        static double ComputEps()
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
    }
}
