// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.OOOpt
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An optimization problem with an one target function. 
    /// </summary>
    public class OOOptimizationProblem : BaseOptimizationProblem
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
    }
}