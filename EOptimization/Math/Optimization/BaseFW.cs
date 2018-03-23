// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Linq;

    using EOpt.Help;
    using EOpt.Math.Random;

    using Math.LA;

    /// <summary>
    /// Base class for the FW method. 
    /// </summary>
    /// <typeparam name="TProblem"></typeparam>
    public abstract class BaseFW<TObj, TProblem> : IBaseOptimizer<FWParams, TProblem> where TProblem : IConstrOptProblem<double, TObj>
    {
        /// <summary>
        /// Charges. 
        /// </summary>
        protected List<Agent> _chargePoints;

        // Need in the method 'GenerateIndexesOfAxes'.
        protected int[] _coordNumbers;

        /// <summary>
        /// Debris for charges. 
        /// </summary>
        protected LinkedList<Agent>[] _debris;

        protected DynSymmetricMatrix _matrixOfDistances;

        protected int _minDebrisCount, _maxDebrisCount;

        protected double _minS, _maxS;

        protected INormalGen _normalRand;

        protected FWParams _parameters;

        protected IContUniformGen _uniformRand;

        protected List<WeightOfAgent> _weightedAgents;

        protected KahanSum _distKahanSum, _denumForProbKahanSum;

        /// <summary>
        /// Class for internal computation. 
        /// </summary>
        protected class WeightOfAgent : IComparable<WeightOfAgent>
        {
            public Agent Agent { get; private set; }

            public bool IsTake { get; set; }

            public double Weight { get; set; }

            public WeightOfAgent(Agent Agent, double Dist)
            {
                this.Agent = Agent;
                this.Weight = Dist;
                this.IsTake = false;
            }

            public int CompareTo(WeightOfAgent Other)
            {
                if (Weight < Other.Weight)
                {
                    return -1;
                }
                else if (Weight > Other.Weight)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }


        protected void CalculateDistances()
        {
            _denumForProbKahanSum.SumResest();

            // Calculate distance between all points.
            for (int i = 0; i < _matrixOfDistances.RowCount; i++)
            {
                for (int j = i + 1; j < _matrixOfDistances.ColumnCount; j++)
                {
                    _matrixOfDistances[i, j] = PointND.Distance(_weightedAgents[i].Agent.Point, _weightedAgents[j].Agent.Point);
                }
            }

            for (int ii = 0; ii < _matrixOfDistances.RowCount; ii++)
            {
                _distKahanSum.SumResest();

                for (int j = 0; j < _matrixOfDistances.ColumnCount; j++)
                {
                    _distKahanSum.Add(_matrixOfDistances[ii, j]);
                }

                _weightedAgents[ii].Weight = _distKahanSum.Sum;

                _denumForProbKahanSum.Add(_distKahanSum.Sum);
            }

            // Probability of explosion.
            for (int jj = 0; jj < _weightedAgents.Count; jj++)
            {
                _weightedAgents[jj].Weight /= _denumForProbKahanSum.Sum;

                if (CheckDouble.GetTypeValue(_weightedAgents[jj].Weight) != DoubleTypeValue.Valid)
                {
                    _weightedAgents[jj].Weight = 1.0 / _matrixOfDistances.ColumnCount;
                }
            }
        }

        protected virtual void Clear()
        {
            _chargePoints.Clear();

            for (int i = 0; i < _debris.Length; i++)
            {
                _debris[i].Clear();
            }
        }

        protected void FindAmountDebrisForCharge(double S, int WhicCharge, int DimObjs)
        {
            int countDebris = 0;

            if (S < _minS)
            {
                countDebris = _minDebrisCount;
            }
            else if (S > _maxS)
            {
                countDebris = _maxDebrisCount;
            }
            else
            {
                countDebris = (int)Math.Truncate(S);
            }

            if (countDebris == 0)
            {
                _debris[WhicCharge].Clear();
            }
            else if (_debris[WhicCharge].Count > countDebris)
            {
                int del = _debris[WhicCharge].Count - countDebris;

                while (del > 0)
                {
                    _debris[WhicCharge].RemoveLast();
                    del--;
                }
            }
            else if (_debris[WhicCharge].Count < countDebris)
            {
                int total = countDebris - _debris[WhicCharge].Count;

                int dimension = _chargePoints[0].Point.Count;

                while (total > 0)
                {
                    _debris[WhicCharge].AddLast(new Agent(dimension, DimObjs));
                    total--;
                }
            }
        }

        /// <summary>
        /// First method for determination of position of the debris. 
        /// </summary>
        /// <param name="Splinter">        </param>
        /// <param name="CountOfDimension"></param>
        /// <param name="Amplitude">       </param>
        /// <param name="LowerBounds">     </param>
        /// <param name="UpperBounds">     </param>
        protected void FirstMethodDeterminationOfPosition(Agent Splinter, int CountOfDimension, double Amplitude, IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds)
        {
            // The indices are choosing randomly.
            GenerateIndexesOfAxes();

            double h = 0;

            // Calculate position of debris.
            for (int i = 0; i < CountOfDimension; i++)
            {
                int axisIndex = _coordNumbers[i];

                h = Amplitude * _uniformRand.URandVal(-1, 1);

                Splinter.Point[axisIndex] += h;

                Splinter.Point[axisIndex] = ClampDouble.Clamp(Splinter.Point[axisIndex], LowerBounds[axisIndex], UpperBounds[axisIndex]);
            }
        }

        protected abstract void FirstStep(TProblem Problem);

        protected void GenerateDebrisForCharge(IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds, double Amplitude, int WhichCharge)
        {
            double ksi = 0;

            // For each debris.
            foreach (Agent splinter in _debris[WhichCharge])
            {
                // The position of debris sets to the position of charge.
                splinter.Point.SetAt(_chargePoints[WhichCharge].Point);

                ksi = _uniformRand.URandVal(0, 1);

                int CountOfDimension = (int)Math.Ceiling(LowerBounds.Count * ksi);

                if (ksi < 0.5)
                {
                    FirstMethodDeterminationOfPosition(splinter, CountOfDimension, Amplitude, LowerBounds, UpperBounds);
                }
                else
                {
                    SecondMethodDeterminationOfPosition(splinter, CountOfDimension, LowerBounds, UpperBounds);
                }
            }
        }

        /// <summary>
        /// Generate randomly indices of axes. 
        /// </summary>
        /// <returns></returns>
        protected void GenerateIndexesOfAxes()
        {
            // Set coordinate numbers.
            for (int i = 0; i < _coordNumbers.Length; i++)
            {
                _coordNumbers[i] = i;
            }

            Сombinatorics.RandomPermutation(_coordNumbers, SyncRandom.Get());
        }

        protected virtual void Init(FWParams Parameters, int Dim, int DimObjs)
        {
            if (!Parameters.IsParamsInit)
            {
                throw new ArgumentException("The parameters were created by the default constructor and have invalid values.\n" +
                    "You need to create parameters with a custom constructor.", nameof(Parameters));
            }

            _parameters = Parameters;

            _minS = _parameters.Alpha * _parameters.M;
            _maxS = _parameters.Beta * _parameters.M;
            _minDebrisCount = (int)Math.Truncate(_minS);
            _maxDebrisCount = (int)Math.Truncate(_maxS);

            if (_coordNumbers == null)
            {
                _coordNumbers = new int[Dim];
            }
            else if (_coordNumbers.Length != Dim)
            {
                _coordNumbers = new int[Dim];
            }

            if (_chargePoints == null)
            {
                _chargePoints = new List<Agent>(_parameters.NP);
            }
            else
            {
                _chargePoints.Capacity = _parameters.NP;
            }

            if (_debris == null)
            {
                InitDebris();
            }
            else if (_debris.Length != this.Parameters.NP)
            {
                InitDebris();
            }

            int newSizeMatrix = checked(_parameters.NP - 1 + _parameters.NP * _minDebrisCount);

            _weightedAgents = new List<WeightOfAgent>(newSizeMatrix);

            //for (int i = 0; i < _weights.Count; i++)
            //{
            //    // The structure is storing an agent and its probability of choosing.
            //    _weights[i] = new WeightOfAgent(new Agent(Dim, DimObjs), 0.0);
            //}

            if (_matrixOfDistances == null)
            {
                _matrixOfDistances = new DynSymmetricMatrix(newSizeMatrix);
            }
            else if (_matrixOfDistances.RowCount != newSizeMatrix)
            {
                _matrixOfDistances.RowCount = newSizeMatrix;
            }
        }

        protected virtual void InitAgents(IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds, int DimObjs)
        {
            int dimension = LowerBounds.Count;

            // Create points of explosion.
            for (int i = 0; i < _parameters.NP; i++)
            {
                PointND point = new PointND(0.0, dimension);

                for (int j = 0; j < dimension; j++)
                {
                    point[j] = _uniformRand.URandVal(LowerBounds[j], UpperBounds[j]);
                }

                _chargePoints.Add(new Agent(point, new PointND(0.0, DimObjs)));
            }
        }

        protected void InitDebris()
        {
            _debris = new LinkedList<Agent>[_parameters.NP];

            for (int i = 0; i < _debris.Length; i++)
            {
                _debris[i] = new LinkedList<Agent>();
            }
        }

        protected abstract void NextStep(TProblem Problem);

        protected void ResetMatrixAndTrimWeights(int NewSize)
        {
            _matrixOfDistances.ColumnCount = NewSize;

            for (int row = 0; row < _matrixOfDistances.RowCount; row++)
            {
                for (int column = 0; column < _matrixOfDistances.ColumnCount; column++)
                {
                    _matrixOfDistances[row, column] = 0;
                }
            }

            if(NewSize > _weightedAgents.Count)
            {
                int countAdd = NewSize - _weightedAgents.Count;

                while(countAdd > 0)
                {
                    _weightedAgents.Add(new WeightOfAgent(new Agent(_chargePoints[0].Point.Count, _chargePoints[0].Objs.Count), 0.0));
                    countAdd--;
                }
            }
            else if(_weightedAgents.Count > NewSize)
            {
                _weightedAgents.RemoveRange(NewSize, _weightedAgents.Count - NewSize);
            }

            for (int i = 0; i < _weightedAgents.Count; i++)
            {
                _weightedAgents[i].Weight = 0;
                _weightedAgents[i].IsTake = false;
            }
        }

        /// <summary>
        /// Second method for determination of position of the debris. 
        /// </summary>
        /// <param name="Splinter">        </param>
        /// <param name="CountOfDimension"></param>
        /// <param name="LowerBounds">     </param>
        /// <param name="UpperBounds">     </param>
        protected void SecondMethodDeterminationOfPosition(Agent Splinter, int CountOfDimension, IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds)
        {
            GenerateIndexesOfAxes();

            double g = 0;

            int axisIndex = 0;

            // Calculate position of debris.
            for (int i = 0; i < CountOfDimension; i++)
            {
                axisIndex = _coordNumbers[i];

                g = _normalRand.NRandVal(1, 1);

                Splinter.Point[axisIndex] *= g;

                Splinter.Point[axisIndex] = ClampDouble.Clamp(Splinter.Point[axisIndex], LowerBounds[axisIndex], UpperBounds[axisIndex]);
            }
        }


        /// <summary>
        /// <para>
        /// Weighted Random Sampling without replacement.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The original algorithm described in the Efraimidis, P.S., & Spirakis, P.G. (2007). Weighted Random Sampling (2005; Efraimidis, Spirakis).
        /// </remarks>
        /// <param name="Weights"></param>
        /// <param name="ActualSize"></param>
        /// <param name="TotalToTake"></param>
        protected void TakeAgents(int ActualSize, int TotalToTake)
        {
            int count = 0;

            for (int i = 0; i < _weightedAgents.Count; i++)
            {
                _weightedAgents[i].Weight = Math.Pow(_uniformRand.URandVal(0, 1), 1 / _weightedAgents[i].Weight);
            }

            // Sort by descending.
            _weightedAgents.Sort((a, b) => -a.CompareTo(b));

            for (int i = 0; i < TotalToTake; i++)
            {
                _weightedAgents[i].IsTake = true;
            }
        }

        /// <summary>
        /// Parameters for method. 
        /// </summary>
        public FWParams Parameters => _parameters;

        /// <summary>
        /// Create the object which uses default implementation for random generators. 
        /// </summary>
        public BaseFW() : this(new ContUniformDist(), new NormalDist())
        {
        }

        /// <summary>
        /// Create the object which uses custom implementation for random generators. 
        /// </summary>
        /// <param name="UniformGen">
        /// Object, which implements <see cref="IContUniformGen"/> interface.
        /// </param>
        /// <param name="NormalGen">  Object, which implements <see cref="INormalGen"/> interface. </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="NormalGen"/> or <paramref name="UniformGen"/> is null.
        /// </exception>
        public BaseFW(IContUniformGen UniformGen, INormalGen NormalGen)
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

            _distKahanSum = new KahanSum();
            _denumForProbKahanSum = new KahanSum();
        }

        /// <summary>
        /// <see cref="IBaseOptimizer{TParams, TProblem}.Minimize(TParams, TProblem)"/>. 
        /// </summary>
        /// <param name="Parameters"></param>
        /// <param name="Problem">   </param>
        public abstract void Minimize(FWParams Parameters, TProblem Problem);

        /// <summary>
        /// <see cref="IBaseOptimizer{TParams, TProblem}.Minimize(TParams, TProblem)"/>. 
        /// </summary>
        /// <param name="Parameters"> </param>
        /// <param name="Problem">    </param>
        /// <param name="CancelToken"></param>
        public abstract void Minimize(FWParams Parameters, TProblem Problem, CancellationToken CancelToken);

        /// <summary>
        /// <see cref="IBaseOptimizer{TParams, TProblem}.Minimize(TParams, TProblem)"/>. 
        /// </summary>
        /// <param name="Parameters"></param>
        /// <param name="Problem">   </param>
        /// <param name="Reporter">  </param>
        public abstract void Minimize(FWParams Parameters, TProblem Problem, IProgress<Progress> Reporter);

        /// <summary>
        /// <see cref="IBaseOptimizer{TParams, TProblem}.Minimize(TParams, TProblem)"/>. 
        /// </summary>
        /// <param name="Parameters"> </param>
        /// <param name="Problem">    </param>
        /// <param name="Reporter">   </param>
        /// <param name="CancelToken"></param>
        public abstract void Minimize(FWParams Parameters, TProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken);
    }
}