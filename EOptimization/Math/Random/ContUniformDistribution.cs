namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// Continuous uniform distribution.
    /// </summary>
    public class ContUniformDistribution : IContUniformGenerator
    {

        private double a, b;

        private Random rand;

        /// <summary>
        /// The left boundary of the distribution range.
        /// </summary>
        public double LeftBound
        {
            get
            {
                return a;
            }
            set
            {
                if (value > b)
                    throw new ArgumentException($"{LeftBound} must be less than right boundary.", nameof(LeftBound));

                a = value;
            }
        }

        /// <summary>
        /// The right boundary of the distribution range.
        /// </summary>
        public double RightBound
        {
            get
            {
                return b;
            }
            set
            {
                if (value < a)
                    throw new ArgumentException($"{RightBound} must be greater than left boundary.", nameof(RightBound));

                b = value;
            }
        }


        /// <summary>
        /// Create continuous uniform distribution on [0; 1].
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public ContUniformDistribution() : this(0, 1)
        {
        }

        /// <summary>
        /// Create continuous uniform distribution on [<paramref name="LeftBound"/>; <paramref name="RightBound"/>].
        /// </summary>
        /// <param name="LeftBound">The left boundary of the distribution range.</param>
        /// <param name="RightBound">The right boundary of the distribution range.</param>
        /// <exception cref="ArgumentException"></exception>
        public ContUniformDistribution(double LeftBound, double RightBound)
        {
            if (LeftBound > RightBound)
                throw new ArgumentException($"{RightBound} must be greater than {LeftBound}.");

            this.LeftBound = LeftBound;
            this.RightBound = RightBound;

            rand = SyncRandom.Get();
        }

        /// <summary>
        /// Random value from continuous uniform distribution, where distribution range specified in class.
        /// </summary>
        /// <returns></returns>
        public double RandVal()
        {
            return URandVal(LeftBound, RightBound);
        }

        /// <summary>
        /// Random value from continuous uniform distribution on [<paramref name="LeftBound"/>; <paramref name="RightBound"/>].
        /// </summary>
        /// <param name="LeftBound">The left boundary of the distribution range.</param>
        /// <param name="RightBound">The right boundary of the distribution range.</param>
        /// <returns></returns>
        public double URandVal(double LeftBound, double RightBound)
        {
            if (LeftBound > RightBound)
                throw new ArgumentException($"{RightBound} must be greater than {LeftBound}.");

            return (RightBound - LeftBound) * rand.NextDouble() + LeftBound;
        }
    }
}
