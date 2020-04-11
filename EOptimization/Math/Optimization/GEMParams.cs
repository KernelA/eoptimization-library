// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;

    /// <summary>
    /// Parameters for GEM method.
    /// </summary>
    public struct GEMParams
    {
        /// <summary>
        /// The number of desired minimums.
        /// </summary>
        public int DesiredMin { get; private set; }

        /// <summary>
        /// The number of iteration.
        /// </summary>
        public int Imax { get; private set; }

        /// <summary>
        /// The initial radius of the grenade territory.
        /// </summary>
        public double InitRadiusGrenade { get; private set; }

        /// <summary>
        /// A value indicates, if parameters are set or not.
        /// </summary>
        public bool IsParamsInit { get; }

        /// <summary>
        /// Maximum value of exponent for reduce the radius of explosion.
        /// </summary>
        public double Mmax { get; private set; }

        /// <summary>
        /// Minimum value of exponent for reduce the radius of explosion.
        /// </summary>
        public double Mmin { get; private set; }

        /// <summary>
        /// Number of grenades on each iteration.
        /// </summary>
        public int NGrenade { get; private set; }

        /// <summary>
        /// Number of shrapnel for each grenade.
        /// </summary>
        public int NShrapnel { get; private set; }

        /// <summary>
        /// The exponent for determine weight optimal search direction m_osd.
        /// </summary>
        public double Psin { get; private set; }

        /// <summary>
        /// Probability of collision.
        /// </summary>
        public double Pts { get; private set; }

        /// <summary>
        /// The coefficient of radius reduction.
        /// </summary>
        public double RadiusReduct { get; private set; }

        /// <summary>
        /// Parameters for GEM.
        /// </summary>
        /// <param name="NGrenade">
        /// Number of grenades on each iteration. <paramref name="NGrenade"/> &gt;= 1.
        /// </param>
        /// <param name="NShrapnel">
        /// Number of shrapnel for each grenade. <paramref name="NShrapnel"/> &gt;= 1.
        /// </param>
        /// <param name="Imax">
        /// The number of iteration. <paramref name="Imax"/> &gt;= 1.
        /// </param>
        /// <param name="InitRadiusGrenade">
        /// The initial radius of the grenade territory. Maximum value is equal 2 * sqrt(n), where
        /// n-dimension space. <paramref name="InitRadiusGrenade"/> &gt; 0.
        /// </param>
        /// <param name="RadiusReduct">
        /// The coefficient of radius reduction. <paramref name="RadiusReduct"/> &gt; 1.
        /// </param>
        /// <param name="ProbabilityCollision">
        /// Probability of collision. <paramref name="NGrenade"/> in (0;1).
        /// </param>
        /// <param name="Mmax">
        /// <para>
        /// Maximum value of exponent for reduce the radius of explosion. <paramref name="Mmax"/> in (0;1].
        /// </para>
        /// <para> <paramref name="Mmax"/> &gt; <paramref name="Mmin"/>. </para>
        /// </param>
        /// <param name="Mmin">
        /// <para>
        /// Minimum value of exponent for reduce the radius of explosion. <paramref name="Mmin"/> in [0;1).
        /// </para>
        /// <para> <paramref name="Mmin"/> &lt; <paramref name="Mmax"/>. </para>
        /// </param>
        /// <param name="DesiredMinimum">
        /// The number of desired minimums. <paramref name="DesiredMinimum"/> &gt;= 1.
        /// </param>
        /// <param name="Psin">
        /// The exponent for determine weight optimal search direction m_osd. <paramref name="Psin"/>
        /// &gt; 0.
        /// </param>
        /// <exception cref="ArgumentException"> If conditions for parameters do not performed. </exception>
        public GEMParams(int NGrenade, int NShrapnel, int Imax, double InitRadiusGrenade,
            double RadiusReduct = 100, double ProbabilityCollision = 0.8, double Psin = 5, double Mmin = 0.1, double Mmax = 0.9, int DesiredMinimum = 1)
        {
            if (NGrenade < 1)
                throw new ArgumentException($"{nameof(NGrenade)} (actual value is {NGrenade}) must be > 0.", nameof(NGrenade));
            if (NShrapnel < 1)
                throw new ArgumentException($"{nameof(NShrapnel)} (actual value is {NShrapnel}) must be > 0.", nameof(NShrapnel));
            if (Imax < 1)
                throw new ArgumentException($"{nameof(Imax)}  (actual value is {Imax}) must be > 0.", nameof(Imax));
            if (DesiredMinimum < 1)
                throw new ArgumentException($"{nameof(DesiredMinimum)}  (actual value is {DesiredMinimum}) must be > 0.", nameof(DesiredMinimum));

            if (RadiusReduct < 1)
                throw new ArgumentException($"{nameof(RadiusReduct)} (actual value is {RadiusReduct}) must be > 1.", nameof(RadiusReduct));

            if (CmpDouble.AlmostEqual(0, ProbabilityCollision, -Constants.EPS_EXPONENT / 2) || ProbabilityCollision < 0)
                throw new ArgumentException($"{nameof(ProbabilityCollision)} (actual value is {ProbabilityCollision})  too small or less than 0.", nameof(ProbabilityCollision));

            if (CmpDouble.AlmostEqual(1, ProbabilityCollision, 2) || ProbabilityCollision > 1)
                throw new ArgumentException($"{nameof(ProbabilityCollision)} (actual value is {ProbabilityCollision}) must be < 1.", nameof(ProbabilityCollision));

            if (Mmax < 0 || Mmax > 1 || Mmax < Mmin)
                throw new ArgumentException($"{nameof(Mmax)}  (actual value is {Mmax}) must be in (0;1] and {nameof(Mmax)} > {nameof(Mmin)} (actual value is {Mmin}).", nameof(Mmax));

            if (Mmin < 0 || Mmin > 1)
                throw new ArgumentException($"{nameof(Mmin)}  (actual value is {Mmin}) must be in [0;1)", nameof(Mmin));

            this.IsParamsInit = true;
            this.DesiredMin = DesiredMinimum;
            this.NGrenade = NGrenade;
            this.NShrapnel = NShrapnel;
            this.Imax = Imax;
            this.InitRadiusGrenade = InitRadiusGrenade;
            this.RadiusReduct = RadiusReduct;
            this.Pts = ProbabilityCollision;
            this.Mmax = Mmax;
            this.Mmin = Mmin;
            this.Psin = Psin;
        }
    }
}