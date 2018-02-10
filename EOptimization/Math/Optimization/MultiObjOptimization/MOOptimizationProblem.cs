// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.MOOpt
{
    using System;
    using System.Collections.Generic;

    using TTargetFunction = System.Func<System.Collections.Generic.IReadOnlyList<double>, double>;

    public class MOOptimizationProblem : BaseOptimizationProblem
    {
        private List<TTargetFunction> _targetFunction;

        public IReadOnlyList<TTargetFunction> TargetFunction => _targetFunction;

        public MOOptimizationProblem(IEnumerable<TTargetFunction> TargetFunction, IReadOnlyCollection<double> LowerBounds, IReadOnlyCollection<double> UpperBounds) :
            base(LowerBounds, UpperBounds)
        {
            if (TargetFunction == null)
            {
                throw new ArgumentNullException(nameof(TargetFunction));
            }

            int position = 0;

            foreach (var func in TargetFunction)
            {
                if (func == null)
                {
                    throw new ArgumentNullException($"{nameof(TargetFunction)} contains null at position {position}.");
                }
                position++;
            }

            _targetFunction = new List<Func<IReadOnlyList<double>, double>>(TargetFunction);
        }
    }
}