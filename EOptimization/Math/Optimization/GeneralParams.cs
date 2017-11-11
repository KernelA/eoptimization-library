// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{

    using System;

    /// <summary>
    /// General parameters for the optimization methods.
    /// <seealso cref="IOptimizer{T}"/>
    /// </summary>
    public class GeneralParams
    {
        /// <summary>
        /// Coordinates of first vertex.
        /// </summary>
        public double[] LowerBound { get; private set; }

        /// <summary>
        /// Coordinates of second vertex.
        /// </summary>
        public double[] UpperBound { get; private set; }

        /// <summary>
        /// Target function.
        /// </summary>
        public Func<double[], double> TargetFunction { get; private set; }

        /// <summary>
        /// <para>
        /// General parameters for methods.
        /// </para>
        /// <para>
        /// Constraints is rectangular parallelepiped. The target function is f(x1, ...,xn). Each xi in [<paramref name="LowerBound"/>[i], <paramref name="UpperBound"/>[i]], i = 1,...,n.
        /// </para>
        /// </summary>
        /// <param name="TargetFunction">Target function.</param>
        /// <param name="LowerBound">An array of lower boundaries.</param>
        /// <param name="UpperBound">An array of upper boundaries.</param>
        /// <exception cref="ArgumentException">
        /// <para>
        /// If lengths of <paramref name="LowerBound"/> and <paramref name="UpperBound"/> are not equal.
        /// </para>
        /// <para>
        /// If exist j  such as <paramref name="LowerBound"/>[j] >= <paramref name="UpperBound"/>[j].
        /// </para>
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="TargetFunction"/> or  <paramref name="LowerBound"/> or <paramref name="UpperBound"/> are null.</exception>
        public GeneralParams(Func<double[], double> TargetFunction, double[] LowerBound, double[] UpperBound)
        {
            if (LowerBound == null)
                throw new ArgumentNullException(nameof(LowerBound));
            if (UpperBound == null)
                throw new ArgumentNullException(nameof(UpperBound));
            if (TargetFunction == null)
                throw new ArgumentNullException(nameof(TargetFunction));
            if (LowerBound.Length != UpperBound.Length)
                throw new ArgumentException($"Length {nameof(LowerBound)} ({LowerBound.Length}) and  {nameof(UpperBound)} ({UpperBound.Length}) must be equal.");

            for (int i = 0; i < LowerBound.Length; i++)
            {
                if (LowerBound[i] >= UpperBound[i])
                    throw new ArgumentException($"{nameof(LowerBound)}[{i}]  greater than {nameof(UpperBound)}[{i}].");
            }

            this.LowerBound = LowerBound;
            this.UpperBound = UpperBound;
            this.TargetFunction = TargetFunction;
        }
    }
}
