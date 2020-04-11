// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com

namespace EOpt.Math
{
    /// <summary>
    /// Kahan summation algorithm.
    /// </summary>
    public class KahanSum
    {
        private double _sum, _correction, _correctedNext, _newSum;

        /// <summary>
        /// A sum.
        /// </summary>
        public double Sum => _sum;

        /// <summary>
        ///
        /// </summary>
        public KahanSum()
        {
            Init();
        }

        /// <summary>
        /// Reset sum.
        /// </summary>
        public void SumResest()
        {
            Init();
        }

        /// <summary>
        /// Add a summand.
        /// </summary>
        /// <param name="Value"></param>
        public void Add(double Value)
        {
            _correctedNext = Value - _correction;
            _newSum = _sum + _correctedNext;
            _correction = (_newSum - _sum) - _correctedNext;
            _sum = _newSum;
        }

        private void Init()
        {
            _sum = _correction = _correctedNext = _newSum = 0.0;
        }
    }
}