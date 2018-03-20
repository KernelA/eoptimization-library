namespace EOpt.Math.Optimization.Tests
{
    using System;
    using System.Collections.Generic;

    using EOpt.Math.Optimization.MOOpt;

    using TTargetFunction = System.Func<System.Collections.Generic.IReadOnlyList<double>, double>;

    internal class MOOptimizationProblem : BaseOptimizationProblem, IMOOptProblem
    {
        private List<TTargetFunction> _targetFunction;

        public IReadOnlyList<TTargetFunction> TargetFunction => _targetFunction;

        public int CountObjs => throw new NotImplementedException();

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

        public double ObjFunction(IReadOnlyList<double> Point, int NumObj) => throw new NotImplementedException();

        IEnumerable<double> IConstrOptProblem<double, IEnumerable<double>>.TargetFunction(IReadOnlyList<double> Point) => throw new NotImplementedException();
    }
}