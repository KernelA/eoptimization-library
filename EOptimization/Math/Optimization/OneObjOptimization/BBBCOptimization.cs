// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.OOOpt
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using EOpt.Math.Optimization;

    using Exceptions;

    using Help;

    using Math;
    using Math.Random;

    /// <summary>
    /// Optimization method BBBC.
    /// </summary>
    public class BBBCOptimizer : IBaseOptimizer<BBBCParams, IOOOptProblem>, IOOOptimizer<BBBCParams>
    {
        private List<Agent> _agents;

        private INormalGen _normalRand;

        private BBBCParams _parameters;

        private IContUniformGen _uniformRand;

        private KahanSum _denumKahanSum;

        /// <summary>
        /// Parameters for method.
        /// </summary>
        public BBBCParams Parameters => _parameters;

        private void Clear()
        {
            _agents.Clear();
        }

        /// <summary>
        /// Create the object which uses default implementation for random generators.
        /// </summary>
        public BBBCOptimizer() : this(new ContUniformDist(), new NormalDist())
        {
        }

        /// <summary>
        /// Create the object which uses custom implementation for random generators.
        /// </summary>
        /// <param name="UniformGen"> Object, which implements <see cref="IContUniformGen"/> interface. </param>
        /// <param name="NormalGen">  Object, which implements <see cref="INormalGen"/> interface. </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="NormalGen"/> or <paramref name="UniformGen"/> is null.
        /// </exception>
        public BBBCOptimizer(IContUniformGen UniformGen, INormalGen NormalGen)
        {
            if (NormalGen == null)
            {
                throw new ArgumentNullException(nameof(NormalGen));
            }

            _uniformRand = UniformGen ?? throw new ArgumentNullException(nameof(UniformGen));

            _normalRand = NormalGen;

            _denumKahanSum = new KahanSum();
        }

        private PointND _centerOfMass;

        private int _indexBestSol;

        private Agent _solution;

        private void EvalFunction(Func<IReadOnlyList<double>, double> Func)
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                _agents[i].Eval(Func);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="DimObjs">    </param>
        private void InitAgents(IOOOptProblem Problem, int DimObjs)
        {
            int dimension = Problem.LowerBounds.Count;

            for (int i = 0; i < _parameters.NP; i++)
            {
                PointND point = new PointND(0.0, dimension);

                for (int j = 0; j < dimension; j++)
                {
                    point[j] = _uniformRand.URandVal(Problem.LowerBounds[j], Problem.UpperBounds[j]);
                }

                _agents.Add(new Agent(point, new PointND(0.0, DimObjs)));
            }
        }

        private void FindBestSolution()
        {
            _indexBestSol = 0;
            _solution = _agents[0];

            for (int i = 1; i < _agents.Count; i++)
            {
                if (_agents[i].Objs[0] < _solution.Objs[0])
                {
                    _solution = _agents[i];
                    _indexBestSol = i;
                }
            }
        }

        private void FindCenterOfMass()
        {
            double mass = 0.0;

            _centerOfMass.MultiplyByInplace(0);

            _denumKahanSum.SumResest();

            for (int i = 0; i < _parameters.NP; i++)
            {
                mass = _agents[i].Objs[0] - _solution.Objs[0];

                if (mass < Constants.VALUE_AVOID_DIV_BY_ZERO)
                {
                    mass += Constants.VALUE_AVOID_DIV_BY_ZERO;
                }

                mass = 1 / mass;

                _denumKahanSum.Add(mass);

                for (int coordNum = 0; coordNum < _centerOfMass.Count; coordNum++)
                {
                    _centerOfMass[coordNum] += mass * _agents[i].Point[coordNum];
                }
            }

            _centerOfMass.MultiplyByInplace(1 / _denumKahanSum.Sum);
        }

        private void GenNextAgents(IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds, int IterNum)
        {
            for (int i = 0; i < _parameters.NP; i++)
            {
                if (i != _indexBestSol)
                {
                    for (int j = 0; j < _agents[i].Point.Count; j++)
                    {
                        _agents[i].Point[j] = _parameters.Beta * _centerOfMass[j] + (1 - _parameters.Beta) * Solution.Point[j] + _normalRand.NRandVal(0, 1)
                            * _parameters.Alpha * (UpperBounds[j] - LowerBounds[j]) / IterNum;

                        _agents[i].Point[j] = ClampDouble.Clamp(_agents[i].Point[j], LowerBounds[j], UpperBounds[j]);
                    }
                }
            }
        }

        private void FirstStep(IOOOptProblem Problem)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            InitAgents(Problem, 1);

            EvalFunction(Problem.TargetFunction);

            FindBestSolution();
        }

        private void Init(BBBCParams Parameters, IOOOptProblem Problem, int DimObjs)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            if (!Parameters.IsParamsInit)
            {
                throw new ArgumentException("The parameters were created by the default constructor and have invalid value\nYou need to create parameters with a custom constructor.", nameof(Parameters));
            }

            _parameters = Parameters;

            if (_agents == null)
            {
                _agents = new List<Agent>(_parameters.NP);
            }
            else
            {
                _agents.Capacity = _parameters.NP;
            }

            int dim = Problem.LowerBounds.Count;

            if (_centerOfMass == null)
            {
                _centerOfMass = new PointND(0.0, dim);
            }
            else if (_centerOfMass.Count != dim)
            {
                _centerOfMass = new PointND(0.0, dim);
            }
        }

        private void NextStep(IOOOptProblem Problem, int Iter)
        {
            FindCenterOfMass();

            GenNextAgents(Problem.LowerBounds, Problem.UpperBounds, Iter);

            EvalFunction(Problem.TargetFunction);

            FindBestSolution();
        }

        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        public Agent Solution => _solution;

        /// <summary>
        /// <see cref="IBaseOptimizer{TParams, TProblem}.Minimize(TParams, TProblem)"/>
        /// </summary>
        /// <param name="Parameters"> Parameters for method. </param>
        /// <param name="Problem">    An optimization problem. </param>
        /// <exception cref="ArgumentException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException"> If <paramref name="Problem"/> is null. </exception>
        /// <exception cref="InvalidValueFunctionException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        public void Minimize(BBBCParams Parameters, IOOOptProblem Problem)
        {
            Init(Parameters, Problem, 1);

            FirstStep(Problem);

            for (int i = 2; i <= _parameters.Imax; i++)
            {
                NextStep(Problem, i);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IBaseOptimizer{TParams, TProblem}.Minimize(TParams, TProblem, CancellationToken)"/>
        /// </summary>
        /// <param name="Parameters">  Parameters for method. </param>
        /// <param name="Problem">     An optimization problem. </param>
        /// <param name="CancelToken"> <see cref="CancellationToken"/> </param>
        /// <exception cref="ArgumentException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException"> If <paramref name="Problem"/> is null. </exception>
        /// <exception cref="InvalidValueFunctionException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        /// <exception cref="OperationCanceledException"></exception>
        public void Minimize(BBBCParams Parameters, IOOOptProblem Problem, CancellationToken CancelToken)
        {
            Init(Parameters, Problem, 1);

            FirstStep(Problem);

            for (int i = 2; i <= _parameters.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();
                NextStep(Problem, i);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IBaseOptimizer{TParams, TProblem}.Minimize(TParams, TProblem, IProgress{Progress})"/>
        /// </summary>
        /// <param name="Parameters"> Parameters for method. </param>
        /// <param name="Problem">    An optimization problem. </param>
        /// <param name="Reporter">
        /// Object which implement interface <see cref="IProgress{T}"/>, where T is <see cref="Progress"/>.
        /// </param>
        /// <exception cref="ArgumentException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="Problem"/> or <paramref name="Reporter"/> is null.
        /// </exception>
        /// <exception cref="InvalidValueFunctionException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        public void Minimize(BBBCParams Parameters, IOOOptProblem Problem, IProgress<Progress> Reporter)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem, 1);

            FirstStep(Problem);

            Progress progress = new Progress(this, 1, _parameters.Imax, 1);

            Reporter.Report(progress);

            for (int i = 2; i <= _parameters.Imax; i++)
            {
                NextStep(Problem, i);
                progress.Current = i;
                Reporter.Report(progress);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IBaseOptimizer{TParams, TProblem}.Minimize(TParams, TProblem, CancellationToken)"/>
        /// </summary>
        /// <param name="Parameters">  Parameters for method. </param>
        /// <param name="Problem">     An optimization problem. </param>
        /// <param name="Reporter">
        /// Object which implement interface <see cref="IProgress{T}"/>, where T is <see cref="Progress"/>.
        /// </param>
        /// <param name="CancelToken"> <see cref="CancellationToken"/> </param>
        /// <exception cref="ArgumentException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="Problem"/> or <paramref name="Reporter"/> is null.
        /// </exception>
        /// <exception cref="InvalidValueFunctionException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        /// <exception cref="OperationCanceledException"></exception>
        public void Minimize(BBBCParams Parameters, IOOOptProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem, 1);

            FirstStep(Problem);

            Progress progress = new Progress(this, 1, _parameters.Imax, 1);

            Reporter.Report(progress);

            for (int i = 2; i <= _parameters.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();
                NextStep(Problem, i);
                progress.Current = i;
                Reporter.Report(progress);
            }

            Clear();
        }
    }
}