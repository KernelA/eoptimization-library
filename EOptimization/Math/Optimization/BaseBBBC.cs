// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using EOpt.Exceptions;
    using EOpt.Help;
    using EOpt.Math.Random;

    /// <summary>
    /// Base class for the BBBC method. 
    /// </summary>
    /// <typeparam name="TProblem"></typeparam>
    public abstract class BBBBC<TObj, TProblem> : IBaseOptimizer<BBBCParams, TProblem> where TProblem : IConstrOptProblem<double, TObj>
    {
        protected List<Agent> _agents;

        protected INormalGen _normalRand;

        protected BBBCParams _parameters;

        protected IContUniformGen _uniformRand;

        protected KahanSum _denumKahanSum;

        protected virtual void Clear()
        {
            _agents.Clear();
        }

        protected abstract void FirstStep(TProblem Problem);

        /// <summary>
        /// </summary>
        /// <param name="Parameters"></param>
        protected virtual void Init(BBBCParams Parameters, TProblem Problem, int DimObjs)
        {
            if (!Parameters.IsParamsInit)
            {
                throw new ArgumentException("The parameters were created by the default constructor and have invalid values.\n" +
                    "You need to create parameters with a custom constructor.", nameof(Parameters));
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
        }

        /// <summary>
        /// </summary>
        /// <param name="DimObjs">    </param>
        protected virtual void InitAgents(TProblem Problem, int DimObjs)
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

        protected abstract void NextStep(TProblem Problem, int Iter);

        /// <summary>
        /// Parameters for method. 
        /// </summary>
        public BBBCParams Parameters => _parameters;

        /// <summary>
        /// Create the object which uses default implementation for random generators. 
        /// </summary>
        public BBBBC() : this(new ContUniformDist(), new NormalDist())
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
        public BBBBC(IContUniformGen UniformGen, INormalGen NormalGen)
        {
            if (UniformGen == null)
            {
                throw new ArgumentNullException(nameof(UniformGen));
            }

            if (NormalGen == null)
            {
                throw new ArgumentNullException(nameof(NormalGen));
            }

            _uniformRand = UniformGen;

            _normalRand = NormalGen;

            _denumKahanSum = new KahanSum();
        }

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
        public abstract void Minimize(BBBCParams Parameters, TProblem Problem);

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
        public abstract void Minimize(BBBCParams Parameters, TProblem Problem, CancellationToken CancelToken);

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
        public abstract void Minimize(BBBCParams Parameters, TProblem Problem, IProgress<Progress> Reporter);

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
        public abstract void Minimize(BBBCParams Parameters, TProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken);
    }
}