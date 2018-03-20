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
    public class GEMOptimizer : BaseGEM<double, IOOOptProblem>, IOOOptimizer<GEMParams>
    {
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
                shrapnel.Eval(_targetFuncWithTransformedCoords);
            }
        }

        /// <summary>
        /// Find best solution. 
        /// </summary>
        private void FindSolution(IOOOptProblem Problem)
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

            TransformCoord(_tempArray, Problem.LowerBounds, Problem.UpperBounds);

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
        private void GenerateShrapneles(IOOOptProblem Problem, int WhichGrenade, int NumIter)
        {
            // Determine OSD and Xosd.
            if (NumIter <= 0.1 * _parameters.Imax && WhichGrenade < _parameters.DesiredMin)
            {
                base.FindOSD(WhichGrenade, NumIter, Problem.LowerBounds.Count, 1, (a, b) => a.Objs[0] < b.Objs[0]);
            }

            base.GenerateShrapnelesForGrenade(WhichGrenade, NumIter, Problem.LowerBounds.Count, 1);
        }



        protected override void FirstStep(IOOOptProblem Problem)
        {
            int dimension = Problem.LowerBounds.Count;

            _radiusExplosion = 2 * Math.Sqrt(dimension);

            InitAgents(Problem.LowerBounds, Problem.UpperBounds, 1);

            EvalFunctionForGrenades();
        }

        protected override void Init(GEMParams Parameters, IOOOptProblem Problem, int DimObjs)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            base.Init(Parameters, Problem, DimObjs);

            int dim = Problem.LowerBounds.Count;

            if (_solution == null)
            {
                _solution = new Agent(dim, DimObjs);
            }
            else if (_solution.Point.Count != dim)
            {
                _solution = new Agent(dim, DimObjs);
            }
        }

        protected override void EvalTempAgent(Agent Temp)
        {
            Temp.Eval(_targetFuncWithTransformedCoords);
        }


        protected override void NextStep(IOOOptProblem Problem, int Iter)
        {
            ArrangeGrenades();

            for (int j = 0; j < this._parameters.NGrenade; j++)
            {
                GenerateShrapneles(Problem, j, Iter);

                EvalFunctionForShrapnels(j);

                FindBestPosition(j, (a,b) => a.Objs[0] < b.Objs[0]);
            }

            base.UpdateParams(Iter, Problem.LowerBounds.Count);

            FindSolution(Problem);
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
        public override void Minimize(GEMParams Parameters, IOOOptProblem Problem)
        {
            Init(Parameters, Problem, 1);

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
        public override void Minimize(GEMParams Parameters, IOOOptProblem Problem, CancellationToken CancelToken)
        {
            Init(Parameters, Problem, 1);

            FirstStep(Problem);

            for (int i = 1; i <= _parameters.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();
                NextStep(Problem, i);
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
        public override void Minimize(GEMParams Parameters, IOOOptProblem Problem, IProgress<Progress> Reporter)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem, 1);

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
        public override void Minimize(GEMParams Parameters, IOOOptProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem, 1);

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