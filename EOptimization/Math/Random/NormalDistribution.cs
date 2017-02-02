namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// Normal distribution.
    /// </summary>
    public class NormalDistribution : INormalGenerator
    {

        private double mean, stdDev;

        private Random rand;

        /// <summary>
        /// Mean value.
        /// </summary>
        public double Mean
        {
            get
            {
                return mean;
            }
            set
            {
                mean = value;
            }
        }

        /// <summary>
        /// Standard deviation.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public double StdDev
        {
            get
            {
                return stdDev;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException($"{nameof(StdDev)} must be > 0.", nameof(StdDev));

                stdDev = value;
            }
        }


        /// <summary>
        /// Create normal distribution with mean  equal 0 and standard deviation equal 1.
        /// </summary>
        public NormalDistribution() : this(0, 1)
        {

        }

        /// <summary>
        /// Create normal distribution with mean  equal <paramref name="Mean"/> and standard deviation equal <paramref name="StdDev"/>.
        /// </summary>
        /// <param name="Mean">Mean value.</param>
        /// <param name="StdDev">Standard deviation.</param>
        /// <exception cref="ArgumentException"></exception>
        public NormalDistribution(double Mean, double StdDev)
        {
            rand = SyncRandom.Get();

            this.Mean = Mean;
            this.StdDev = StdDev;

        }

        /// <summary>
        /// Random value from normal distribution with mean equal <paramref name="Mean"/> and standard deviation equal <paramref name="StdDev"/>.
        /// </summary>
        /// <remarks>Using Box–Muller transform.</remarks>
        /// <param name="Mean">Mean.</param>
        /// <param name="StdDev">Standard deviation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public double NRandVal(double Mean, double StdDev)
        {
            if (StdDev <= 0)
                throw new ArgumentException($"{nameof(StdDev)} must be > 0.", nameof(StdDev));

            double u = 0.0;
            double v = 0.0;

            double s = 0.0;

            do
            {
                u = 2.0 * rand.NextDouble() - 1.0;
                v = 2.0 * rand.NextDouble() - 1.0;
                s = u * u + v * v;
            }
            while (s > 1.0 || s < 1E-10);

            return Mean + StdDev * (u * Math.Sqrt(-2 * Math.Log(s) / s));
        }


        /// <summary>
        /// Random value from normal distribution where mean and standard deviation specified in class.
        /// </summary>
        /// <remarks>Using Box–Muller transform.</remarks>
        /// <param name="Mean">Mean.</param>
        /// <param name="StdDev">Standard deviation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public double RandVal()
        {
            return NRandVal(Mean, StdDev);
        }
    }
}
