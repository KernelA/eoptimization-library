// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;

    /// <summary>
    /// Parameters for BBBC method. 
    /// </summary>
    public struct BBBCParams
    {
        /// <summary>
        /// Alpha - restricts the search area for each points. 
        /// </summary>
        public double Alpha { get; private set; }

        /// <summary>
        /// Beta - the parameter that determines the effect of the "center of mass" and the best
        /// current solution.
        /// </summary>
        public double Beta { get; private set; }

        /// <summary>
        /// The number of iteration. 
        /// </summary>
        public int Imax { get; private set; }

        /// <summary>
        /// A value indicates, if parameters are set or not. 
        /// </summary>
        public bool IsParamsInit { get; }

        /// <summary>
        /// Number of points for searching on each iteration. 
        /// </summary>
        public int NP { get; private set; }

        /// <summary>
        /// Parameters for BBBC method. 
        /// </summary>
        /// <param name="NP">   
        /// Number of points for searching on each iteration. <paramref name="NP"/> &gt; 0.
        /// </param>
        /// <param name="Imax">  The number of iteration. <paramref name="Imax"/> &gt; 0. </param>
        /// <param name="Alpha">
        /// Restricts the search area for each points. <paramref name="Alpha"/> &gt; 0.
        /// </param>
        /// <param name="Beta"> 
        /// Parameter that determines the effect of the "center of mass" and the best current
        /// solution. <paramref name="Beta"/> in [0;1]
        /// </param>
        /// <exception cref="ArgumentException"> If conditions for parameters do not performed. </exception>
        public BBBCParams(int NP, int Imax, double Alpha, double Beta)
        {
            if (NP < 1)
                throw new ArgumentException($"{nameof(NP)} (actual value is {NP}) must be > 0.", nameof(NP));
            if (Imax < 1)
                throw new ArgumentException($"{nameof(Imax)} (actual value is {Imax})  must be > 0.", nameof(Imax));
            if (Alpha < 0)
                throw new ArgumentException($"{nameof(Alpha)}  (actual value is {Alpha}) must be > 0.", nameof(Alpha));
            if (Beta < 0 || Beta > 1)
                throw new ArgumentException($"{nameof(Beta)} (actual value is {Beta})  must be in range [0; 1].", nameof(Beta));

            IsParamsInit = true;

            this.NP = NP;
            this.Imax = Imax;
            this.Alpha = Alpha;
            this.Beta = Beta;
        }
    }
}