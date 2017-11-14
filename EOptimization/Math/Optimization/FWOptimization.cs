// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;

    using Math.Random;
    using Math.LA;
    using Math;
    using Exceptions;

    using Help;


    /// <summary>
    /// Optimization method Fireworks.
    /// </summary>
    public class FWOptimizer : IOptimizer<FWParams>
    {
        /// <summary>
        /// If isInitiParams = true, then parameters did not set.
        /// </summary>
        private bool isInitParams;

        // The temporary array has a 'Length' is equal 'dimension'.
        // Used in the calculation the value of the target function.
        private double[] tempArray;

        // Used in the method 'GenerateIndexesOfAxes'.
        private int[] coordNumbers;

        private int dimension, indexmin1, indexmin2, indexmin3;

        private double fmax, fmin;

        private FWParams parametrs;

        /// <summary>
        /// Charges.
        /// </summary>
        private List<PointND> chargePoints;

        /// <summary>
        /// Debris for charges.
        /// </summary>
        private List<PointND>[] debris;

        private IContUniformGenerator uniformRand;

        private INormalGenerator normalRand;

        private PointND solution, tempPoint1, tempPoint2;


        /// <summary>
        /// Structure for internal computation.
        /// </summary>
        private struct WeightOfPoint
        {
            public double Distance { get; set; }

            public PointND Point { get; private set; }

            public bool IsTake { get; set; }

            public WeightOfPoint(PointND Point, double Dist)
            {
                this.Point = Point;
                this.Distance = Dist;
                this.IsTake = false;
            }
        }



        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        public PointND Solution => solution;

        /// <summary>
        /// Parameters for Fireworks method. <see cref="FWParams"/>.
        /// </summary>
        public FWParams Parameters => parametrs;

        /// <summary>
        /// Create object which uses default implementation for random generators.
        /// </summary>
        public FWOptimizer() : this(new ContUniformDistribution(), new NormalDistribution())
        {

        }

        /// <summary>
        /// Create object which uses custom implementation for random generators.
        /// </summary>
        /// <param name="UniformGen">Object, which implements <see cref="IContUniformGenerator"/> interface.</param>
        /// <param name="NormalGen">Object, which implements <see cref="INormalGenerator"/> interface.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="NormalGen"/> or <paramref name="UniformGen"/> is null.</exception>
        public FWOptimizer(IContUniformGenerator UniformGen, INormalGenerator NormalGen)
        {
            if (UniformGen == null)
            {
                throw new ArgumentNullException(nameof(UniformGen));
            }

            if (NormalGen == null)
            {
                throw new ArgumentNullException(nameof(NormalGen));
            }

            uniformRand = UniformGen;

            normalRand = NormalGen;

            isInitParams = true;
        }



        private void InitializePopulation(double[] LowerBound, double[] UpperBound)
        {
            dimension = LowerBound.Length;

            if (tempArray == null)
                tempArray = new double[dimension];
            else if (tempArray.Length != dimension)
                tempArray = new double[dimension];

            if (coordNumbers == null)
                coordNumbers = new int[dimension];
            else if (coordNumbers.Length != dimension)
                coordNumbers = new int[dimension];

            if (tempPoint1 == null)
                tempPoint1 = new PointND(0, dimension);
            else if (tempPoint1.Dimension != dimension)
                tempPoint1 = new PointND(0, dimension);

            if (tempPoint2 == null)
                tempPoint2 = new PointND(0, dimension);
            else if (tempPoint2.Dimension != dimension)
                tempPoint2 = new PointND(0, dimension);

            if (solution == null)
                solution = new PointND(0, dimension + 1);
            else if (solution.Dimension != dimension + 1)
                solution = new PointND(0, dimension + 1);

            if (chargePoints == null)
                chargePoints = new List<PointND>(this.parametrs.NP);

            if (debris == null)
                InitDebris();
            else if (debris.Length != this.Parameters.NP)
                InitDebris();


            PointND temp = new PointND(0, dimension + 1);

            // Create points of explosion.
            for (int i = 0; i < this.parametrs.NP; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    temp[j] = uniformRand.URandVal(LowerBound[j], UpperBound[j]);
                }

                this.chargePoints.Add(temp.DeepCopy());
            }


        }

        private void InitDebris()
        {
            debris = new List<PointND>[this.parametrs.NP];

            for (int i = 0; i < debris.Length; i++)
            {
                debris[i] = new List<PointND>();
            }
        }


        private void Clear()
        {
            chargePoints.Clear();

            for (int i = 0; i < debris.Length; i++)
            {
                debris[i].Clear();
            }

        }

        private void CalculateFunction(Func<double[], double> Function)
        {

            double value = 0;

            for (int i = 0; i < this.parametrs.NP; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    tempArray[j] = chargePoints[i][j];
                }

                value = Function(tempArray);

                try
                {
                    CheckDouble.CheckInvalidValue(value);
                }
                catch (ArithmeticException exc)
                {
                    throw new InvalidValueFunctionException($"Function has an invalid value at point.\n{exc.Message}", new PointND(tempArray),
                        value);
                }

                chargePoints[i][dimension] = value;
            }
        }

        private void FindFMaxMin()
        {
            // Last coordinate stores value of the target function.
            fmin = chargePoints.Min(a => a[dimension]);

            fmax = chargePoints.Max(a => a[dimension]);
        }


        /// <summary>
        /// Find amount debris for each point of charge.
        /// </summary>
        private void FindAmountDebris()
        {
            double s = 0;

            double denumerator = 0;

            int numOfDebris = 0;

            for (int i = 0; i < this.parametrs.NP; i++)
            {
                denumerator += fmax - this.chargePoints[i][dimension];
            }

            denumerator += Constants.ValueForAvoidDivByZero;


            for (int i = 0; i < this.parametrs.NP; i++)
            {
                s = parametrs.M * (fmax - this.chargePoints[i][dimension] + Constants.ValueForAvoidDivByZero) / denumerator;

                if (s < parametrs.Alpha * parametrs.M)
                    s = Math.Truncate(parametrs.Alpha * parametrs.M);
                else if (s > parametrs.Beta * parametrs.M)
                    s = Math.Truncate(parametrs.Beta * parametrs.M);
                else
                    s = Math.Truncate(s);

                numOfDebris = (int)s;

                if (numOfDebris == 0 && i == 0)
                    numOfDebris = 1;

                if (numOfDebris == 0)
                {
                    debris[i].Clear();
                }
                else if (debris[i].Count > numOfDebris)
                {
                    debris[i].RemoveRange(numOfDebris, debris[i].Count - numOfDebris);
                }
                else if (debris[i].Count < numOfDebris)
                {
                    int total = numOfDebris - debris[i].Count;

                    for (int j = 0; j < total; j++)
                    {
                        debris[i].Add(new PointND(0, dimension + 1));
                    }
                }
            }
        }


        /// <summary>
        /// First method for determination of position of the debris.
        /// </summary>
        /// <param name="CountOfDimension"></param>
        /// <param name="Amplitude"></param>
        /// <param name="IndexCurrentCharge"></param>
        /// <param name="IndexCurrentDebris"></param>
        /// <param name="LowerBound"></param>
        /// <param name="UpperBound"></param>
        private void FirstMethodDeterminationOfPosition(int CountOfDimension, double Amplitude,
            int IndexCurrentCharge, int IndexCurrentDebris, double[] LowerBound, double[] UpperBound)
        {
            // The indices are choosing randomly.
            GenerateIndexesOfAxes();

            double h = 0;

            PointND temp = null;

            temp = this.debris[IndexCurrentCharge][IndexCurrentDebris];

            // Calculate position of debris.
            for (int i = 0; i < CountOfDimension; i++)
            {
                int axisIndex = coordNumbers[i];

                h = Amplitude * this.uniformRand.URandVal(-1, 1);

                temp[axisIndex] += h;

                // If point leaves region then it returns to random position.
                if (temp[axisIndex] < LowerBound[axisIndex] || temp[axisIndex] > UpperBound[axisIndex])
                {
                    temp[axisIndex] = uniformRand.URandVal(LowerBound[axisIndex], UpperBound[axisIndex]);
                }
            }
        }

        /// <summary>
        /// Generate randomly indices of axes.
        /// </summary>
        /// <returns></returns>
        private void GenerateIndexesOfAxes()
        {
            // Set coordinate numbers.
            for (int i = 0; i < dimension; i++)
            {
                coordNumbers[i] = i;
            }

            EOpt.Math.Сombinatorics.RandomPermutation(coordNumbers, SyncRandom.Get());
        }

        /// <summary>
        /// Second method for determination of position of the debris.
        /// </summary>
        /// <param name="CountOfDimension"></param>
        /// <param name="IndexCurrentCharge"></param>
        /// <param name="IndexCurrentDebris"></param>
        /// <param name="LowerBound"></param>
        /// <param name="UpperBound"></param>
        private void SecondMethodDeterminationOfPosition(int CountOfDimension,
            int IndexCurrentCharge, int IndexCurrentDebris, double[] LowerBound, double[] UpperBound)
        {
            GenerateIndexesOfAxes();

            double g = 0;

            PointND temp;

            temp = this.debris[IndexCurrentCharge][IndexCurrentDebris];

            // Calculate position of debris.
            for (int i = 0; i < CountOfDimension; i++)
            {
                int axisIndex = coordNumbers[i];

                g = this.normalRand.NRandVal(1, 1);

                temp[axisIndex] *= g;

                // If point leave region that she return to random position.
                if (temp[axisIndex] < LowerBound[axisIndex] || temp[axisIndex] > UpperBound[axisIndex])
                {
                    temp[axisIndex] = uniformRand.URandVal(LowerBound[axisIndex], UpperBound[axisIndex]);
                }
            }
        }

        /// <summary>
        /// Determine debris position.
        /// </summary>
        /// <param name="LowerBound"></param>
        /// <param name="UpperBound"></param>
        /// <param name="Function"></param>
        private void GenerateDebris(double[] LowerBound, double[] UpperBound, Func<double[], double> Function)
        {
            double denumerator = 0;

            for (int j = 0; j < this.parametrs.NP; j++)
            {
                denumerator += chargePoints[j][dimension] - fmin;
            }

            denumerator += Constants.ValueForAvoidDivByZero;

            PointND temp;


            for (int i = 0; i < this.parametrs.NP; i++)
            {

                double amplitude = 0;

                // Amplitude of explosion.
                amplitude = parametrs.Amax * (chargePoints[i][dimension] - fmin + Constants.ValueForAvoidDivByZero) / denumerator;


                // For each debris.
                for (int k = 0; k < this.debris[i].Count; k++)
                {
                    // The position of debris sets to the position of charge.
                    this.debris[i][k].SetAt(this.chargePoints[i]);

                    double ksi = uniformRand.URandVal(0, 1);

                    int CountOfDimension = (int)Math.Ceiling(dimension * ksi);

                    if (ksi < 0.5)
                    {
                        FirstMethodDeterminationOfPosition(CountOfDimension, amplitude, i, k, LowerBound, UpperBound);
                    }
                    else
                    {
                        SecondMethodDeterminationOfPosition(CountOfDimension, i, k, LowerBound, UpperBound);
                    }
                }

            }

            for (int i = 0; i < this.parametrs.NP; i++)
            {
                for (int j = 0; j < this.debris[i].Count; j++)
                {
                    temp = debris[i][j];

                    // Coordinates of the point are copying to the array.
                    for (int k = 0; k < temp.Dimension - 1; k++)
                    {
                        tempArray[k] = this.debris[i][j][k];
                    }

                    temp[dimension] = Function(tempArray);
                }
            }
        }

        /// <summary>
        /// Find best solution among debris and charges.
        /// </summary>
        private void FindBestSolution()
        {
            double min1 = this.chargePoints[0][dimension];

            // The index of the best solution among  charges.
            indexmin1 = 0;

            // Searching best solution among charges.
            for (int i = 1; i < this.parametrs.NP; i++)
            {
                if (this.chargePoints[i][dimension] < min1)
                {
                    min1 = this.chargePoints[i][dimension];
                    indexmin1 = i;
                }
            }

            // The indexes of the best solutions among debris.
            indexmin2 = 0;
            indexmin3 = 0;

            double min2 = this.debris[0][0][dimension];

            // Searching best solution among debris.
            for (int j = 0; j < this.parametrs.NP; j++)
            {
                for (int k = 0; k < this.debris[j].Count; k++)
                {
                    if (this.debris[j][k][dimension] < min2)
                    {
                        min2 = this.debris[j][k][dimension];

                        indexmin2 = j;
                        indexmin3 = k;
                    }
                }
            }



            // Select best solution among debris and charges.
            if (min1 < min2)
            {
                this.solution.SetAt(chargePoints[indexmin1]);
                indexmin2 = -1;
                indexmin3 = -1;
            }
            else
            {
                this.solution.SetAt(this.debris[indexmin2][indexmin3]);
                indexmin1 = -1;
            }
        }

        /// <summary>
        /// Generate current population.
        /// </summary>
        /// <param name="DistanceFunction">Distance between points.</param>
        private void GenerateCurrentPopulation(Func<PointND, PointND, double> DistanceFunction)
        {
            // Minus solution.
            // Solution is always on the 'chargePoints'.
            int totalPoints = this.chargePoints.Count - 1;

            for (int k = 0; k < debris.Length; k++)
            {

                totalPoints += debris[k].Count;
            }


            // The structure is storing a point and it weight.
            WeightOfPoint[] weights = new WeightOfPoint[totalPoints];

            {
                int index = 0;

                for (int i = 0; i < chargePoints.Count; i++)
                {
                    // Skip solution.
                    if (i == indexmin1)
                        continue;

                    weights[index] = new WeightOfPoint(chargePoints[i], 0);
                    index++;
                }

                for (int i = 0; i < debris.Length; i++)
                {
                    for (int j = 0; j < debris[i].Count; j++)
                    {
                        // Skip solution, if it is debris.
                        if (indexmin2 == i && indexmin3 == j)
                            continue;

                        weights[index] = new WeightOfPoint(debris[i][j], 0);
                        index++;

                    }
                }
            }

            double denumeratorForProbability = 0;

            SymmetricMatrix matrixOfDistances = new SymmetricMatrix(totalPoints, 0);

            // Calculate distance between all points.
            for (int i = 0; i < totalPoints; i++)
            {
                // First point.
                for (int l = 0; l < dimension; l++)
                {
                    tempPoint1[l] = weights[i].Point[l];
                }

                for (int j = i + 1; j < totalPoints; j++)
                {
                    // Last coordinate of point is storing the value of target function.
                    // Do not need to take the last coordinate.
                    for (int l = 0; l < dimension; l++)
                    {
                        tempPoint2[l] = weights[j].Point[l];
                    }

                    matrixOfDistances[i, j] = DistanceFunction(tempPoint1, tempPoint2);
                }
            }


            for (int ii = 0; ii < totalPoints; ii++)
            {
                double dist = 0;

                for (int j = 0; j < matrixOfDistances.ColumnCount; j++)
                {
                    dist += matrixOfDistances[ii, j];
                }

                weights[ii].Distance = dist;

                denumeratorForProbability += dist;
            }

            // Probability of explosion.
            for (int jj = 0; jj < totalPoints; jj++)
            {
                weights[jj].Distance /= denumeratorForProbability;
            }

            TakePoints(weights);

            this.chargePoints[0].SetAt(solution);

            int startIndex = 1;

            foreach (var weight in weights.Where(w => w.IsTake).Take(parametrs.NP - 1))
            {
                this.chargePoints[startIndex] = weight.Point;
                startIndex++;
            }

        }


        private void TakePoints(WeightOfPoint[] Weights)
        {
            int count = 0;

            // Take the points with a probability is equal Distance.
            for (int i = 0; i < Weights.Length; i++)
            {
                if (uniformRand.URandVal(0, 1) < Weights[i].Distance)
                {
                    Weights[i].IsTake = true;
                    Weights[i].Distance = Double.MaxValue;
                    count++;
                }
            }

            int remainder = parametrs.NP - 1 - count;

            // Need to take some points.
            if (remainder > 0)
            {
                // Sort by descending probability of explosion.
                // All points with IsTake = true will in the end.
                // Points with higher probability are taking.
                Array.Sort(Weights, (x, y) => x.Distance.CompareTo(y.Distance));

                for (int i = 0; i < remainder; i++)
                {
                    Weights[i].IsTake = true;
                }
            }


        }

        private void FirstStep(GeneralParams GenParams)
        {

            if (GenParams == null)
            {
                throw new ArgumentNullException(nameof(GenParams));
            }
            if (isInitParams)
                throw new InvalidOperationException($"First you need invoke {nameof(InitializeParameters)}.");

            InitializePopulation(GenParams.LowerBound, GenParams.UpperBound);

            CalculateFunction(GenParams.TargetFunction);

            FindFMaxMin();

            FindAmountDebris();

            GenerateDebris(GenParams.LowerBound, GenParams.UpperBound, GenParams.TargetFunction);

            FindBestSolution();
        }

        private void NextStep(GeneralParams genParams)
        {
            GenerateCurrentPopulation(parametrs.DistanceFunction);

            CalculateFunction(genParams.TargetFunction);

            FindFMaxMin();

            FindAmountDebris();

            GenerateDebris(genParams.LowerBound, genParams.UpperBound, genParams.TargetFunction);

            FindBestSolution();
        }



        /// <summary>
        /// <see cref="IOptimizer{T}.InitializeParameters(T)"/>
        /// </summary>
        /// <param name="Parameters">Parameters for method. Must be type <see cref="FWParams"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="Parameters"/> is null.</exception>
        public void InitializeParameters(FWParams Parameters)
        {

            if (Parameters == null)
            {
                throw new ArgumentNullException(nameof(Parameters));
            }

            this.parametrs = Parameters;

            this.chargePoints = null;
            this.debris = null;

            isInitParams = false;
        }


        /// <summary>
        /// <see cref="IOptimizer{T}.Minimize(GeneralParams)"/>
        /// </summary>
        /// <param name="GenParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <exception cref="InvalidOperationException">If parameters do not set.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="GenParams"/> is null.</exception>
        /// <exception cref="ArithmeticException">If the function has value is NaN, PositiveInfinity or NegativeInfinity.</exception>
        public void Minimize(GeneralParams GenParams)
        {
            FirstStep(GenParams);

            for (int i = 1; i < this.parametrs.Imax; i++)
            {
                NextStep(GenParams);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Minimize(GeneralParams, CancellationToken)"/>
        /// </summary>
        /// <param name="GenParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="CancelToken"><see cref="CancellationToken"/></param>
        /// <exception cref="InvalidOperationException">If parameters do not set.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="GenParams"/> is null.</exception>
        /// <exception cref="ArithmeticException">If the function has value is NaN, PositiveInfinity or NegativeInfinity.</exception>
        /// <exception cref="OperationCanceledException"></exception>
        public void Minimize(GeneralParams GenParams, CancellationToken CancelToken)
        {
            FirstStep(GenParams);

            for (int i = 1; i < this.parametrs.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();
                NextStep(GenParams);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// </summary>
        /// <param name="GenParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="Reporter">Object which implement interface <see cref="IProgress{T}"/>, 
        /// where T is <see cref="Progress"/>. 
        /// <seealso cref="IOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// </param>
        /// <exception cref="InvalidOperationException">If parameters do not set.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="GenParams"/> or <paramref name="Reporter"/> is null.</exception>
        /// <exception cref="ArithmeticException">If the function has value is NaN, PositiveInfinity or NegativeInfinity.</exception>
        public void Minimize(GeneralParams GenParams, IProgress<Progress> Reporter)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            FirstStep(GenParams);

            Progress progress = new Progress(this, 0, this.parametrs.Imax - 1, 0);

            Reporter.Report(progress);

            for (int i = 1; i < this.parametrs.Imax; i++)
            {
                NextStep(GenParams);
                progress.Current = i;
                Reporter.Report(progress);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// </summary>
        /// <param name="GenParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="Reporter">Object which implement interface <see cref="IProgress{T}"/>, 
        /// where T is <see cref="Progress"/>. 
        /// <seealso cref="IOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// <param name="CancelToken"><see cref="CancellationToken"/></param>
        /// </param>
        /// <exception cref="InvalidOperationException">If parameters do not set.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="GenParams"/> or <paramref name="Reporter"/> is null.</exception>
        /// <exception cref="ArithmeticException">If the function has value is NaN, PositiveInfinity or NegativeInfinity.</exception>
        /// <exception cref="OperationCanceledException"></exception>
        public void Minimize(GeneralParams GenParams, IProgress<Progress> Reporter, CancellationToken CancelToken)
        {
            if (Reporter == null)
            {
                throw new ArgumentNullException(nameof(Reporter));
            }

            FirstStep(GenParams);

            Progress progress = new Progress(this, 0, this.parametrs.Imax - 1, 0);

            Reporter.Report(progress);

            for (int i = 1; i < this.parametrs.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();

                NextStep(GenParams);
                progress.Current = i;
                Reporter.Report(progress);
            }

            Clear();
        }
    }
}

