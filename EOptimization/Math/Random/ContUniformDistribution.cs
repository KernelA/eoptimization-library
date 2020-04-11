// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// A continuous uniform distribution.
    /// </summary>
    public class ContUniformDist : IContUniformGen
    {
        private double _lowBound, _upperBound;

        private Random _rand;

        /// <summary>
        /// The left boundary of the distribution range.
        /// </summary>
        /// <exception cref="ArgumentException"> If value &gt; UpperBound. </exception>
        public double LowBound
        {
            get => _lowBound;
            set
            {
                if (value > _upperBound)
                {
                    throw new ArgumentException($"The lower boundary ({LowBound}) must be less than the upper boundary ({UpperBound}).", nameof(LowBound));
                }

                _lowBound = value;
            }
        }

        /// <summary>
        /// The right boundary of the distribution range.
        /// </summary>
        public double UpperBound
        {
            get
            {
                return _upperBound;
            }
            set
            {
                if (value < _lowBound)
                    throw new ArgumentException($"The upper boundary ({UpperBound}) must be greater than lower boundary ({LowBound}).", nameof(UpperBound));

                _upperBound = value;
            }
        }

        /// <summary>
        /// Create continuous uniform distribution on [0; 1].
        /// </summary>
        public ContUniformDist() : this(0, 1)
        {
        }

        /// <summary>
        /// Create continuous uniform distribution on [ <paramref name="LowBound"/>; <paramref name="UpperBound"/>].
        /// </summary>
        /// <param name="LowBound">   The lower boundary of the distribution range. </param>
        /// <param name="UpperBound"> The upper boundary of the distribution range. </param>
        /// <exception cref="ArgumentException"> If <paramref name="LowBound"/> &gt;= <paramref name="UpperBound"/>. </exception>
        public ContUniformDist(double LowBound, double UpperBound)
        {
            if (LowBound >= UpperBound)
            {
                throw new ArgumentException($"{nameof(UpperBound)} must be greater than {nameof(LowBound)}.");
            }

            this.LowBound = LowBound;
            this.UpperBound = UpperBound;

            _rand = SyncRandom.Get();
        }

        /// <summary>
        /// Random value from continuous uniform distribution on [ <paramref name="LowBound"/>; <paramref name="UpperBound"/>].
        /// </summary>
        /// <param name="LowBound">   The lower boundary of the distribution range. </param>
        /// <param name="UpperBound"> The upper boundary of the distribution range. </param>
        /// <exception cref="ArgumentException"> If <paramref name="LowBound"/> &gt;= <paramref name="UpperBound"/>. </exception>
        public double URandVal(double LowBound, double UpperBound)
        {
            if (LowBound >= UpperBound)
            {
                throw new ArgumentException($"{nameof(UpperBound)} (value is {UpperBound}) must be greater than {nameof(LowBound)} (value is {LowBound}).");
            }

            return (UpperBound - LowBound) * _rand.NextDouble() + LowBound;
        }
    }
}