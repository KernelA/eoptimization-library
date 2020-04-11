namespace EOpt.Math.Optimization.Tests
{
    using System;
    using System.Collections.Generic;

    using EOpt.Math.Optimization.OOOpt;

    /// <summary>
    /// An optimization problem with an one target function.
    /// </summary>
    internal class OOOptimizationProblem : BaseOptimizationProblem, IOOOptProblem
    {
        private Func<IReadOnlyList<double>, double> _targetFunction;

        /// <summary>
        /// A target function.
        /// </summary>
        public Func<IReadOnlyList<double>, double> TargetFunction => _targetFunction;

        /// <summary>
        /// </summary>
        /// <param name="TargetFunction"> A target function. </param>
        /// <param name="LowerBounds">   </param>
        /// <param name="UpperBounds">   </param>
        public OOOptimizationProblem(Func<IReadOnlyList<double>, double> TargetFunction, IReadOnlyCollection<double> LowerBounds, IReadOnlyCollection<double> UpperBounds) :
            base(LowerBounds, UpperBounds)
        {
            if (TargetFunction == null)
            {
                throw new ArgumentNullException(nameof(TargetFunction));
            }
            _targetFunction = TargetFunction;
        }

        double IConstrOptProblem<double, double>.TargetFunction(IReadOnlyList<double> Point)
        {
            return _targetFunction(Point);
        }
    }
}