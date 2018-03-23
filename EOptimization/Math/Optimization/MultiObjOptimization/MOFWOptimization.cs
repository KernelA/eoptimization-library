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

        private void EvalFunction(Func<IReadOnlyList<double>, IEnumerable<double>> Function)
        {
            for (int i = 0; i < _parameters.NP; i++)
            {
                _chargePoints[i].Eval(Function);
            }
        }

        private void EvalFunctionForCharges(Func<IReadOnlyList<double>, IEnumerable<double>> Function)
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
        private void FindAmountDebris(int[] ChargeFronts)
        {
            double s = 0.0;

            int dimObjs = _chargePoints[0].Objs.Count;

            for (int i = 0; i < _parameters.NP; i++)
            {
                s = _parameters.M * Math.Pow(Math.E, -0.1 * ChargeFronts[i]);

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
        private void GenerateDebris(int[] ChargeFronts, IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds, Func<IReadOnlyList<double>, IEnumerable<double>> Function)
        {
            double amplitude = 0;

            for (int i = 0; i < _parameters.NP; i++)
            {
                // Amplitude of explosion.
                amplitude = _parameters.Amax * Math.Tanh(0.1 * (ChargeFronts[i] + 1));

                base.GenerateDebrisForCharge(LowerBounds, UpperBounds, amplitude, i);
            }
        }

        /// <summary>
        /// Generate current population. 
        /// </summary>
        private void GenerateNextAgents(int[] ChargeAndDebrisFronts, IEnumerable<Agent> FirstFront)
        {
            int firstFrontCount = ChargeAndDebrisFronts.Count(front => front == 0);

            // The total count minus non-dominated solutions.
            int actualSizeMatrix = _chargePoints.Count - firstFrontCount;

            for (int k = 0; k < _debris.Length; k++)
            {
                actualSizeMatrix += _debris[k].Count;
            }

            base.ResetMatrixAndTrimWeights(actualSizeMatrix);

            {
                int index = 0;
                int total = 0;

                for (int i = 0; i < _chargePoints.Count; i++)
                {
                    // Skip non-dominated solutions.
                    if (ChargeAndDebrisFronts[total] != 0)
                    {
                        _weightedAgents[index].Agent.SetAt(_chargePoints[i]);
                        index++;
                    }
                    total++;
                }

                for (int i = 0; i < _debris.Length; i++)
                {
                    foreach (Agent splinter in _debris[i])
                    {
                        if (ChargeAndDebrisFronts[total] != 0)
                        {
                            _weightedAgents[index].Agent.SetAt(splinter);
                            index++;
                        }

                        total++;
                    }
                }
            }

            base.CalculateDistances();

            int startIndex = 0;

            foreach (var item in FirstFront)
            {
                if (startIndex < _parameters.NP)
                {
                    _chargePoints[startIndex++].SetAt(item);
                }
            }

            int totalToTake = _parameters.NP - firstFrontCount;

            if (totalToTake > 0)
            {
                base.TakeAgents(actualSizeMatrix, totalToTake);
            }

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

            EvalFunction(Problem.TargetFunction);

            int[] chargeFronts = _nds.NonDominSort(_chargePoints, item => item.Objs);

            FindAmountDebris(chargeFronts);

            GenerateDebris(chargeFronts, Problem.LowerBounds, Problem.UpperBounds, Problem.TargetFunction);

            EvalFunctionForCharges(Problem.TargetFunction);
        }

        protected override void NextStep(IMOOptProblem Problem)
        {
            var allAgents = _chargePoints.Concat(_debris[0]);

            for (int i = 1; i < _debris.Length; i++)
            {
                allAgents = allAgents.Concat(_debris[i]);
            }

            int[] allFronts = _nds.NonDominSort(allAgents, item => item.Objs);

            LinkedList<Agent> firstFront = new LinkedList<Agent>();

            int index = 0;

            foreach (var item in allAgents)
            {
                if (allFronts[index] == 0)
                {
                    firstFront.AddLast(item);
                }
                index++;
            }

            GenerateNextAgents(allFronts, firstFront);

            EvalFunction(Problem.TargetFunction);

            int[] chargeFronts = _nds.NonDominSort(_chargePoints, item => item.Objs);

            FindAmountDebris(chargeFronts);

            GenerateDebris(chargeFronts, Problem.LowerBounds, Problem.UpperBounds, Problem.TargetFunction);

            EvalFunctionForCharges(Problem.TargetFunction);
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