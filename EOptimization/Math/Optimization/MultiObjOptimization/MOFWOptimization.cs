// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.MOOpt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Help;

    using Math;
    using Math.Optimization;
    using Math.Random;

    using Nds;

    /// <summary>
    /// Optimization method Fireworks.
    /// </summary>
    public class MOFWOptimizer : BaseFW<IEnumerable<double>, IMOOptProblem>, IMOOptimizer<FWParams>
    {
        private Ndsort<double> _nds;

        private List<Agent> _chargesCopy;

        private int _iter;

        private int[] _currentFronts;

        private bool _isUseChachedFronts;

        private void EvalFunctionForCharges(Func<IReadOnlyList<double>, IEnumerable<double>> Function)
        {
            for (int i = 0; i < _parameters.NP; i++)
            {
                _chargePoints[i].Eval(Function);
            }
        }

        private void EvalFunctionForDebris(Func<IReadOnlyList<double>, IEnumerable<double>> Function)
        {
            for (int i = 0; i < _debris.Length; i++)
            {
                foreach (var splinter in _debris[i])
                {
                    splinter.Eval(Function);
                }
            }
        }

        /// <summary>
        /// Find amount debris for each point of charge.
        /// </summary>
        private void FindAmountDebris(int[] Fronts, IDictionary<int, int> CountFronts, int FMax)
        {
            double s = 0;

            int dimObjs = _chargePoints[0].Objs.Count;

            for (int i = 0; i < _parameters.NP; i++)
            {
                int countFront = CountFronts[Fronts[i]];
                // Uses binary logarithm.
                s = _parameters.M * Math.Log(1 + FMax / (Fronts[i] + 1.0)) / Constants.LN_2 * (1 - ((double)countFront) / Fronts.Length);

                base.FindAmountDebrisForCharge(s, i, dimObjs);
            }
        }

        /// <summary>
        /// Determine debris position.
        /// </summary>
        /// <param name="ChargeFronts"></param>
        /// <param name="LowerBounds"> </param>
        /// <param name="UpperBounds"> </param>
        /// <param name="Function">    </param>
        private void GenerateDebris(IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds, int[] Fronts, IDictionary<int, int> CountFronts, int FMax)
        {
            double amplitude = 0;

            for (int i = 0; i < _parameters.NP; i++)
            {
                // Amplitude of explosion. Uses binary logarithm.
                amplitude = _parameters.Amax * Math.Log(1 + (Fronts[i] + 1.0) / FMax) / Constants.LN_2 * CountFronts[Fronts[i]] / Fronts.Length;

                base.GenerateDebrisForCharge(LowerBounds, UpperBounds, amplitude, i);
            }
        }

        private void FirstMethod(IEnumerable<Agent> ChargesAndDebris, int[] Fronts)
        {
            int actualSizeMatrix = 0, totalTake = 0;

            int lengthFirstFront = Fronts.Count(fr => fr == 0);

            if (lengthFirstFront > _parameters.NP)
            {
                actualSizeMatrix = lengthFirstFront;
                totalTake = _parameters.NP;
            }
            else
            {
                // The total count minus non-dominated solutions.
                actualSizeMatrix = Fronts.Length - lengthFirstFront;
                totalTake = _parameters.NP - lengthFirstFront;
            }

            // Need to compare 'lengthLastFront' agents.
            base.ResetMatrixAndTrimWeights(actualSizeMatrix);

            int k = 0, index = 0;

            if (lengthFirstFront > _parameters.NP)
            {
                foreach (Agent agent in ChargesAndDebris)
                {
                    if (Fronts[k] == 0)
                    {
                        _weightedAgents[index++].Agent.SetAt(agent);
                    }
                    k++;
                }
            }
            else
            {
                foreach (Agent agent in ChargesAndDebris)
                {
                    if (Fronts[k] != 0)
                    {
                        _weightedAgents[index++].Agent.SetAt(agent);
                    }
                    k++;
                }
            }

            base.CalculateDistances((a, b) => PointND.Distance(a.Objs, b.Objs));

            int startIndex = 0;

            if (lengthFirstFront <= _parameters.NP)
            {
                int j = 0;

                foreach (Agent agent in ChargesAndDebris)
                {
                    // Solutions with front index equals to 'lastFrontIndex' are taken.
                    if (Fronts[j] == 0)
                    {
                        _chargesCopy[startIndex++].SetAt(agent);
                    }
                    j++;
                }
            }

            base.TakeAgents(actualSizeMatrix, totalTake);

            for (int i = 0; i < _weightedAgents.Count; i++)
            {
                if (_weightedAgents[i].IsTake)
                {
                    _chargesCopy[startIndex++].SetAt(_weightedAgents[i].Agent);
                }
            }
        }

        private void SecondMethod(IEnumerable<Agent> ChargesAndDebris, int[] Fronts)
        {
        }

        /// <summary>
        /// Generate current population.
        /// </summary>
        private void GenerateNextAgents(IEnumerable<Agent> ChargesAndDebris, int[] Fronts)
        {
            int lengthLastFront = 0, totalTaken = 0, maxFront = Fronts.Max();

            // If 'lastFrontIndex' is equal to 'maxFront + 1' than no need an additional selection.
            int lastFrontIndex = maxFront + 1;

            for (int front = 0; front <= maxFront; front++)
            {
                lengthLastFront = Fronts.Count(fr => fr == front);

                if (totalTaken + lengthLastFront <= _parameters.NP)
                {
                    totalTaken += lengthLastFront;
                }
                else
                {
                    lastFrontIndex = front;
                    break;
                }
            }

            int needToTake = _parameters.NP - totalTaken;

            if (needToTake > 0)
            {
                // Need to compare 'lengthLastFront' agents.
                base.ResetMatrixAndTrimWeights(lengthLastFront);
            }

            int k = 0, index = 0, startIndex = 0;

            foreach (Agent agent in ChargesAndDebris)
            {
                if (Fronts[k] == lastFrontIndex && needToTake != 0)
                {
                    _weightedAgents[index++].Agent.SetAt(agent);
                }
                else if (Fronts[k] < lastFrontIndex)
                {
                    _chargesCopy[startIndex].SetAt(agent);
                    _currentFronts[startIndex++] = Fronts[k];
                }

                k++;
            }

            if (needToTake > 0)
            {
                base.CalculateDistances((a, b) => PointND.Distance(a.Objs, b.Objs));
                base.TakeAgents(lengthLastFront, needToTake);

                for (int i = 0; i < _weightedAgents.Count; i++)
                {
                    if (_weightedAgents[i].IsTake)
                    {
                        _chargesCopy[startIndex].SetAt(_weightedAgents[i].Agent);
                        // _weightedAgents always have 'lastFronIndex' as an index of front.
                        _currentFronts[startIndex++] = lastFrontIndex;
                    }
                }
            }

            for (int i = 0; i < _chargesCopy.Count; i++)
            {
                _chargePoints[i].SetAt(_chargesCopy[i]);
            }
        }

        protected override void Clear()
        {
        }

        protected override void FirstStep(IMOOptProblem Problem)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            InitAgents(Problem.LowerBounds, Problem.UpperBounds, Problem.CountObjs);

            EvalFunctionForCharges(Problem.TargetFunction);
        }

        protected override void NextStep(IMOOptProblem Problem)
        {
            int[] fronts = null;

            if (_isUseChachedFronts)
            {
                fronts = _currentFronts;
            }
            else
            {
                fronts = _nds.NonDominSort(_chargePoints.Select(item => item.Objs));
                _isUseChachedFronts = true;
            }

            var countFronts = fronts.GroupBy(frontIndex => frontIndex).ToDictionary(item => item.Key, item => item.Count());

            int maxFront = countFronts.Keys.Max() + 1;

            FindAmountDebris(fronts, countFronts, maxFront);

            GenerateDebris(Problem.LowerBounds, Problem.UpperBounds, fronts, countFronts, maxFront);

            EvalFunctionForDebris(Problem.TargetFunction);

            var allAgents = _chargePoints.Concat(_debris.SelectMany(coll => coll.Select(agent => agent)));

            int[] allFronts = _nds.NonDominSort(allAgents.Select(agent => agent.Objs));

            GenerateNextAgents(allAgents, allFronts);

            EvalFunctionForCharges(Problem.TargetFunction);
        }

        protected override void Init(FWParams Parameters, int Dim, int DimObjs)
        {
            base.Init(Parameters, Dim, DimObjs);

            if (_chargesCopy == null)
            {
                _chargesCopy = new List<Agent>(Parameters.NP);

                for (int i = 0; i < Parameters.NP; i++)
                {
                    _chargesCopy.Add(_pool.GetAgent());
                }
            }
            else if (_chargesCopy.Count != Parameters.NP)
            {
                _chargesCopy.Clear();

                _chargesCopy.Capacity = Parameters.NP;

                for (int i = 0; i < Parameters.NP; i++)
                {
                    _chargesCopy.Add(_pool.GetAgent());
                }
            }

            _currentFronts = new int[Parameters.NP];
        }

        public IEnumerable<Agent> ParetoFront => _chargePoints;

        /// <summary>
        /// Create object which uses default implementation for random generators.
        /// </summary>
        public MOFWOptimizer() : this(new ContUniformDist(), new NormalDist())
        {
        }

        /// <summary>
        /// Create object which uses custom implementation for random generators.
        /// </summary>
        /// <param name="UniformGen">
        /// Object, which implements <see cref="IContUniformGen"/> interface.
        /// </param>
        /// <param name="NormalGen">  Object, which implements <see cref="INormalGen"/> interface. </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="NormalGen"/> or <paramref name="UniformGen"/> is null.
        /// </exception>
        public MOFWOptimizer(IContUniformGen UniformGen, INormalGen NormalGen) : base(UniformGen, NormalGen)
        {
            _nds = new Ndsort<double>(CmpDouble.DoubleCompare);
            _isUseChachedFronts = false;
        }

        /// <summary>
        /// <see cref="IOOOptimizer{T}.Minimize(GeneralParams)"/>
        /// </summary>
        /// <param name="GenParams"> General parameters. <see cref="GeneralParams"/>. </param>
        /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException"> If <paramref name="GenParams"/> is null. </exception>
        /// <exception cref="ArithmeticException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        public override void Minimize(FWParams Parameters, IMOOptProblem Problem)
        {
            Init(Parameters, Problem.LowerBounds.Count, Problem.CountObjs);

            FirstStep(Problem);

            for (int i = 1; i < _parameters.Imax; i++)
            {
                _iter = i;
                NextStep(Problem);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IOOOptimizer{T}.Minimize(GeneralParams, CancellationToken)"/>
        /// </summary>
        /// <param name="GenParams">   General parameters. <see cref="GeneralParams"/>. </param>
        /// <param name="CancelToken"> <see cref="CancellationToken"/> </param>
        /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException"> If <paramref name="GenParams"/> is null. </exception>
        /// <exception cref="ArithmeticException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        /// <exception cref="OperationCanceledException"></exception>
        public override void Minimize(FWParams Parameters, IMOOptProblem Problem, CancellationToken CancelToken)
        {
            Init(Parameters, Problem.LowerBounds.Count, Problem.CountObjs);

            FirstStep(Problem);

            for (int i = 1; i < this._parameters.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();
                NextStep(Problem);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IOOOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// </summary>
        /// <param name="GenParams"> General parameters. <see cref="GeneralParams"/>. </param>
        /// <param name="Reporter">
        /// Object which implement interface <see cref="IProgress{T}"/>, where T is
        /// <see cref="Progress"/>. <seealso cref="IOOOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// </param>
        /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="GenParams"/> or <paramref name="Reporter"/> is null.
        /// </exception>
        /// <exception cref="ArithmeticException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        public override void Minimize(FWParams Parameters, IMOOptProblem Problem, IProgress<Progress> Reporter)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem.LowerBounds.Count, Problem.CountObjs);

            FirstStep(Problem);

            Progress progress = new Progress(this, 0, this._parameters.Imax - 1, 0);

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
        /// <see cref="IOOOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// </summary>
        /// <param name="GenParams"> General parameters. <see cref="GeneralParams"/>. </param>
        /// <param name="Reporter">
        /// Object which implement interface <see cref="IProgress{T}"/>, where T is
        /// <see cref="Progress"/>.
        /// <seealso cref="IOOOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/><param name="CancelToken"> <see cref="CancellationToken"/></param>
        /// </param>
        /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="GenParams"/> or <paramref name="Reporter"/> is null.
        /// </exception>
        /// <exception cref="ArithmeticException">
        /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
        /// </exception>
        /// <exception cref="OperationCanceledException"></exception>
        public override void Minimize(FWParams Parameters, IMOOptProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem.LowerBounds.Count, Problem.CountObjs);

            FirstStep(Problem);

            Progress progress = new Progress(this, 0, this._parameters.Imax - 1, 0);

            Reporter.Report(progress);

            for (int i = 1; i < this._parameters.Imax; i++)
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