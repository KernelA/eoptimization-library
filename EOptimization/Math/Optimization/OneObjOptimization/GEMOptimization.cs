// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.OOOpt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Help;

    using Math.Random;

    /// <summary>
    /// Optimization method GEM.
    /// </summary>
    public class GEMOptimizer : IBaseOptimizer<GEMParams, IOOOptProblem>, IOOOptimizer<GEMParams>
    {
        private Agent _solution;

        private PointND _dosd, _dgrs, _drnd;

        /// <summary>
        /// Grenades.
        /// </summary>
        private List<Agent> _grenades;

        private INormalGen _normalRand;

        private GEMParams _parameters;

        private double _radiusGrenade, _radiusExplosion, _radiusInitial, _mosd;

        private IReadOnlyList<double> _lowerBounds;

        private IReadOnlyList<double> _upperBounds;

        private Func<IReadOnlyList<double>, double> _targetFuncWithTransformedCoords;

        private Func<IReadOnlyList<double>, double> _originalTargetFunction;

        /// <summary>
        /// Shrapnels.
        /// </summary>
        private LinkedList<Agent>[] _shrapnels;

        // The temporary array has a 'Length' is equal 'dimension'. Used in the calculation the value
        // of the target function.
        private double[] _tempArray;

        private IContUniformGen _uniformRand;

        private Agent _xcur, _xosd, _xrnd;

        /// <summary>
        /// Sort by ascending.
        /// </summary>
        private void ArrangeGrenades()
        {
            _grenades.Sort((x, y) => x.Objs[0].CompareTo(y.Objs[0]));
        }

        private void FindBestPosition(int WhichGrenade)
        {
            // Find shrapnel with a minimum value of the target function.
            _xrnd = null;

            if (_shrapnels[WhichGrenade].Count != 0)
            {
                _xrnd = _shrapnels[WhichGrenade].First.Value;

                foreach (Agent shrapnel in _shrapnels[WhichGrenade])
                {
                    if (shrapnel.Objs[0] < _xrnd.Objs[0])
                    {
                        _xrnd = shrapnel;
                    }
                }
            }

            Agent bestPosition = null;

            // Find best position with a minimum value of the target function among: xcur, xrnd, xosd.
            if (_xcur == null && _xrnd != null)
            {
                bestPosition = _xrnd;
            }
            else if (_xcur != null && _xrnd == null)
            {
                bestPosition = _xcur;
            }
            else if (_xcur != null && _xrnd != null)
            {
                bestPosition = _xrnd.Objs[0] < _xcur.Objs[0] ? _xrnd : _xcur;
            }

            if (_xosd != null && bestPosition != null)
            {
                bestPosition = bestPosition.Objs[0] < _xosd.Objs[0] ? bestPosition : _xosd;
            }
            else if (_xosd != null && bestPosition == null)
            {
                bestPosition = _xosd;
            }

            // If exist a best position then move grenade.
            if (bestPosition != null)
            {
                _grenades[WhichGrenade] = bestPosition;
            }

            _xosd = null;
            _xrnd = null;
            _xcur = null;
            _dosd = null;

            _shrapnels[WhichGrenade].Clear();
        }

        /// <summary>
        /// Calculate target function for the grenades.
        /// </summary>
        private void EvalFunctionForGrenades()
        {
            for (int i = 0; i < _parameters.NGrenade; i++)
            {
                _grenades[i].Eval(_targetFuncWithTransformedCoords);
            }
        }

        /// <summary>
        /// Coordinates transformation: [-1; 1] -&gt; [LowerBounds[i]; UpperBounds[i]].
        /// </summary>
        /// <param name="X"> Input coordinates. </param>
        /// <param name="LowerBounds"></param>
        /// <param name="UpperBounds"></param>
        private static void TransformCoord(double[] X, IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds)
        {
            for (int i = 0; i < LowerBounds.Count; i++)
            {
                X[i] = 0.5 * (LowerBounds[i] + UpperBounds[i] + (UpperBounds[i] - LowerBounds[i]) * X[i]);
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
                FindOSD(WhichGrenade, Problem.LowerBounds.Count);
            }

            GenerateShrapnelesForGrenade(WhichGrenade, Problem.LowerBounds.Count);
        }

        private void GenerateShrapnelesForGrenade(int WhichGrenade, int Dimension)
        {
            double p = Math.Max(1.0 / Dimension,
                 Math.Log10(_radiusGrenade / _radiusExplosion) / Math.Log10(_parameters.Pts));

            if (CheckDouble.GetTypeValue(p) != DoubleTypeValue.Valid)
            {
                p = 0.5;
            }

            double randomValue1, randomValue2;

            bool isShrapnelAdd = true;

            PointND tempPoint = null;

            // Generating shrapnels.
            for (int i = 0; i < _parameters.NShrapnel; i++)
            {
                // Reuse an allocated memory, if 'tempPoint' was not added to the array, otherwise
                // allocate a new memory.
                if (isShrapnelAdd)
                {
                    tempPoint = new PointND(0.0, Dimension);
                }

                randomValue1 = _uniformRand.URandVal(0, 1);

                // Random search direction.
                for (int w = 0; w < _drnd.Count; w++)
                {
                    _drnd[w] = _normalRand.NRandVal(0, 1);
                }

                _drnd.MultiplyByInplace(1 / _drnd.Norm());

                // If exist OSD.
                if (_dosd != null)
                {
                    randomValue2 = _uniformRand.URandVal(0, 1);

                    for (int coordIndex = 0; coordIndex < _dgrs.Count; coordIndex++)
                    {
                        _dgrs[coordIndex] = _mosd * randomValue1 * _dosd[coordIndex] + (1 - _mosd) * randomValue2 * _drnd[coordIndex];
                    }

                    _dgrs.MultiplyByInplace(1 / _dgrs.Norm());

                    randomValue1 = Math.Pow(randomValue1, p) * _radiusExplosion;

                    for (int coordIndex = 0; coordIndex < _dgrs.Count; coordIndex++)
                    {
                        tempPoint[coordIndex] = _grenades[WhichGrenade].Point[coordIndex] + randomValue1 * _dgrs[coordIndex];
                    }
                }
                else
                {
                    randomValue1 = Math.Pow(randomValue1, p) * _radiusExplosion;

                    for (int coordIndex = 0; coordIndex < _dgrs.Count; coordIndex++)
                    {
                        tempPoint[coordIndex] = _grenades[WhichGrenade].Point[coordIndex] + randomValue1 * _drnd[coordIndex];
                    }
                }

                double randNum = 0.0, largeComp = 0.0;

                // Out of range [-1; 1]^n.
                for (int j = 0; j < Dimension; j++)
                {
                    if (tempPoint[j] < -1 || tempPoint[j] > 1)
                    {
                        randNum = _uniformRand.URandVal(0, 1);

                        largeComp = tempPoint.Max(num => Math.Abs(num));

                        double normPos = 0.0;

                        for (int coordIndex = 0; coordIndex < tempPoint.Count; coordIndex++)
                        {
                            normPos = tempPoint[coordIndex] / largeComp;

                            tempPoint[coordIndex] = randNum * (normPos - _grenades[WhichGrenade].Point[coordIndex]) + _grenades[WhichGrenade].Point[coordIndex];
                        }

                        break;
                    }
                }

                double dist = 0.0;

                // Calculate distance to grenades.
                for (int idxGren = 0; idxGren < _parameters.NGrenade; idxGren++)
                {
                    if (idxGren != WhichGrenade)
                    {
                        dist = PointND.Distance(tempPoint, _grenades[idxGren].Point);

                        // Shrapnel does not accept (it is too near to other grenades).
                        if (dist <= _radiusGrenade)
                        {
                            isShrapnelAdd = false;
                            break;
                        }
                    }
                }

                if (isShrapnelAdd)
                {
                    _shrapnels[WhichGrenade].AddLast(new Agent(tempPoint, new PointND(0.0, 1)));
                }
            }
        }

        private double TargetFunctionWithTransformedCoords(IReadOnlyList<double> Point)
        {
            for (int i = 0; i < Point.Count; i++)
            {
                _tempArray[i] = Point[i];
            }

            TransformCoord(_tempArray, _lowerBounds, _upperBounds);

            return _originalTargetFunction(_tempArray);
        }

        /// <summary>
        /// Searching OSD and Xosd position.
        /// </summary>
        /// <param name="WhichGrenade"></param>
        /// <param name="Dimension"></param>
        private void FindOSD(int WhichGrenade, int Dimension)
        {
            LinkedList<Agent> ortogonalArray = new LinkedList<Agent>();

            // Generate 2 * n shrapnels along coordinate axis. 1 in positive direction. 1 in negative direction.
            bool isAddToList = false;

            Agent tempAgent = new Agent(Dimension, 1);

            bool isPosDirection = true;

            for (int i = 0; i < 2 * Dimension; i++)
            {
                // Reuse an allocated memory, if 'tempAgent' was not added to the array, otherwise
                // allocate a new memory.
                if (isAddToList)
                {
                    tempAgent = new Agent(Dimension, 1);
                }

                isAddToList = true;

                tempAgent.Point.SetAt(_grenades[WhichGrenade].Point);

                // The positive direction along the coordinate axis.
                if (isPosDirection)
                {
                    if (CmpDouble.AlmostEqual(_grenades[WhichGrenade].Point[i / 2], 1, Exponent: 1))
                    {
                        tempAgent.Point[i / 2] = 1;
                    }
                    else
                    {
                        tempAgent.Point[i / 2] = _uniformRand.URandVal(_grenades[WhichGrenade].Point[i / 2], 1);
                    }
                }
                // The negative direction along the coordinate axis.
                else
                {
                    if (CmpDouble.AlmostEqual(_grenades[WhichGrenade].Point[i / 2], -1, Exponent: 1))
                    {
                        tempAgent.Point[i / 2] = -1;
                    }
                    else
                    {
                        tempAgent.Point[i / 2] = _uniformRand.URandVal(-1, _grenades[WhichGrenade].Point[i / 2]);
                    }
                }

                // Change direction.
                isPosDirection = !isPosDirection;

                // If shrapnel and grenade too near, then shrapnel deleted.
                for (int j = 0; j < _parameters.NGrenade; j++)
                {
                    if (j != WhichGrenade)
                    {
                        if (PointND.Distance(tempAgent.Point, _grenades[j].Point) <= _radiusGrenade)
                        {
                            isAddToList = false;
                            break;
                        }
                    }
                }

                if (isAddToList)
                {
                    tempAgent.Eval(_targetFuncWithTransformedCoords);
                    ortogonalArray.AddLast(tempAgent);
                }
            }

            // Determine position Xosd.
            _xosd = null;

            if (ortogonalArray.Count != 0)
            {
                _xosd = ortogonalArray.First.Value;

                foreach (Agent item in ortogonalArray)
                {
                    if (item.Objs[0] < _xosd.Objs[0])
                    {
                        _xosd = item;
                    }
                }
            }

            // Determine position Xcur.
            _xcur = _grenades[WhichGrenade];

            // If grenade does not be in a neighborhood of the other grenades, then xcur = null.
            for (int j = 0; j < _parameters.NGrenade; j++)
            {
                if (j != WhichGrenade)
                {
                    if (PointND.Distance(_grenades[j].Point, _grenades[WhichGrenade].Point) <= _radiusGrenade)
                    {
                        _xcur = null;
                        break;
                    }
                }
            }

            // Vector dosd.
            _dosd = _xosd == null ? null : _xosd.Point - _grenades[WhichGrenade].Point;

            // Normalization vector.
            if (_dosd != null)
            {
                double norm = _dosd.Norm();

                if (CmpDouble.AlmostEqual(norm, 0.0, 2))
                {
                    _dosd = null;
                }
                else
                {
                    _dosd.MultiplyByInplace(1 / norm);
                }
            }
        }

        /// <summary>
        /// Create grenades.
        /// </summary>
        /// <param name="LowerBounds"></param>
        /// <param name="UpperBounds"></param>
        private void InitAgents(IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds)
        {
            int dimension = LowerBounds.Count;

            for (int i = 0; i < _parameters.NGrenade; i++)
            {
                PointND point = new PointND(0.0, dimension);

                for (int j = 0; j < dimension; j++)
                {
                    point[j] = _uniformRand.URandVal(-1, 1);
                }

                _grenades.Add(new Agent(point, new PointND(0.0, 1)));
            }
        }

        private void FirstStep(IOOOptProblem Problem)
        {
            int dimension = Problem.LowerBounds.Count;

            _radiusExplosion = 2 * Math.Sqrt(dimension);

            InitAgents(Problem.LowerBounds, Problem.UpperBounds);

            EvalFunctionForGrenades();
        }

        private void Init(GEMParams Parameters, IOOOptProblem Problem)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            if (!Parameters.IsParamsInit)
            {
                throw new ArgumentException("The parameters were created by the default constructor and have invalid values.\nYou need to create parameters with a custom constructor.", nameof(Parameters));
            }

            _originalTargetFunction = Problem.TargetFunction;
            _lowerBounds = Problem.LowerBounds;
            _upperBounds = Problem.UpperBounds;
            _parameters = Parameters;
            _radiusGrenade = _parameters.InitRadiusGrenade;
            _radiusInitial = _parameters.InitRadiusGrenade;

            int dim = Problem.LowerBounds.Count;

            _mosd = 0;

            if (_grenades == null)
            {
                _grenades = new List<Agent>(_parameters.NGrenade);
            }
            else
            {
                _grenades.Capacity = _parameters.NGrenade;
            }

            if (_shrapnels == null)
            {
                InitShrapnels();
            }
            else if (_shrapnels.Length != _parameters.NGrenade)
            {
                InitShrapnels();
            }

            if (_tempArray == null)
            {
                _tempArray = new double[dim];
            }
            else if (_tempArray.Length != dim)
            {
                _tempArray = new double[dim];
            }

            if (_drnd == null)
            {
                _drnd = new PointND(0.0, dim);
            }
            else if (_drnd.Count != dim)
            {
                _drnd = new PointND(0.0, dim);
            }

            if (_dgrs == null)
            {
                _dgrs = new PointND(0, dim);
            }
            else if (_dgrs.Count != dim)
            {
                _dgrs = new PointND(0, dim);
            }

            if (_solution == null)
            {
                _solution = new Agent(dim, 1);
            }
            else if (_solution.Point.Count != dim)
            {
                _solution = new Agent(dim, 1);
            }
        }

        private void NextStep(IOOOptProblem Problem, int Iter)
        {
            ArrangeGrenades();

            for (int j = 0; j < this._parameters.NGrenade; j++)
            {
                GenerateShrapneles(Problem, j, Iter);

                EvalFunctionForShrapnels(j);

                FindBestPosition(j);
            }

            UpdateParams(Iter, Problem.LowerBounds.Count);

            FindSolution(Problem);
        }

        private void InitShrapnels()
        {
            _shrapnels = new LinkedList<Agent>[_parameters.NGrenade];

            for (int i = 0; i < _parameters.NGrenade; i++)
            {
                _shrapnels[i] = new LinkedList<Agent>();
            }
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
        public GEMOptimizer(IContUniformGen UniformGen, INormalGen NormalGen)
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

            _targetFuncWithTransformedCoords = TargetFunctionWithTransformedCoords;
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
        public void Minimize(GEMParams Parameters, IOOOptProblem Problem)
        {
            Init(Parameters, Problem);

            FirstStep(Problem);

            for (int i = 1; i <= _parameters.Imax; i++)
            {
                NextStep(Problem, i);
            }

            Clear();
        }

        private void Clear()
        {
            _grenades.Clear();

            for (int i = 0; i < _parameters.NGrenade; i++)
            {
                _shrapnels[i].Clear();
            }
        }

        /// <summary>
        /// Update parameters.
        /// </summary>
        /// <param name="NumIter"></param>
        /// <param name="Dimension"></param>
        private void UpdateParams(int NumIter, int Dimension)
        {
            _radiusGrenade = _radiusInitial / Math.Pow(_parameters.RadiusReduct, (double)NumIter / _parameters.Imax);

            double m = _parameters.Mmax - (double)NumIter / _parameters.Imax * (_parameters.Mmax - _parameters.Mmin);

            _radiusExplosion = Math.Pow(2 * Math.Sqrt(Dimension), m) * Math.Pow(_radiusGrenade, 1 - m);

            _mosd = Math.Sin(Math.PI / 2 * Math.Pow(Math.Abs(NumIter - 0.1 * _parameters.Imax) / (0.9 * _parameters.Imax), _parameters.Psin));
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
        public void Minimize(GEMParams Parameters, IOOOptProblem Problem, CancellationToken CancelToken)
        {
            Init(Parameters, Problem);

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
        public void Minimize(GEMParams Parameters, IOOOptProblem Problem, IProgress<Progress> Reporter)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem);

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
        public void Minimize(GEMParams Parameters, IOOOptProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            Init(Parameters, Problem);

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