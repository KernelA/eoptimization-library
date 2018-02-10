// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using EOpt.Help;
    using EOpt.Math.Random;
    using EOpt.Exceptions;

    public abstract class BaseGEM<TProblem> : IBaseOptimizer<GEMParams, TProblem>
    {
        protected PointND _dosd, _dgrs, _drnd;

        /// <summary>
        /// Grenades. 
        /// </summary>
        protected List<Agent> _grenades;

        protected INormalGen _normalRand;
        protected GEMParams _parameters;
        protected double _radiusGrenade, _radiusExplosion, _radiusInitial, _mosd;

        /// <summary>
        /// Shrapnels. 
        /// </summary>
        protected LinkedList<Agent>[] _shrapnels;

        // The temporary array has a 'Length' is equal 'dimension'. Used in the calculation the value
        // of the target function.
        protected double[] _tempArray;

        protected IContUniformGen _uniformRand;
        protected Agent _xcur, _xosd, _xrnd;

        protected virtual void Clear()
        {
            _grenades.Clear();

            for (int i = 0; i < _parameters.NGrenade; i++)
            {
                _shrapnels[i].Clear();
            }
        }

        protected abstract double EvalFuncForTransformedCoord(IReadOnlyList<double> Point);

        /// <summary>
        /// Search a best position to grenade. 
        /// </summary>
        /// <param name="WhichGrenade"></param>
        protected void FindBestPosition(int WhichGrenade)
        {
            // Find shrapnel with a minimum value of the target function.
            _xrnd = null;

            if (_shrapnels[WhichGrenade].Count != 0)
            {
                _xrnd = _shrapnels[WhichGrenade].First.Value;

                foreach (Agent shrapnel in _shrapnels[WhichGrenade])
                {
                    if (IsLessByObjs(shrapnel, _xrnd))
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
                bestPosition = IsLessByObjs(_xcur, _xrnd) ? _xcur : _xrnd;
            }

            if (_xosd != null && bestPosition != null)
            {
                bestPosition = IsLessByObjs(_xosd, bestPosition) ? _xosd : bestPosition;
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
        /// Searching OSD and Xosd position. 
        /// </summary>
        /// <param name="WhichGrenade"></param>
        /// <param name="NumIter">     </param>
        protected void FindOSD(int WhichGrenade, int NumIter, int Dimension, int DimObjs)
        {
            LinkedList<Agent> ortogonalArray = new LinkedList<Agent>();

            // Generate 2 * n shrapnels along coordinate axis. 1 in positive direction. 1 in negative direction.
            bool isAddToList = false;

            Agent tempAgent = new Agent(Dimension, DimObjs);

            bool isPosDirection = true;

            for (int i = 0; i < 2 * Dimension; i++)
            {
                // Reuse an allocated memory, if 'tempAgent' was not added to the array, otherwise
                // allocate a new memory.
                if (isAddToList)
                {
                    tempAgent = new Agent(Dimension, DimObjs);
                }

                isAddToList = true;

                tempAgent.Point.SetAt(_grenades[WhichGrenade].Point);

                // The positive direction along the coordinate axis.
                if (isPosDirection)
                {
                    if (CompareDouble.AlmostEqual(_grenades[WhichGrenade].Point[i / 2], 1, Exponent: 1))
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
                    if (CompareDouble.AlmostEqual(_grenades[WhichGrenade].Point[i / 2], -1, Exponent: 1))
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
                    tempAgent.Eval(EvalFuncForTransformedCoord);
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
                    if (IsLessByObjs(item, _xosd))
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

                if(CompareDouble.AlmostEqual(norm, 0.0, 2))
                {
                    _dosd = null;
                }
                else
                {
                    _dosd.MultiplyByInplace(1 / norm);
                }              
            }
        }

        protected abstract void FirstStep();

        protected void GenerateShrapnelesForGrenade(int WhichGrenade, int NumIter, int Dimension, int DimObjs)
        {
            double p = Math.Max(1.0 / Dimension,
                 Math.Log10(_radiusGrenade / _radiusExplosion) / Math.Log10(_parameters.Pts));

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
                    _shrapnels[WhichGrenade].AddLast(new Agent(tempPoint, new PointND(0.0, DimObjs)));
                }
            }
        }

        protected virtual void Init(GEMParams Parameters, TProblem Problem, int Dimension, int DimObjs)
        {
            if (!Parameters.IsParamsInit)
            {
                throw new ArgumentException("The parameters were created by the default constructor and have invalid values.\n" +
                    "You need to create parameters with a custom constructor.", nameof(Parameters));
            }

            _parameters = Parameters;
            ;

            _radiusGrenade = _parameters.InitRadiusGrenade;
            _radiusInitial = _parameters.InitRadiusGrenade;

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
                _tempArray = new double[Dimension];
            }
            else if (_tempArray.Length != Dimension)
            {
                _tempArray = new double[Dimension];
            }

            if (_drnd == null)
            {
                _drnd = new PointND(0.0, Dimension);
            }
            else if (_drnd.Count != Dimension)
            {
                _drnd = new PointND(0.0, Dimension);
            }

            if (_dgrs == null)
            {
                _dgrs = new PointND(0, Dimension);
            }
            else if (_dgrs.Count != Dimension)
            {
                _dgrs = new PointND(0, Dimension);
            }
        }

        /// <summary>
        /// Create grenades. 
        /// </summary>
        /// <param name="LowerBounds"></param>
        /// <param name="UpperBounds"></param>
        /// <param name="DimObjs">    </param>
        protected virtual void InitAgents(IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds, int DimObjs)
        {
            int dimension = LowerBounds.Count;

            for (int i = 0; i < _parameters.NGrenade; i++)
            {
                PointND point = new PointND(0.0, dimension);

                for (int j = 0; j < dimension; j++)
                {
                    point[j] = _uniformRand.URandVal(-1, 1);
                }

                _grenades.Add(new Agent(point, new PointND(0.0, DimObjs)));
            }
        }

        protected void InitShrapnels()
        {
            _shrapnels = new LinkedList<Agent>[_parameters.NGrenade];

            for (int i = 0; i < _parameters.NGrenade; i++)
            {
                _shrapnels[i] = new LinkedList<Agent>();
            }
        }

        protected abstract bool IsLessByObjs(Agent First, Agent Second);

        protected abstract void NextStep(int Iter);

        /// <summary>
        /// Coordinates transformation: [-1; 1] -&gt; [LowerBounds[i]; UpperBounds[i]]. 
        /// </summary>
        /// <param name="X"> Input coordinates. </param>
        protected void TransformCoord(double[] X, IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds)
        {
            for (int i = 0; i < LowerBounds.Count; i++)
            {
                X[i] = 0.5 * (LowerBounds[i] + UpperBounds[i] + (UpperBounds[i] - LowerBounds[i]) * X[i]);
            }
        }

        /// <summary>
        /// Update parameters. 
        /// </summary>
        /// <param name="NumIter"></param>
        protected void UpdateParams(int NumIter, int Dimension)
        {
            _radiusGrenade = _radiusInitial / Math.Pow(_parameters.RadiusReduct, (double)NumIter / _parameters.Imax);

            double m = _parameters.Mmax - (double)NumIter / _parameters.Imax * (_parameters.Mmax - _parameters.Mmin);

            _radiusExplosion = Math.Pow(2 * Math.Sqrt(Dimension), m) * Math.Pow(_radiusGrenade, 1 - m);

            _mosd = Math.Sin(Math.PI / 2 * Math.Pow(Math.Abs(NumIter - 0.1 * _parameters.Imax) / (0.9 * _parameters.Imax), _parameters.Psin));
        }

        /// <summary>
        /// Create object which uses custom implementation for random generators. 
        /// </summary>
        public BaseGEM() : this(new ContUniformDist(), new NormalDist())
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
        public BaseGEM(IContUniformGen UniformGen, INormalGen NormalGen)
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
        }

        public abstract void Minimize(GEMParams Parameters, TProblem Problem);

        public abstract void Minimize(GEMParams Parameters, TProblem Problem, CancellationToken CancelToken);

        public abstract void Minimize(GEMParams Parameters, TProblem Problem, IProgress<Progress> Reporter);

        public abstract void Minimize(GEMParams Parameters, TProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken);
    }
}