// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// Continuous uniform distribution.
    /// </summary>
    public class ContUniformDistribution : IContUniformGenerator
    {

        private double lowBound, upperBound;

        private Random rand;

        /// <summary>
        /// The left boundary of the distribution range.
        /// </summary>
        /// <exception cref="ArgumentException">If value > UpperBound.</exception>
        public double LowBound
        {
            get
            {
                return lowBound;
            }
            set
            {
                if (value > upperBound)
                    throw new ArgumentException($"The {LowBound} must be less than the upper boundary ({UpperBound}).", nameof(LowBound));

                lowBound = value;
            }
        }

        /// <summary>
        /// The right boundary of the distribution range.
        /// </summary>
        public double UpperBound
        {
            get
            {
                return upperBound;
            }
            set
            {
                if (value < lowBound)
                    throw new ArgumentException($"The {UpperBound} must be greater than lower boundary ({LowBound}).", nameof(UpperBound));

                upperBound = value;
            }
        }


        /// <summary>
        /// Create continuous uniform distribution on [0; 1].
        /// </summary>
        public ContUniformDistribution() : this(0, 1)
        {
        }

        /// <summary>
        /// Create continuous uniform distribution on [<paramref name="LowBound"/>; <paramref name="UpperBound"/>].
        /// </summary>
        /// <param name="LowBound">The lower boundary of the distribution range.</param>
        /// <param name="UpperBound">The upper boundary of the distribution range.</param>
        /// <exception cref="ArgumentException">If <paramref name="LowBound"/> >= <paramref name="UpperBound"/>.</exception>
        public ContUniformDistribution(double LowBound, double UpperBound)
        {
            if (LowBound >= UpperBound)
                throw new ArgumentException($"{UpperBound} must be greater than {LowBound}.");

            this.LowBound = LowBound;
            this.UpperBound = UpperBound;

            rand = SyncRandom.Get();
        }


        /// <summary>
        /// Random value from continuous uniform distribution on [<paramref name="LowBound"/>; <paramref name="UpperBound"/>].
        /// </summary>
        /// <param name="LowBound">The lower boundary of the distribution range.</param>
        /// <param name="UpperBound">The upper boundary of the distribution range.</param>
        /// <exception cref="ArgumentException">If <paramref name="LowBound"/> >= <paramref name="UpperBound"/>.</exception>
        public double URandVal(double LowBound, double UpperBound)
        {
            if (LowBound >= UpperBound)
                throw new ArgumentException($"{UpperBound} must be greater than {LowBound}.");

            return (UpperBound - LowBound) * rand.NextDouble() + LowBound;
        }
    }
}
