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
    using Math.LA;
    using Math.Optimization;
    using Math.Random;

    using Nds;

    /// <summary>
    /// Optimization method Fireworks. 
    /// </summary>
    //public class MOFWOptimizer : IMOOptimizer<FWParams>
    //{
    //    /// <summary>
    //    /// Charges. 
    //    /// </summary>
    //    private List<MOAgent> _chargePoints;

    //    // Used in the method 'GenerateIndexesOfAxes'.
    //    private int[] _coordNumbers;

    //    /// <summary>
    //    /// Debris for charges. 
    //    /// </summary>
    //    private LinkedList<MOAgent>[] _debris;

    //    private double _fmax, _fmin;
    //    private int _indexMinChargePoints, _indexMinDebris1, _indexMinDebris2;

    //    private SymmetricMatrix _matrixOfDistances;
    //    private INormalGen _normalRand;
    //    private FWParams _parameters;
    //    private IContUniformGen _uniformRand;

    //    private WeightOfAgent[] _weights;

    //    private void Clear()
    //    {
    //        //_chargePoints.Clear();

    //        for (int i = 0; i < _debris.Length; i++)
    //        {
    //            _debris[i].Clear();
    //        }
    //    }

    //    private void EvalFunction(IEnumerable<Func<IReadOnlyList<double>, double>> Function)
    //    {
    //        for (int i = 0; i < _parameters.NP; i++)
    //        {
    //            _chargePoints[i].Eval(Function);
    //        }
    //    }

    //    /// <summary>
    //    /// Find amount debris for each point of charge. 
    //    /// </summary>
    //    private void FindAmountDebris(int[] ChargeFronts)
    //    {
    //        double s = 0;

    //        int countDebris = 0;

    //        int maxFront = ChargeFronts.Max();

    //        if (maxFront == 0)
    //            maxFront++;

    //        int dimObjs = _chargePoints[0].Objs.Count;

    //        for (int i = 0; i < _parameters.NP; i++)
    //        {
    //            s = (double)ChargeFronts[i] / maxFront;
    //            s = _parameters.M * (1 - 0.9 * s * s);

    //            if (s < _parameters.Alpha * _parameters.M)
    //            {
    //                s = Math.Truncate(_parameters.Alpha * _parameters.M);
    //            }
    //            else if (s > _parameters.Beta * _parameters.M)
    //            {
    //                s = Math.Truncate(_parameters.Beta * _parameters.M);
    //            }
    //            else
    //            {
    //                s = Math.Truncate(s);
    //            }

    //            countDebris = (int)s;

    //            if (countDebris == 0)
    //            {
    //                _debris[i].Clear();
    //            }
    //            else if (_debris[i].Count > countDebris)
    //            {
    //                int del = _debris[i].Count - countDebris;

    //                while (del > 0)
    //                {
    //                    _debris[i].RemoveLast();
    //                    del--;
    //                }
    //            }
    //            else if (_debris[i].Count < countDebris)
    //            {
    //                int total = countDebris - _debris[i].Count;

    //                int dimension = _chargePoints[0].Point.Count;

    //                while (total > 0)
    //                {
    //                    _debris[i].AddLast(new MOAgent(dimension, dimObjs));
    //                    total--;
    //                }
    //            }
    //        }
    //    }

    //    //    for (int i = 0; i < _parameters.NP; i++)
    //    //    {
    //    //        if (_chargePoints[i].Obj < _fmin)
    //    //        {
    //    //            _fmin = _chargePoints[i].Obj;
    //    //        }
    //    //        else if (_chargePoints[i].Obj > _fmax)
    //    //        {
    //    //            _fmax = _chargePoints[i].Obj;
    //    //        }
    //    //    }
    //    //}
    //    /// <summary>
    //    /// First method for determination of position of the debris. 
    //    /// </summary>
    //    /// <param name="Splinter">        </param>
    //    /// <param name="CountOfDimension"></param>
    //    /// <param name="Amplitude">       </param>
    //    /// <param name="LowerBounds">     </param>
    //    /// <param name="UpperBounds">     </param>
    //    private void FirstMethodDeterminationOfPosition(MOAgent Splinter, int CountOfDimension, double Amplitude, IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds)
    //    {
    //        // The indices are choosing randomly.
    //        GenerateIndexesOfAxes();

    //        double h = 0;

    //        // Calculate position of debris.
    //        for (int i = 0; i < CountOfDimension; i++)
    //        {
    //            int axisIndex = _coordNumbers[i];

    //            h = Amplitude * _uniformRand.URandVal(-1, 1);

    //            Splinter.Point[axisIndex] += h;

    //            Splinter.Point[axisIndex] = ClampDouble.Clamp(Splinter.Point[axisIndex], LowerBounds[axisIndex], UpperBounds[axisIndex]);

    //            // If point leaves region then it returns to random position.
    //            //if (Splinter.Point[axisIndex] < LowerBound[axisIndex] || Splinter.Point[axisIndex] > UpperBound[axisIndex])
    //            //{
    //            //    Splinter.Point[axisIndex] = _uniformRand.URandVal(LowerBound[axisIndex], UpperBound[axisIndex]);
    //            //}
    //        }
    //    }

    //    private void FirstStep(MOOptimizationProblem Problem)
    //    {
    //        if (Problem == null)
    //        {
    //            throw new ArgumentNullException(nameof(Problem));
    //        }

    //        InitAgents(Problem.LowerBounds, Problem.UpperBounds, Problem.TargetFunction.Count);

    //        EvalFunction(Problem.TargetFunction);

    //        int[] chargeFronts = Ndsort.NonDominSort(_chargePoints, item => item.Objs);

    //        FindAmountDebris(chargeFronts);

    //        GenerateDebris(chargeFronts, Problem.LowerBounds, Problem.UpperBounds, Problem.TargetFunction);

    //        //FindBestSolution()
    //    }

    //    /// <summary>
    //    /// Determine debris position. 
    //    /// </summary>
    //    /// <param name="ChargeFronts"></param>
    //    /// <param name="LowerBounds"> </param>
    //    /// <param name="UpperBounds"> </param>
    //    /// <param name="Function">    </param>
    //    private void GenerateDebris(int[] ChargeFronts, IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds, IEnumerable<Func<IReadOnlyList<double>, double>> Function)
    //    {
    //        double amplitude = 0;

    //        int maxFront = ChargeFronts.Max();

    //        if (maxFront == 0)
    //            maxFront++;

    //        for (int i = 0; i < _parameters.NP; i++)
    //        {
    //            // Amplitude of explosion.
    //            amplitude = _parameters.Amax * 2 / Math.PI * Math.Atan(ChargeFronts[i] / maxFront + 0.2);

    //            // For each debris.
    //            foreach (MOAgent splinter in _debris[i])
    //            {
    //                // The position of debris sets to the position of charge.
    //                splinter.Point.SetAt(_chargePoints[i].Point);

    //                double ksi = _uniformRand.URandVal(0, 1);

    //                int CountOfDimension = (int)Math.Ceiling(LowerBounds.Count * ksi);

    //                if (ksi < 0.5)
    //                {
    //                    FirstMethodDeterminationOfPosition(splinter, CountOfDimension, amplitude, LowerBounds, UpperBounds);
    //                }
    //                else
    //                {
    //                    SecondMethodDeterminationOfPosition(splinter, CountOfDimension, LowerBounds, UpperBounds);
    //                }

    //                splinter.Eval(Function);
    //            }
    //        }
    //    }

    //    //private void FindFMaxMin()
    //    //{
    //    //    // Last coordinate stores value of the target function.
    //    //    _fmin = _chargePoints[0].Obj;
    //    //    _fmax = _chargePoints[0].Obj;
    //    /// <summary>
    //    /// Generate randomly indices of axes. 
    //    /// </summary>
    //    /// <returns></returns>
    //    private void GenerateIndexesOfAxes()
    //    {
    //        // Set coordinate numbers.
    //        for (int i = 0; i < _coordNumbers.Length; i++)
    //        {
    //            _coordNumbers[i] = i;
    //        }

    //        Сombinatorics.RandomPermutation(_coordNumbers, SyncRandom.Get());
    //    }

    //    /// <summary>
    //    /// Generate current population. 
    //    /// </summary>
    //    private void GenerateNextAgents(int[] ChargeAndDebrisFronts, IEnumerable<MOAgent> FirstFront)
    //    {
    //        for (int row = 0; row < _matrixOfDistances.RowCount; row++)
    //        {
    //            for (int column = 0; column < _matrixOfDistances.ColumnCount; column++)
    //            {
    //                _matrixOfDistances[row, column] = 0;
    //            }
    //        }

    //        for (int i = 0; i < _weights.Length; i++)
    //        {
    //            _weights[i].Distance = 0;
    //            _weights[i].IsTake = false;
    //        }

    //        int firstFrontCount = ChargeAndDebrisFronts.Count(front => front == 0);

    //        // The total count minus non-dominated solutions.
    //        int actualSizeMatrix = _chargePoints.Count - firstFrontCount;

    //        for (int k = 0; k < _debris.Length; k++)
    //        {
    //            actualSizeMatrix += _debris[k].Count;
    //        }

    //        {
    //            int index = 0;
    //            int total = 0;

    //            for (int i = 0; i < _chargePoints.Count; i++)
    //            {
    //                // Skip non-dominated solutions.
    //                if (ChargeAndDebrisFronts[total] != 0)
    //                {
    //                    _weights[index].Agent.SetAt(_chargePoints[i]);
    //                    index++;
    //                }
    //                total++;
    //            }

    //            for (int i = 0; i < _debris.Length; i++)
    //            {
    //                foreach (MOAgent splinter in _debris[i])
    //                {
    //                    if (ChargeAndDebrisFronts[total] != 0)
    //                    {
    //                        _weights[index].Agent.SetAt(splinter);
    //                        index++;
    //                    }

    //                    total++;
    //                }
    //            }
    //        }

    //        double denumeratorForProbability = 0;

    //        // Calculate distance between all points.
    //        for (int i = 0; i < actualSizeMatrix; i++)
    //        {
    //            for (int j = i + 1; j < actualSizeMatrix; j++)
    //            {
    //                _matrixOfDistances[i, j] = PointND.Distance(_weights[i].Agent.Point, _weights[j].Agent.Point);
    //            }
    //        }

    //        for (int ii = 0; ii < actualSizeMatrix; ii++)
    //        {
    //            double dist = 0;

    //            for (int j = 0; j < actualSizeMatrix; j++)
    //            {
    //                dist += _matrixOfDistances[ii, j];
    //            }

    //            _weights[ii].Distance = dist;

    //            denumeratorForProbability += dist;
    //        }

    //        // Probability of explosion.
    //        for (int jj = 0; jj < actualSizeMatrix; jj++)
    //        {
    //            _weights[jj].Distance /= denumeratorForProbability;
    //        }

    //        int startIndex = 0;

    //        foreach (var item in FirstFront)
    //        {
    //            if (startIndex < _parameters.NP)
    //            {
    //                _chargePoints[startIndex++].SetAt(item);
    //            }
    //        }

    //        int totalToTake = _parameters.NP - firstFrontCount;

    //        if (totalToTake > 0)
    //        {
    //            TakeAgents(_weights, actualSizeMatrix, firstFrontCount);
    //        }

    //        for (int i = 0; i < actualSizeMatrix && totalToTake > 0; i++)
    //        {
    //            if (_weights[i].IsTake)
    //            {
    //                _chargePoints[startIndex].SetAt(_weights[i].Agent);
    //                startIndex++;
    //                totalToTake--;
    //            }
    //        }
    //    }

    //    private void Init(FWParams Parameters, int Dimension, int DimObjs)
    //    {
    //        if (!Parameters.IsParamsInit)
    //        {
    //            throw new ArgumentException("The parameters were created by the default constructor and have invalid values.\n" +
    //                "You need to create parameters with a custom constructor.", nameof(Parameters));
    //        }

    //        _parameters = Parameters;

    //        if (_coordNumbers == null)
    //            _coordNumbers = new int[Dimension];
    //        else if (_coordNumbers.Length != Dimension)
    //            _coordNumbers = new int[Dimension];

    //        if (_chargePoints == null)
    //        {
    //            _chargePoints = new List<MOAgent>(_parameters.NP);
    //        }
    //        else
    //        {
    //            _chargePoints.Capacity = _parameters.NP;
    //        }

    //        if (_debris == null)
    //        {
    //            InitDebris();
    //        }
    //        else if (_debris.Length != this.Parameters.NP)
    //        {
    //            InitDebris();
    //        }

    //        int newSizeMatrix = checked(_parameters.NP - 1 + _parameters.NP * (int)Math.Truncate(_parameters.Beta * _parameters.M));

    //        _weights = new WeightOfAgent<MOAgent>[newSizeMatrix];

    //        for (int i = 0; i < _weights.Length; i++)
    //        {
    //            // The structure is storing an agent and it weight.
    //            _weights[i] = new WeightOfAgent<MOAgent>(new MOAgent(Dimension, DimObjs), 0.0);
    //        }

    //        if (_matrixOfDistances == null)
    //        {
    //            // The matrix have a maximum size. An actual size is '_matrixSize'. It always changes.
    //            _matrixOfDistances = new SymmetricMatrix(newSizeMatrix);
    //        }
    //        else if (_matrixOfDistances.RowCount < newSizeMatrix)
    //        {
    //            _matrixOfDistances = new SymmetricMatrix(newSizeMatrix);
    //        }
    //    }

    //    private void InitAgents(IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds, int DimObjs)
    //    {
    //        int dimension = LowerBounds.Count;

    //        // Create points of explosion.
    //        for (int i = 0; i < _parameters.NP; i++)
    //        {
    //            PointND point = new PointND(0.0, dimension);

    //            for (int j = 0; j < dimension; j++)
    //            {
    //                point[j] = _uniformRand.URandVal(LowerBounds[j], UpperBounds[j]);
    //            }

    //            _chargePoints.Add(new MOAgent(point, new PointND(0.0, DimObjs)));
    //        }
    //    }

    //    private void InitDebris()
    //    {
    //        _debris = new LinkedList<MOAgent>[_parameters.NP];

    //        for (int i = 0; i < _debris.Length; i++)
    //        {
    //            _debris[i] = new LinkedList<MOAgent>();
    //        }
    //    }

    //    private void NextStep(MOOptimizationProblem Problem)
    //    {
    //        var allAgents = _chargePoints.Concat(_debris[0]);

    //        for (int i = 1; i < _debris.Length; i++)
    //        {
    //            allAgents = allAgents.Concat(_debris[i]);
    //        }

    //        int[] allFronts = Ndsort.NonDominSort(allAgents, item => item.Objs);

    //        LinkedList<MOAgent> firstFront = new LinkedList<MOAgent>();

    //        int index = 0;

    //        foreach (var item in allAgents)
    //        {
    //            if (allFronts[index] == 0)
    //            {
    //                firstFront.AddLast(item);
    //            }
    //            index++;
    //        }

    //        GenerateNextAgents(allFronts, firstFront);

    //        EvalFunction(Problem.TargetFunction);

    //        //FindFMaxMin();

    //        int[] chargeFronts = Ndsort.NonDominSort(_chargePoints, item => item.Objs);

    //        FindAmountDebris(chargeFronts);

    //        GenerateDebris(chargeFronts, Problem.LowerBounds, Problem.UpperBounds, Problem.TargetFunction);

    //        //FindBestSolution();
    //    }

    //    /// <summary>
    //    /// Second method for determination of position of the debris. 
    //    /// </summary>
    //    /// <param name="Splinter">        </param>
    //    /// <param name="CountOfDimension"></param>
    //    /// <param name="LowerBounds">     </param>
    //    /// <param name="UpperBounds">     </param>
    //    private void SecondMethodDeterminationOfPosition(MOAgent Splinter, int CountOfDimension, IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds)
    //    {
    //        GenerateIndexesOfAxes();

    //        double g = 0;

    //        int axisIndex = 0;

    //        // Calculate position of debris.
    //        for (int i = 0; i < CountOfDimension; i++)
    //        {
    //            axisIndex = _coordNumbers[i];

    //            g = _normalRand.NRandVal(1, 1);

    //            Splinter.Point[axisIndex] *= g;

    //            Splinter.Point[axisIndex] = ClampDouble.Clamp(Splinter.Point[axisIndex], LowerBounds[axisIndex], UpperBounds[axisIndex]);

    //            // If point leave region that she return to random position.
    //            //if (Splinter.Point[axisIndex] < LowerBounds[axisIndex] || Splinter.Point[axisIndex] > UpperBounds[axisIndex])
    //            //{
    //            //    Splinter.Point[axisIndex] = _uniformRand.URandVal(LowerBounds[axisIndex], UpperBounds[axisIndex]);
    //            //}
    //        }
    //    }

    //    //    // Select best solution among debris and charges.
    //    //    if (min1 < min2)
    //    //    {
    //    //        _solution = _chargePoints[_indexMinChargePoints];
    //    //        _indexMinDebris1 = -1;
    //    //        _indexMinDebris2 = -1;
    //    //    }
    //    //    else
    //    //    {
    //    //        _solution = _debris[_indexMinDebris1].ElementAt(_indexMinDebris2);
    //    //        _indexMinChargePoints = -1;
    //    //    }
    //    //}
    //    private void TakeAgents(WeightOfAgent<MOAgent>[] Weights, int ActualSize, int FirstFrontCount)
    //    {
    //        int count = 0;

    //        // Take the points with a probability is equal to Distance.
    //        for (int i = 0; i < ActualSize; i++)
    //        {
    //            if (_uniformRand.URandVal(0, 1) < Weights[i].Distance)
    //            {
    //                Weights[i].IsTake = true;
    //                Weights[i].Distance = Double.MaxValue;
    //                count++;
    //            }
    //        }

    //        int remainder = _parameters.NP - count - FirstFrontCount;

    //        // Need to take some points.
    //        if (remainder > 0)
    //        {
    //            // Sort by descending probability of explosion. All points with IsTake = true will in
    //            // the end. Points with higher probability are taking.
    //            Array.Sort(Weights, 0, ActualSize);

    //            for (int i = 0; i < remainder; i++)
    //            {
    //                Weights[i].IsTake = true;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Parameters for Fireworks method. <see cref="FWParams"/>. 
    //    /// </summary>
    //    public FWParams Parameters => _parameters;

    //    public IEnumerable<MOAgent> ParetoFront => _chargePoints;

    //    /// <summary>
    //    /// Create object which uses default implementation for random generators. 
    //    /// </summary>
    //    public MOFWOptimizer() : this(new ContUniformDist(), new NormalDist())
    //    {
    //    }

    //    /// <summary>
    //    /// Create object which uses custom implementation for random generators. 
    //    /// </summary>
    //    /// <param name="UniformGen"> Object, which implements <see cref="IContUniformGen"/> interface. </param>
    //    /// <param name="NormalGen">  Object, which implements <see cref="INormalGen"/> interface. </param>
    //    /// <exception cref="ArgumentNullException">
    //    /// If <paramref name="NormalGen"/> or <paramref name="UniformGen"/> is null.
    //    /// </exception>
    //    public MOFWOptimizer(IContUniformGen UniformGen, INormalGen NormalGen)
    //    {
    //        if (UniformGen == null)
    //        {
    //            throw new ArgumentNullException(nameof(UniformGen));
    //        }

    //        if (NormalGen == null)
    //        {
    //            throw new ArgumentNullException(nameof(NormalGen));
    //        }

    //        _uniformRand = UniformGen;

    //        _normalRand = NormalGen;
    //    }

    //    ///// <summary>
    //    ///// Find best solution among debris and charges.
    //    ///// </summary>
    //    //private void FindBestSolution()
    //    //{
    //    //    double min1 = _chargePoints[0].Obj;

    //    // // The index of the best solution among charges. _indexMinChargePoints = 0;

    //    // // Searching best solution among charges. for (int i = 1; i < _parameters.NP; i++) { if
    //    // (_chargePoints[i].Obj < min1) { min1 = _chargePoints[i].Obj; _indexMinChargePoints = i; } }

    //    // // The indexes of the best solutions among debris. _indexMinDebris1 = 0; _indexMinDebris2
    //    // = 0;

    //    // double min2 = _debris[0].First.Value.Obj;

    //    // // Searching best solution among debris. for (int j = 0; j < _parameters.NP; j++) { int k
    //    // = 0;

    //    // foreach (OOAgent splinter in _debris[j]) { if (splinter.Obj < min2) { min2 = splinter.Obj;

    //    // _indexMinDebris1 = j; _indexMinDebris2 = k; } k++; } }
    //    /// <summary>
    //    /// <see cref="IOOOptimizer{T}.Minimize(GeneralParams)"/> 
    //    /// </summary>
    //    /// <param name="GenParams"> General parameters. <see cref="GeneralParams"/>. </param>
    //    /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
    //    /// <exception cref="ArgumentNullException"> If <paramref name="GenParams"/> is null. </exception>
    //    /// <exception cref="ArithmeticException">
    //    /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
    //    /// </exception>
    //    public void Minimize(FWParams Parameters, MOOptimizationProblem Problem)
    //    {
    //        Init(Parameters, Problem.LowerBounds.Count, Problem.TargetFunction.Count);

    //        FirstStep(Problem);

    //        for (int i = 1; i < this._parameters.Imax; i++)
    //        {
    //            NextStep(Problem);
    //        }

    //        Clear();
    //    }

    //    /// <summary>
    //    /// <see cref="IOOOptimizer{T}.Minimize(GeneralParams, CancellationToken)"/> 
    //    /// </summary>
    //    /// <param name="GenParams">   General parameters. <see cref="GeneralParams"/>. </param>
    //    /// <param name="CancelToken"> <see cref="CancellationToken"/> </param>
    //    /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
    //    /// <exception cref="ArgumentNullException"> If <paramref name="GenParams"/> is null. </exception>
    //    /// <exception cref="ArithmeticException">
    //    /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
    //    /// </exception>
    //    /// <exception cref="OperationCanceledException"></exception>
    //    public void Minimize(FWParams Parameters, MOOptimizationProblem Problem, CancellationToken CancelToken)
    //    {
    //        Init(Parameters, Problem.LowerBounds.Count, Problem.TargetFunction.Count);

    //        FirstStep(Problem);

    //        for (int i = 1; i < this._parameters.Imax; i++)
    //        {
    //            CancelToken.ThrowIfCancellationRequested();
    //            NextStep(Problem);
    //        }

    //        Clear();
    //    }

    //    /// <summary>
    //    /// <see cref="IOOOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/> 
    //    /// </summary>
    //    /// <param name="GenParams"> General parameters. <see cref="GeneralParams"/>. </param>
    //    /// <param name="Reporter"> 
    //    /// Object which implement interface <see cref="IProgress{T}"/>, where T is
    //    /// <see cref="Progress"/>. <seealso cref="IOOOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
    //    /// </param>
    //    /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
    //    /// <exception cref="ArgumentNullException">
    //    /// If <paramref name="GenParams"/> or <paramref name="Reporter"/> is null.
    //    /// </exception>
    //    /// <exception cref="ArithmeticException">
    //    /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
    //    /// </exception>
    //    public void Minimize(FWParams Parameters, MOOptimizationProblem Problem, IProgress<Progress> Reporter)
    //    {
    //        if (Reporter == null)
    //        {
    //            throw new ArgumentNullException(nameof(Reporter));
    //        }

    //        Init(Parameters, Problem.LowerBounds.Count, Problem.TargetFunction.Count);

    //        FirstStep(Problem);

    //        Progress progress = new Progress(this, 0, this._parameters.Imax - 1, 0);

    //        Reporter.Report(progress);

    //        for (int i = 1; i < this._parameters.Imax; i++)
    //        {
    //            NextStep(Problem);
    //            progress.Current = i;
    //            Reporter.Report(progress);
    //        }

    //        Clear();
    //    }

    //    /// <summary>
    //    /// <see cref="IOOOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/> 
    //    /// </summary>
    //    /// <param name="GenParams"> General parameters. <see cref="GeneralParams"/>. </param>
    //    /// <param name="Reporter"> 
    //    /// Object which implement interface <see cref="IProgress{T}"/>, where T is
    //    /// <see cref="Progress"/>.
    //    /// <seealso cref="IOOOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/><param name="CancelToken"> <see cref="CancellationToken"/></param>
    //    /// </param>
    //    /// <exception cref="InvalidOperationException"> If parameters do not set. </exception>
    //    /// <exception cref="ArgumentNullException">
    //    /// If <paramref name="GenParams"/> or <paramref name="Reporter"/> is null.
    //    /// </exception>
    //    /// <exception cref="ArithmeticException">
    //    /// If the function has value is NaN, PositiveInfinity or NegativeInfinity.
    //    /// </exception>
    //    /// <exception cref="OperationCanceledException"></exception>
    //    public void Minimize(FWParams Parameters, MOOptimizationProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken)
    //    {
    //        if (Reporter == null)
    //        {
    //            throw new ArgumentNullException(nameof(Reporter));
    //        }

    //        Init(Parameters, Problem.LowerBounds.Count, Problem.TargetFunction.Count);

    //        FirstStep(Problem);

    //        Progress progress = new Progress(this, 0, this._parameters.Imax - 1, 0);

    //        Reporter.Report(progress);

    //        for (int i = 1; i < this._parameters.Imax; i++)
    //        {
    //            CancelToken.ThrowIfCancellationRequested();

    //            NextStep(Problem);
    //            progress.Current = i;
    //            Reporter.Report(progress);
    //        }

    //        Clear();
    //    }
    //}
}