// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// Normal distribution.
    /// </summary>
    public class NormalDistribution : INormalGenerator
    {

        private double mean, stdDev, uniformRand1, uniformRand2, r, cachedValue;

        private bool isCachedValue;

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
        /// Create normal distribution with mean is equal 0 and standard deviation is equal 1.
        /// </summary>
        public NormalDistribution() : this(0, 1)
        {

        }

        /// <summary>
        /// Create normal distribution with mean is equal <paramref name="Mean"/> and standard deviation is equal <paramref name="StdDev"/>.
        /// </summary>
        /// <param name="Mean">Mean value.</param>
        /// <param name="StdDev">Standard deviation.</param>
        /// <exception cref="ArgumentException"></exception>
        public NormalDistribution(double Mean, double StdDev)
        {
            rand = SyncRandom.Get();

            uniformRand1 = 0;
            cachedValue = 0;
            r = 0;
            uniformRand2 = 0;
            isCachedValue = false;
            this.Mean = Mean;
            this.StdDev = StdDev;

        }

        /// <summary>
        /// Random value from normal distribution with mean is equal <paramref name="Mean"/> and standard deviation is equal <paramref name="StdDev"/>.
        /// </summary>
        /// <remarks>Using Marsaglia polar method.</remarks>
        /// <param name="Mean">Mean.</param>
        /// <param name="StdDev">Standard deviation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="StdDev"/> &lt;=0</exception>
        public double NRandVal(double Mean, double StdDev)
        {
            if (StdDev <= 0)
                throw new ArgumentException($"{nameof(StdDev)} must be > 0.", nameof(StdDev));

            if(isCachedValue)
            {
                uniformRand1 = cachedValue;
                isCachedValue = false;
            }
            else
            {
                do
                {
                    uniformRand1 = 2.0 * rand.NextDouble() - 1.0;
                    uniformRand2 = 2.0 * rand.NextDouble() - 1.0;
                    r = uniformRand1 * uniformRand1 + uniformRand2 * uniformRand2;
                }
                while (r >= 1.0 || r == 0);

                isCachedValue = true;

                cachedValue = uniformRand2;

            }


            return Mean + StdDev * (uniformRand1 * Math.Sqrt(-2 * Math.Log(r) / r));
        }


    }
}
