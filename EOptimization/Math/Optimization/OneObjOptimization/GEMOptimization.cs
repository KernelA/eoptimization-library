// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.OOOpt
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Help;

    using Math.Random;

    /// <summary>
    /// Optimization method GEM. 
    /// </summary>
    public class GEMOptimizer : BaseGEM<OOOptimizationProblem>, IOOOptimizer<GEMParams>
    {
        private OOOptimizationProblem _problem;
        private Agent _solution;

        /// <summary>
        /// Sort by ascending. 
        /// </summary>
        private void ArrangeGrenades()
        {
            _grenades.Sort((x, y) => x.Objs[0].CompareTo(y.Objs[0]));
        }

        /// <summary>
        /// Calculate target function for the grenades. 
        /// </summary>
        private void EvalFunctionForGrenades()
        {
            for (int i = 0; i < _parameters.NGrenade; i++)
            {
                _grenades[i].Eval(EvalFuncForTransformedCoord);
            }
        }

        /// <summary>
        /// Calculate target function for the shrapnels. Shrapnels from grenade under number <paramref name="WhichGrenade"/>. 
        /// </summary>
        private void EvalFunctionForShrapnels(int WhichGrenade)
        {
            foreach (Agent shrapnel in _shrapnels[WhichGrenade])
            {
                shrapnel.Eval(EvalFuncForTransformedCoord);
            }
        }

        /// <summary>
        /// Find best solution. 
        /// </summary>
        private void FindSolution()
        {
            double fMin = _grenades[0].Objs[0];

            int indexMin = 0;

            for (int i = 1; i < _grenades.Count; i++)
            {
                if (_grenades[i].Objs[0] < fMin)
                {
                    fMin = _grenades[i].Objs[0];
                    indexMin = i;
                }
            }

            // Solution has coordinates in the range [-1; 1].
            _solution.SetAt(_grenades[indexMin]);

            for (int i = 0; i < _solution.Point.Count; i++)
            {
                _tempArray[i] = _solution.Point[i];
            }

            TransformCoord(_tempArray, _problem.LowerBounds, _problem.UpperBounds);

            for (int i = 0; i < _solution.Point.Count; i++)
            {
                _solution.Point[i] = _tempArray[i];
            }
        }

        /// <summary>
        /// Determine shrapnels position. 
        /// </summary>
        /// <param name="WhichGrenade"></param>
        /// <param name="NumIter">     </param>
        private void GenerateShrapneles(int WhichGrenade, int NumIter)
        {
            // Determine OSD and Xosd.
            if (NumIter <= 0.1 * _parameters.Imax && WhichGrenade < _parameters.DesiredMin)
            {
                base.FindOSD(WhichGrenade, NumIter, _problem.LowerBounds.Count, 1);
            }

            base.GenerateShrapnelesForGrenade(WhichGrenade, NumIter, _problem.LowerBounds.Count, 1);
        }

        protected override double EvalFuncForTransformedCoord(IReadOnlyList<double> Point)
        {
            for (int j = 0; j < Point.Count; j++)
            {
                _tempArray[j] = Point[j];
            }

            base.TransformCoord(_tempArray, _problem.LowerBounds, _problem.UpperBounds);

            return _problem.TargetFunction(_tempArray);
        }

        protected override void FirstStep()
        {
            int dimension = _problem.LowerBounds.Count;

            _radiusExplosion = 2 * Math.Sqrt(dimension);

            InitAgents(_problem.LowerBounds, _problem.UpperBounds, 1);

            EvalFunctionForGrenades();
        }

        protected override void Init(GEMParams Parameters, OOOptimizationProblem Problem, int Dimesnion, int DimObjs)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            base.Init(Parameters, Problem, Dimesnion, DimObjs);

            _problem = Problem;

            if (_solution == null)
            {
                _solution = new Agent(Dimesnion, DimObjs);
            }
            else if (_solution.Point.Count != Dimesnion)
            {
                _solution = new Agent(Dimesnion, DimObjs);
            }
        }

        protected override bool IsLessByObjs(Agent First, Agent Second)
        {
            return First.Objs[0] < Second.Objs[0];
        }

        protected override void NextStep(int Iter)
        {
            ArrangeGrenades();

            for (int j = 0; j < this._parameters.NGrenade; j++)
            {
                GenerateShrapneles(j, Iter);

                EvalFunctionForShrapnels(j);

                FindBestPosition(j);
            }

            base.UpdateParams(Iter, _problem.LowerBounds.Count);

            FindSolution();
        }

        /// <summary>
        /// The solution of the constrained optimization problem. 
        /// </summary>
        public Agent Solution => _solution;

        /// <summary>
        /// Create object which uses custom implementation for random generators. 
        /// </summary>
        public GEMOptimizer() : this(new ContUniformDist(), new NormalDist())
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
        public GEMOptimizer(IContUniformGen UniformGen, INormalGen NormalGen) : base(UniformGen, NormalGen)
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
        public override void Minimize(GEMParams Parameters, OOOptimizationProblem Problem)
        {
            Init(Parameters, Problem, Problem.LowerBounds.Count, 1);

            FirstStep();

            for (int i = 1; i <= _parameters.Imax; i++)
            {
                NextStep(i);
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
        public override void Minimize(GEMParams Parameters, OOOptimizationProblem Problem, CancellationToken CancelToken)
        {
            Init(Parameters, Problem, Problem.LowerBounds.Count, 1);

            FirstStep();

            for (int i = 1; i <= _parameters.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();
                NextStep(i);
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
        public override void Minimize(GEMParams Parameters, OOOptimizationProblem Problem, IProgress<Progress> Reporter)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }
            Init(Parameters, Problem, Problem.LowerBounds.Count, 1);

            FirstStep();

            Progress progress = new Progress(this, 1, _parameters.Imax, 1);

            Reporter.Report(progress);

            for (int i = 1; i <= _parameters.Imax; i++)
            {
                NextStep(i);
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
        public override void Minimize(GEMParams Parameters, OOOptimizationProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem, Problem.LowerBounds.Count, 1);

            FirstStep();

            Progress progress = new Progress(this, 1, _parameters.Imax, 1);

            Reporter.Report(progress);

            for (int i = 1; i <= _parameters.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();

                NextStep(i);
                progress.Current = i;
                Reporter.Report(progress);
            }

            Clear();
        }
    }
}