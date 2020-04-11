// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// Normal distribution.
    /// </summary>
    public class NormalDist : INormalGen
    {
        private bool _isCachedValue;
        private double _mean, _stdDev, _uniformRand1, _uniformRand2, _r, _cachedValue;
        private Random _rand;

        /// <summary>
        /// Mean value.
        /// </summary>
        public double Mean
        {
            get => _mean;
            set => _mean = value;
        }

        /// <summary>
        /// Standard deviation.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public double StdDev
        {
            get => _stdDev;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"{nameof(StdDev)} must be > 0.", nameof(StdDev));
                }

                _stdDev = value;
            }
        }

        /// <summary>
        /// Create normal distribution with mean is equal 0 and standard deviation is equal 1.
        /// </summary>
        public NormalDist() : this(0, 1)
        {
        }

        /// <summary>
        /// Create normal distribution with mean is equal <paramref name="Mean"/> and standard
        /// deviation is equal <paramref name="StdDev"/>.
        /// </summary>
        /// <param name="Mean">   Mean value. </param>
        /// <param name="StdDev"> Standard deviation. </param>
        /// <exception cref="ArgumentException"></exception>
        public NormalDist(double Mean, double StdDev)
        {
            _rand = SyncRandom.Get();

            _uniformRand1 = 0;
            _cachedValue = 0;
            _r = 0;
            _uniformRand2 = 0;
            _isCachedValue = false;
            this.Mean = Mean;
            this.StdDev = StdDev;
        }

        /// <summary>
        /// Random value from normal distribution with mean is equal <paramref name="Mean"/> and
        /// standard deviation is equal <paramref name="StdDev"/>.
        /// </summary>
        /// <remarks> Using Marsaglia polar method. </remarks>
        /// <param name="Mean">   Mean. </param>
        /// <param name="StdDev"> Standard deviation. </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> If <paramref name="StdDev"/> &lt;=0 </exception>
        public double NRandVal(double Mean, double StdDev)
        {
            if (StdDev <= 0)
            {
                throw new ArgumentException($"{nameof(StdDev)} (value is {StdDev}) must be > 0.", nameof(StdDev));
            }

            if (_isCachedValue)
            {
                _uniformRand1 = _cachedValue;
                _isCachedValue = false;
            }
            else
            {
                do
                {
                    _uniformRand1 = 2.0 * _rand.NextDouble() - 1.0;
                    _uniformRand2 = 2.0 * _rand.NextDouble() - 1.0;
                    _r = _uniformRand1 * _uniformRand1 + _uniformRand2 * _uniformRand2;
                }
                while (_r >= 1.0 || _r == 0);

                _isCachedValue = true;

                _cachedValue = _uniformRand2;
            }

            return Mean + StdDev * (_uniformRand1 * Math.Sqrt(-2 * Math.Log(_r) / _r));
        }
    }
}