namespace EOpt.Math.Optimization
{
    using System;

    /// <summary>
    /// Parameters for GEM method. <see cref="GEMOptimizer"/>
    /// </summary>
    public class GEMParams
    {
        /// <summary>
        /// Number of grenades on each iteration.
        /// </summary>
        public int NGrenade { get; private set; }

        /// <summary>
        /// Number of shrapnel for each grenade.
        /// </summary>
        public int NShrapnel { get; private set; }

        /// <summary>
        /// Max iteration.
        /// </summary>
        public int Imax { get; private set; }

        /// <summary>
        /// The number of desired minimums.
        /// </summary>
        public int DesiredMin { get; private set; }

        /// <summary>
        /// The initial radius of the grenade territory.
        /// </summary>
        public double InitRadiusGrenade { get; private set; }

        /// <summary>
        /// Minimum value of exponent for reduce the radius of explosion.
        /// </summary>
        public double Mmin { get; private set; }

        /// <summary>
        /// Maximum value of exponent for reduce the radius of explosion.
        /// </summary>
        public double Mmax { get; private set; }

        /// <summary>
        /// Probability of collision.
        /// </summary>
        public double Pts { get; private set; }


        /// <summary>
        /// The exponent for determine weight optimal search direction m_osd.
        /// </summary>
        public double Psin { get; private set; }

        /// <summary>
        /// The coefficient of radius reduction.
        /// </summary>
        public double RadiusReduct { get; private set; }

        /// <summary>
        /// Parameters for GEM. <see cref="GEMOptimizer"/>.
        /// </summary>
        /// <param name="NGrenade">Number of grenades on each iteration.</param>
        /// <param name="NShrapnel">Number of shrapnel for each grenade.</param>
        /// <param name="IMax">Max iteration.</param>
        /// <param name="InitRadiusGrenade">The initial radius of the grenade territory. Maximum value is equal  2 * sqrt(n), where n - dimension space.</param>
        /// <param name="RadiusReduct">The coefficient of radius reduction.</param>
        /// <param name="ProbabilityCollision">Probability of collision.</param>
        /// <param name="Mmax">Maximum value of exponent for reduce the radius of explosion.</param>
        /// <param name="Mmin">Minimum value of exponent for reduce the radius of explosion.</param>
        /// <param name="DesiredMinimum">The number of desired minimums.</param>
        /// <param name="Psin">The exponent for determine weight optimal search direction m_osd.</param>
        /// <exception cref="ArgumentException"></exception>
        public GEMParams(int NGrenade, int NShrapnel, int IMax, double InitRadiusGrenade,
            double RadiusReduct, double ProbabilityCollision = 0.8, double Psin = 5, double Mmin = 0.1, double Mmax = 0.9, int DesiredMinimum = 1)
        {
            if (NGrenade < 1)
                throw new ArgumentException($"{nameof(NGrenade)} must be > 0.", nameof(NGrenade));
            if (NShrapnel < 1)
                throw new ArgumentException($"{nameof(NShrapnel)} must be > 0.", nameof(NShrapnel));
            if (IMax < 1)
                throw new ArgumentException($"{nameof(IMax)} must be > 0.", nameof(IMax));
            if (DesiredMinimum < 1)
                throw new ArgumentException($"{nameof(DesiredMinimum)} must be > 0.", nameof(DesiredMinimum));

            if (Math.Abs(RadiusReduct) < 1E-8)
                throw new ArgumentException($"{nameof(RadiusReduct)} too small.", nameof(RadiusReduct));

            if (Math.Abs(ProbabilityCollision) < 1E-6 || ProbabilityCollision < 0)
                throw new ArgumentException($"{nameof(ProbabilityCollision)} too small or less than 0.", nameof(ProbabilityCollision));

            if (Math.Abs(ProbabilityCollision - 1) < 1E-10 || ProbabilityCollision > 1)
                throw new ArgumentException($"{nameof(ProbabilityCollision)} must be < 1.", nameof(ProbabilityCollision));

            if (Mmax < 0 || Mmax > 1 || Mmax < Mmin)
                throw new ArgumentException($"{nameof(Mmax)} must be in [0;1] and {nameof(Mmax)} > {nameof(Mmin)}.", nameof(Mmax));

            if (Mmin < 0 || Mmin > 1)
                throw new ArgumentException($"{nameof(Mmin)}  must be in [0;1]", nameof(Mmin));


            this.DesiredMin = DesiredMinimum;
            this.NGrenade = NGrenade;
            this.NShrapnel = NShrapnel;
            this.Imax = IMax;
            this.InitRadiusGrenade = InitRadiusGrenade;
            this.RadiusReduct = RadiusReduct;
            this.Pts = ProbabilityCollision;
            this.Mmax = Mmax;
            this.Mmin = Mmin;
            this.Psin = Psin;
        }
    }

}
