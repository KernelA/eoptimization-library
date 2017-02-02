namespace EOpt.Math.Optimization
{
    using System;


    /// <summary>
    /// Parameters for BBBC method. <see cref="BBBCOptimizer"/>
    /// </summary>
    public class BBBCParams
    {
        /// <summary>
        /// Number of points for searching on each iteration.
        /// </summary>
        public int NP { get; private set; }


        /// <summary>
        /// Alpha - restricts the search area for each points.
        /// </summary>
        public double Alpha { get; private set; }


        /// <summary>
        /// Beta - parameter that determines the effect of  "center of mass" and best current solution.
        /// </summary>
        public double Beta { get; private set; }


        /// <summary>
        /// Max iteration.
        /// </summary>
        public int Imax { get; private set; }



        /// <summary>
        /// Parameters for BBBC method.
        /// </summary>
        /// <param name="NP">Number of points for searching on each iteration.</param>
        /// <param name="Imax">Max iteration.</param>
        /// <param name="alpha">Restricts the search area for each points. <paramref name="alpha"/> > 0.</param>
        /// <param name="beta">Parameter that determines the effect of  "center of mass" and best current solution. <paramref name="beta"/> in [0;1]</param>
        /// <exception cref="ArgumentException"></exception>
        public BBBCParams(int NP, int Imax, double alpha, double beta)
        {
            if (NP < 1)
                throw new ArgumentException(nameof(NP) + " must be > 0.", nameof(NP));
            if (Imax < 1)
                throw new ArgumentException(nameof(Imax) + " must be > 0.", nameof(Imax));
            if (alpha < 0)
                throw new ArgumentException(nameof(alpha) + "must be > 0", nameof(alpha));
            if (beta < 0 || beta > 1)
                throw new ArgumentException(nameof(beta) + " in [0; 1]", nameof(beta));

            this.NP = NP;
            this.Imax = Imax;
            this.Alpha = alpha;
            this.Beta = beta;
        }
    }
}
