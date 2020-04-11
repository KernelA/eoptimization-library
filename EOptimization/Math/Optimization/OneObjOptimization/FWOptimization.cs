// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.OOOpt
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Help;

    using Math;
    using Math.Random;

    using Optimization;

    /// <summary>
    /// Optimization method Fireworks.
    /// </summary>
    public class FWOptimizer : BaseFW<double, IOOOptProblem>, IOOOptimizer<FWParams>
    {
        private double _fmax, _fmin;

        private int _indexSolutionInCharges, _indexSolutionInDebrisFromCharge, _indexSolutionInDebris;

        private Agent _solution;

        private void EvalFunction(Func<IReadOnlyList<double>, double> Function)
        {
            for (int i = 0; i < _parameters.NP; i++)
            {
                _chargePoints[i].Eval(Function);
            }
        }

        private void EvalFunctionForDebris(Func<IReadOnlyList<double>, double> Function)
        {
            for (int i = 0; i < _debris.Length; i++)
            {
                foreach (Agent agent in _debris[i])
                {
                    agent.Eval(Function);
                }
            }
        }

        /// <summary>
        /// Find amount debris for each point of charge.
        /// </summary>
        private void FindAmountDebris()
        {
            double s = 0;

            double denumerator = 0;

            for (int i = 0; i < _parameters.NP; i++)
            {
                denumerator += _fmax - _chargePoints[i].Objs[0];
            }

            if (denumerator < Constants.VALUE_AVOID_DIV_BY_ZERO)
            {
                denumerator += Constants.VALUE_AVOID_DIV_BY_ZERO;
            }

            for (int i = 0; i < _parameters.NP; i++)
            {
                s = _parameters.M * (_fmax - _chargePoints[i].Objs[0] + Constants.VALUE_AVOID_DIV_BY_ZERO) / denumerator;

                base.FindAmountDebrisForCharge(s, i);
            }
        }

        /// <summary>
        /// Find best solution among debris and charges.
        /// </summary>
        private void FindBestSolution()
        {
            _solution.SetAt(_chargePoints[0]);
            _indexSolutionInCharges = 0;

            _indexSolutionInDebrisFromCharge = -1;
            _indexSolutionInDebris = -1;

            // Searching best solution among charges.
            for (int i = 1; i < _parameters.NP; i++)
            {
                if (_chargePoints[i].Objs[0] < _solution.Objs[0])
                {
                    _solution.SetAt(_chargePoints[i]);
                    _indexSolutionInCharges = i;
                }
            }

            // Searching best solution among debris.
            for (int j = 0; j < _parameters.NP; j++)
            {
                int k = 0;
                foreach (Agent splinter in _debris[j])
                {
                    if (splinter.Objs[0] < _solution.Objs[0])
                    {
                        _solution.SetAt(splinter);
                        _indexSolutionInCharges = -1;
                        _indexSolutionInDebrisFromCharge = j;
                        _indexSolutionInDebris = k;
                    }

                    k++;
                }
            }
        }

        private void FindFMaxMin()
        {
            _fmin = _chargePoints[0].Objs[0];
            _fmax = _chargePoints[0].Objs[0];

            for (int i = 0; i < _parameters.NP; i++)
            {
                if (_chargePoints[i].Objs[0] < _fmin)
                {
                    _fmin = _chargePoints[i].Objs[0];
                }
                else if (_chargePoints[i].Objs[0] > _fmax)
                {
                    _fmax = _chargePoints[i].Objs[0];
                }
            }
        }

        /// <summary>
        /// Determine debris position.
        /// </summary>
        /// <param name="LowerBounds"></param>
        /// <param name="UpperBounds"></param>
        private void GenerateDebris(IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds)
        {
            double denumerator = 0;

            for (int j = 0; j < _parameters.NP; j++)
            {
                denumerator += _chargePoints[j].Objs[0] - _fmin;
            }

            if (denumerator < Constants.VALUE_AVOID_DIV_BY_ZERO)
            {
                denumerator += Constants.VALUE_AVOID_DIV_BY_ZERO;
            }

            for (int i = 0; i < _parameters.NP; i++)
            {
                double amplitude = 0;

                // Amplitude of explosion.
                amplitude = _parameters.Amax * (_chargePoints[i].Objs[0] - _fmin + Constants.VALUE_AVOID_DIV_BY_ZERO) / denumerator;

                base.GenerateDebrisForCharge(LowerBounds, UpperBounds, amplitude, i);
            }
        }

        /// <summary>
        /// Generate current population.
        /// </summary>
        private void GenerateNextAgents()
        {
            // The total count minus solution.
            int actualSizeMatrix = _chargePoints.Count - 1;

            for (int k = 0; k < _parameters.NP; k++)
            {
                actualSizeMatrix += _debris[k].Count;
            }

            base.ResetMatrixAndTrimWeights(actualSizeMatrix);

            {
                int index = 0;

                for (int i = 0; i < _chargePoints.Count; i++)
                {
                    // Skip solution.
                    if (i != _indexSolutionInCharges)
                    {
                        _weightedAgents[index].Agent.SetAt(_chargePoints[i]);
                        index++;
                    }
                }

                for (int i = 0; i < _parameters.NP; i++)
                {
                    int k = 0;

                    foreach (Agent splinter in _debris[i])
                    {
                        // Skip solution, if it in the debris.
                        if (_indexSolutionInDebrisFromCharge != i || _indexSolutionInDebris != k)
                        {
                            _weightedAgents[index].Agent.SetAt(splinter);
                            index++;
                        }

                        k++;
                    }
                }
            }

            base.CalculateDistances((a, b) => PointND.Distance(a.Point, b.Point));

            int totalToTake = _parameters.NP - 1;

            base.TakeAgents(totalToTake);

            _chargePoints[0].SetAt(_solution);

            int startIndex = 1;

            for (int i = 0; i < actualSizeMatrix && totalToTake > 0; i++)
            {
                if (_weightedAgents[i].IsTake)
                {
                    _chargePoints[startIndex].SetAt(_weightedAgents[i].Agent);
                    startIndex++;
                    totalToTake--;
                }
            }
        }

        protected override void FirstStep(IOOOptProblem Problem)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            base.InitAgents(Problem.LowerBounds, Problem.UpperBounds, 1);

            EvalFunction(Problem.TargetFunction);

            FindFMaxMin();

            FindAmountDebris();

            GenerateDebris(Problem.LowerBounds, Problem.UpperBounds);

            EvalFunctionForDebris(Problem.TargetFunction);

            FindBestSolution();
        }

        protected override void Init(FWParams Parameters, int Dimension, int DimObjs)
        {
            base.Init(Parameters, Dimension, DimObjs);

            if (_solution == null)
            {
                _solution = new Agent(Dimension, DimObjs);
            }
            else if (_solution.Point.Count != Dimension)
            {
                _solution = new Agent(Dimension, DimObjs);
            }
        }

        protected override void NextStep(IOOOptProblem Problem)
        {
            GenerateNextAgents();

            EvalFunction(Problem.TargetFunction);

            FindFMaxMin();

            FindAmountDebris();

            GenerateDebris(Problem.LowerBounds, Problem.UpperBounds);

            EvalFunctionForDebris(Problem.TargetFunction);

            FindBestSolution();
        }

        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        public Agent Solution => _solution;

        /// <summary>
        /// Create object which uses default implementation for random generators.
        /// </summary>
        public FWOptimizer() : this(new ContUniformDist(), new NormalDist())
        {
        }

        /// <summary>
        /// Create object which uses custom implementation for random generators.
        /// </summary>
        /// <param name="UniformGen"> Object, which implements <see cref="IContUniformGen"/> interface. </param>
        /// <param name="NormalGen">  Object, which implements <see cref="INormalGen"/> interface. </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="NormalGen"/> or <paramref name="UniformGen"/> is null.
        /// </exception>
        public FWOptimizer(IContUniformGen UniformGen, INormalGen NormalGen) : base(UniformGen, NormalGen)
        {
        }

        /// <summary>
        /// <see cref="IOOOptimizer{T}.Minimize(T, OOOptimizationProblem)"/>
        /// </summary>
        /// <param name="Parameters"> Parameters for method. </param>
        /// <param name="Problem">    An optimization problem. </param>
        /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException"> If <paramref name="Problem"/> is null. </exception>
        /// <exception cref="ArithmeticException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        public override void Minimize(FWParams Parameters, IOOOptProblem Problem)
        {
            Init(Parameters, Problem.LowerBounds.Count, 1);

            FirstStep(Problem);

            for (int i = 1; i < _parameters.Imax; i++)
            {
                NextStep(Problem);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IOOOptimizer{T}.Minimize(T, OOOptimizationProblem, CancellationToken)"/>
        /// </summary>
        /// <param name="Parameters">  Parameters for method. </param>
        /// <param name="Problem">     An optimization problem. </param>
        /// <param name="CancelToken"> <see cref="CancellationToken"/> </param>
        /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException"> If <paramref name="Problem"/> is null. </exception>
        /// <exception cref="ArithmeticException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        /// <exception cref="OperationCanceledException"></exception>
        public override void Minimize(FWParams Parameters, IOOOptProblem Problem, CancellationToken CancelToken)
        {
            Init(Parameters, Problem.LowerBounds.Count, 1);

            FirstStep(Problem);

            for (int i = 1; i < _parameters.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();
                NextStep(Problem);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IOOOptimizer{T}.Minimize(T, OOOptimizationProblem, CancellationToken)"/>
        /// </summary>
        /// <param name="Parameters"> Parameters for method. </param>
        /// <param name="Problem">    An optimization problem. </param>
        /// <param name="Reporter">
        /// Object which implement interface <see cref="IProgress{T}"/>, where T is
        /// <see cref="Progress"/>. <seealso cref="IOOOptimizer{T}.Minimize(T, OOOptimizationProblem, IProgress{Progress})"/>
        /// </param>
        /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="Problem"/> or <paramref name="Reporter"/> is null.
        /// </exception>
        /// <exception cref="ArithmeticException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        public override void Minimize(FWParams Parameters, IOOOptProblem Problem, IProgress<Progress> Reporter)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem.LowerBounds.Count, 1);

            FirstStep(Problem);

            Progress progress = new Progress(this, 0, _parameters.Imax - 1, 0);

            Reporter.Report(progress);

            for (int i = 1; i < this._parameters.Imax; i++)
            {
                NextStep(Problem);
                progress.Current = i;
                Reporter.Report(progress);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IOOOptimizer{T}.Minimize(T, OOOptimizationProblem, CancellationToken)"/>
        /// </summary>
        /// <param name="Parameters">  Parameters for method. </param>
        /// <param name="Problem">     An optimization problem. </param>
        /// <param name="Reporter">
        /// Object which implement interface <see cref="IProgress{T}"/>, where T is
        /// <see cref="Progress"/>. <seealso cref="IOOOptimizer{T}.Minimize(T, OOOptimizationProblem, IProgress{Progress})"/>
        /// </param>
        /// <param name="CancelToken"> <see cref="CancellationToken"/> </param>
        /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="Problem"/> or <paramref name="Reporter"/> is null.
        /// </exception>
        /// <exception cref="ArithmeticException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        /// <exception cref="OperationCanceledException"></exception>
        public override void Minimize(FWParams Parameters, IOOOptProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem.LowerBounds.Count, 1);

            FirstStep(Problem);

            Progress progress = new Progress(this, 0, _parameters.Imax - 1, 0);

            Reporter.Report(progress);

            for (int i = 1; i < _parameters.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();

                NextStep(Problem);
                progress.Current = i;
                Reporter.Report(progress);
            }

            Clear();
        }
    }
}