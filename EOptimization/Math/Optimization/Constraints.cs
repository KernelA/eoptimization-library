// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Constraints for the optimization problem.
    /// </summary>
    internal static class Constraints
    {
        public static void CheckConstraints(IReadOnlyCollection<double> LowerBounds, IReadOnlyCollection<double> UpperBounds)
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

            int position = 0;

            using (IEnumerator<double> enumLower = LowerBounds.GetEnumerator(), enumUpper = UpperBounds.GetEnumerator())
            {
                while (enumLower.MoveNext() & enumUpper.MoveNext())
                {
                    if (enumLower.Current >= enumUpper.Current)
                    {
                        throw new ArgumentException($"The lower bound at position {position} is greater than upper bound.");
                    }
                    position++;
                }
            }
        }
    }
}