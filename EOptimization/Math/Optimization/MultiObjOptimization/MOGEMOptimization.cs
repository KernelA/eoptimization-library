// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.MOOpt
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;

    using Help;

    using Math.Random;

    using Nds;

    /// <summary>
    /// Optimization method GEM. 
    /// </summary>
    public class MOGEMOptimizer : BaseGEM<IEnumerable<double>, IMOOptProblem>, IMOOptimizer<GEMParams>
    {
        private Ndsort<double> _nds;

        private class IndexFront : IComparable<IndexFront>
        {
            public Agent Agent { get; set; }

            public int Front { get; set; }

            public IndexFront(Agent Agent, int Front)
            {
                this.Agent = Agent;
                this.Front = Front;
            }

            public int CompareTo(IndexFront Other)
            {
                return Front.CompareTo(Other.Front);
            }
        }

        private IndexFront[] _idxFronts;
        private int _ndcount;

        public int NDCount => _ndcount;

        /// <summary>
        /// Calculate target function for the grenades. 
        /// </summary>
        private void EvalFunctionForGrenades()
        {
            for (int i = 0; i < _parameters.NGrenade; i++)
            {
                _grenades[i].Eval(base._targetFuncWithTransformedCoords);
            }
        }

        /// <summary>
        /// Calculate target function for the shrapnels. Shrapnels from grenade under number <paramref name="WhichGrenade"/>. 
        /// </summary>
        private void EvalFunctionForShrapnels(int WhichGrenade)
        {
            foreach (Agent shrapnel in _shrapnels[WhichGrenade])
            {   
                shrapnel.Eval(base._targetFuncWithTransformedCoords);
            }
        }

        protected override void EvalTempAgent(Agent Temp)
        {
            Temp.Eval(_targetFuncWithTransformedCoords);
        }


        /// <summary>
        /// Determine shrapnels position. 
        /// </summary>
        /// <param name="WhichGrenade"></param>
        /// <param name="NumIter">     </param>
        private void GenerateShrapneles(IMOOptProblem Problem, int WhichGrenade, int NumIter, bool IsFirstFront)
        {
            // Determine OSD and Xosd.
            if (NumIter <= 0.1 * _parameters.Imax && IsFirstFront)
            {
                base.FindOSD(WhichGrenade, NumIter, Problem.LowerBounds.Count, Problem.CountObjs, (a, b) => Nds.Tools.Stools.IsDominate(a.Objs, b.Objs, CmpDouble.DoubleCompare));
            }

            base.GenerateShrapnelesForGrenade(WhichGrenade, NumIter, Problem.LowerBounds.Count, Problem.CountObjs);
        }



        protected override void FirstStep(IMOOptProblem Problem)
        {
            int dimension = Problem.LowerBounds.Count;

            _radiusExplosion = 2 * Math.Sqrt(dimension);

            InitAgents(Problem.LowerBounds, Problem.UpperBounds, Problem.CountObjs);

            EvalFunctionForGrenades();
        }

        protected override void Init(GEMParams Parameters, IMOOptProblem Problem, int DimObjs)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            base.Init(Parameters, Problem, DimObjs);

            _idxFronts = new IndexFront[_parameters.NGrenade];

            for (int i = 0; i < _idxFronts.Length; i++)
            {
                _idxFronts[i] = new IndexFront(null, 0);
            }
        }

        protected override void NextStep(IMOOptProblem Problem, int Iter)
        {
            int[] grenFronts = _nds.NonDominSort(_grenades, a => a.Objs);

            _ndcount = grenFronts.Count(a => a == 0);


            for (int i = 0; i < grenFronts.Length; i++)
            {
                _idxFronts[i].Front = grenFronts[i];
                _idxFronts[i].Agent = _grenades[i];
            }

            Array.Sort(_idxFronts);

            for (int i = 0; i < _idxFronts.Length; i++)
            {
                _grenades[i] = _idxFronts[i].Agent;
            }

            for (int j = 0; j < _parameters.NGrenade; j++)
            {
                GenerateShrapneles(Problem, j, Iter, grenFronts[j] == 0);

                EvalFunctionForShrapnels(j);

                FindBestPosition(j, (a, b) => Nds.Tools.Stools.IsDominate(a.Objs, b.Objs, CmpDouble.DoubleCompare));
            }

            base.UpdateParams(Iter, Problem.LowerBounds.Count);
        }

        /// <summary>
        /// The solution of the constrained optimization problem. 
        /// </summary>
        public IEnumerable<Agent> ParetoFront => _grenades;

        /// <summary>
        /// Create object which uses custom implementation for random generators. 
        /// </summary>
        public MOGEMOptimizer() : this(new ContUniformDist(), new NormalDist())
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
        public MOGEMOptimizer(IContUniformGen UniformGen, INormalGen NormalGen) : base(UniformGen, NormalGen)
        {
            _nds = new Ndsort<double>(CmpDouble.DoubleCompare);
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
        public override void Minimize(GEMParams Parameters, IMOOptProblem Problem)
        {
            Init(Parameters, Problem, Problem.CountObjs);

            FirstStep(Problem);

            for (int i = 1; i <= _parameters.Imax; i++)
            {
                NextStep(Problem, i);
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
        public override void Minimize(GEMParams Parameters, IMOOptProblem Problem, CancellationToken CancelToken)
        {
            Init(Parameters, Problem, Problem.CountObjs);

            FirstStep(Problem);

            for (int i = 1; i <= _parameters.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();
                NextStep(Problem, i);
            }
            Clear();
        }

        protected override void Clear()
        {

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
        public override void Minimize(GEMParams Parameters, IMOOptProblem Problem, IProgress<Progress> Reporter)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem, Problem.CountObjs);

            FirstStep(Problem);

            Progress progress = new Progress(this, 1, _parameters.Imax, 1);

            Reporter.Report(progress);

            for (int i = 1; i <= _parameters.Imax; i++)
            {
                NextStep(Problem, i);
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
        public override void Minimize(GEMParams Parameters, IMOOptProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem, Problem.CountObjs);

            FirstStep(Problem);

            Progress progress = new Progress(this, 1, _parameters.Imax, 1);

            Reporter.Report(progress);

            for (int i = 1; i <= _parameters.Imax; i++)
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