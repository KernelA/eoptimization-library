namespace EOpt.Math.Optimization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EOpt.Math.Optimization.MOOpt;

    using TTargetFunction = System.Func<System.Collections.Generic.IReadOnlyList<double>, double>;

    internal class MOOptimizationProblem : BaseOptimizationProblem, IMOOptProblem
    {
        private List<TTargetFunction> _targetFunction;

        public int CountObjs => _targetFunction.Count;

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

            _targetFunction = new List<TTargetFunction>(TargetFunction);
        }

        public IEnumerable<double> TargetFunction(IReadOnlyList<double> Point)
        {
            return _targetFunction.Select(func => func(Point));
        }

        public double ObjFunction(IReadOnlyList<double> Point, int NumObj) => _targetFunction[NumObj](Point);
    }
}