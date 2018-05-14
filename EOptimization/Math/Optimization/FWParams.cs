// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;

    /// <summary>
    /// Parameters for Fireworks method. 
    /// </summary>
    public struct FWParams
    {
        /// <summary>
        /// Minimum number of debris for each charge. 
        /// </summary>
        public int Smin { get; private set; }

        /// <summary>
        /// The maximum amplitude of explosion. 
        /// </summary>
        public double Amax { get; private set; }

        /// <summary>
        /// Maximum number of debris for each charge. 
        /// </summary>
        public int Smax { get; private set; }

        /// <summary>
        /// The number of iteration. 
        /// </summary>
        public int Imax { get; private set; }

        /// <summary>
        /// A value indicates, if parameters are set or not. 
        /// </summary>
        public bool IsParamsInit { get; }

        /// <summary>
        /// Parameter affecting the number of debris. 
        /// </summary>
        public double M { get; private set; }

        /// <summary>
        /// Number of charges on each iteration. 
        /// </summary>
        public int NP { get; private set; }

        /// <summary>
        /// Parameters for Fireworks method. 
        /// </summary>
        /// <param name="NP">   
        /// Number of charges on each iteration. <paramref name="NP"/> &gt; 0.
        /// </param>
        /// <param name="Imax">  The number of iteration. <paramref name="Imax"/> &gt; 0. </param>
        /// <param name="M">    
        /// Parameter influences on the number of debris for each charge. <paramref name="M"/> &gt; 0.
        /// </param>
        /// <param name="Smin">
        /// Parameter restricts the number of debris from below. <paramref name="Smin"/> in (0;1),
        /// <paramref name="Smin"/> &lt; <paramref name="Smax"/>.
        /// </param>
        /// <param name="Smax"> 
        /// Parameter restricts the number of debris from above. <paramref name="Smax"/> in (0;1),
        /// <paramref name="Smax"/> &gt; <paramref name="Smin"/>.
        /// </param>
        /// <param name="Amax"> 
        /// Maximum amplitude of explosion. <paramref name="Amax"/> &gt; 0.
        /// </param>
        /// <exception cref="ArgumentException"> If conditions for parameters do not performed. </exception>
        public FWParams(int NP, int Imax, double M, int Smin, int Smax, double Amax)
        {
            if (Imax < 1)
            {
                throw new ArgumentException($"{nameof(Imax)} (actual value is {Imax})  must be > 0.", nameof(Imax));
            }

            if (NP < 1)
            {
                throw new ArgumentException($"{nameof(NP)} (actual value is {NP})  must be > 0.", nameof(NP));
            }

            if (M <= 0)
            {
                throw new ArgumentException($"{nameof(M)} (actual value is {M}) must be > 0.", nameof(M));
            }

            if (Smin < 1 || Smax < 1 || Smin > Smax)
            {
                throw new ArgumentException($"{nameof(Smin)} (actual value is {Smin}) and {nameof(Smax)} (actual value is {Smax}) must be in (0;1)," +
                    $" {nameof(Smin)} (actual value is{Smin}) must be < {nameof(Smax)} (actual value is {Smax}).");
            }

            if (Amax <= 0)
            {
                throw new ArgumentException($"{nameof(Amax)} (actual value is {Amax}) must be > 0.", nameof(Amax));
            }

            IsParamsInit = true;
            this.NP = NP;
            this.Imax = Imax;
            this.M = M;
            this.Amax = Amax;
            this.Smin = Smin;
            this.Smax = Smax;
        }
    }
}