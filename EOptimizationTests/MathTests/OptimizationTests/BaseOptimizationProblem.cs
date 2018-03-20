namespace EOpt.Math.Optimization.Tests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base optimization problem. 
    /// </summary>
    internal abstract class BaseOptimizationProblem
    {
        private double[] _lowerBounds, _upperBounds;

        /// <summary>
        /// Lower bounds. 
        /// </summary>
        public IReadOnlyList<double> LowerBounds => _lowerBounds;

        /// <summary>
        /// Upper bounds. 
        /// </summary>
        public IReadOnlyList<double> UpperBounds => _upperBounds;

        /// <summary>
        /// </summary>
        /// <param name="LowerBounds">   </param>
        /// <param name="UpperBounds">   </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public BaseOptimizationProblem(IReadOnlyCollection<double> LowerBounds, IReadOnlyCollection<double> UpperBounds)
        {
            if (LowerBounds == null)
            {
                throw new ArgumentNullException(nameof(LowerBounds));
            }

            if (UpperBounds == null)
            {
                throw new ArgumentNullException(nameof(UpperBounds));
            }

            if (LowerBounds.Count != UpperBounds.Count)
            {
                throw new ArgumentException($"Length {nameof(LowerBounds)} ({LowerBounds.Count}) and  {nameof(UpperBounds)} ({UpperBounds.Count}) must be equal.");
            }

            if (LowerBounds.Count == 0)
            {
                throw new ArgumentException($"The constraints are empty.");
            }

            _lowerBounds = new double[LowerBounds.Count];
            _upperBounds = new double[UpperBounds.Count];

            int position = 0;

            using (IEnumerator<double> enumLower = LowerBounds.GetEnumerator(), enumUpper = UpperBounds.GetEnumerator())
            {
                while (enumLower.MoveNext() & enumUpper.MoveNext())
                {
                    if (enumLower.Current >= enumUpper.Current)
                    {
                        throw new ArgumentException($"The lower bound at position {position} is greater than upper bound.");
                    }

                    _lowerBounds[position] = enumLower.Current;
                    _upperBounds[position] = enumUpper.Current;

                    position++;
                }
            }
        }
    }
}