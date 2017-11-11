// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;


    /// <summary>
    /// Parameters for Fireworks method. <see cref="FWOptimizer"/>
    /// </summary>
    public class FWParams
    { 
        /// <summary>
        /// Number of charges on each iteration.
        /// </summary>
        public int NP { get; private set; }

        /// <summary>
        /// Parameter affecting  the number of debris.
        /// </summary>
        public double M { get; private set; }

        /// <summary>
        /// The number of iteration.
        /// </summary>
        public int Imax { get; private set; }

        /// <summary>
        /// Parameter restricts the number of debris from below.
        /// </summary>
        public double Alpha { get; private set; }

        /// <summary>
        /// Parameter restricts the number of debris from below.
        /// </summary>
        public double Beta { get; private set; }

        /// <summary>
        /// The maximum amplitude of explosion.
        /// </summary>
        public double Amax { get; private set; }

        /// <summary>
        /// Distance between points.
        /// </summary>
        public Func<PointND, PointND, double> DistanceFunction { get; private set; }

        /// <summary>
        /// Parameters for Fireworks method. <seealso cref="FWOptimizer"/>.
        /// </summary>
        /// <param name="NP">Number of charges on each iteration. <paramref name="NP"/> > 0.</param>
        /// <param name="Imax">The number of iteration. <paramref name="Imax"/> > 0.</param>
        /// <param name="DistanceFunction">Distance between points.</param>
        /// <param name="M">Number of debris for each charge. <paramref name="M"/> > 0.</param>
        /// <param name="Alpha">Parameter restricts the number of debris  from below. <paramref name="Alpha"/> in (0;1),  <paramref name="Alpha"/> &lt; <paramref name="Beta"/>.</param>
        /// <param name="Beta">Parameter restricts the number of debris  from above. <paramref name="Beta"/> in (0;1), <paramref name="Beta"/> &gt; <paramref name="Alpha"/>.</param>
        /// <param name="Amax">Maximum amplitude of explosion. <paramref name="Amax"/> > 0.</param>
        /// <exception cref="ArgumentException">If conditions for parameters do not performed.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="DistanceFunction"/> is null.</exception>
        public FWParams(int NP, int Imax, Func<PointND, PointND, double> DistanceFunction, double M, double Alpha = 0.1, double Beta = 0.9, double Amax = 40)
        {
            if (NP < 1 || Imax < 1)
                throw new ArgumentException($"{nameof(Imax)} must be > 0.", nameof(Imax));
            if (M <= 0)
                throw new ArgumentException($"{nameof(M)} must be > 0.", nameof(M));
            if (Alpha <= 0 || Beta >= 1 || Alpha >= Beta)
                throw new ArgumentException($"{nameof(Alpha)} ({Alpha}) and {nameof(Beta)} ({Beta}) must be in (0;1), {nameof(Alpha)} ({Alpha}) < {nameof(Beta)}({Beta}).");
            if (Amax <= 0)
                throw new ArgumentException($"{nameof(Amax)} must be > 0.", nameof(Amax));
            if (DistanceFunction == null)
                throw new ArgumentNullException(nameof(DistanceFunction));

            this.NP = NP;
            this.Imax = Imax;
            this.M = M;
            this.Amax = Amax;
            this.Alpha = Alpha;
            this.Beta = Beta;
            this.DistanceFunction = DistanceFunction;
        }
    }

}
