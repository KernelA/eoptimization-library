namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;

    using Math.Random;
    using Math.LA;

    /// <summary>
    /// Optimization method Fireworks.
    /// </summary>
    public class FWOptimizer : IOptimizer<FWParams>
    {
        /// <summary>
        /// If initParamsQ = true, then not set parameters.
        /// </summary>
        private bool initParamsQ;

        private int Dimension;

        private double fmax, fmin;

        private FWParams parametrs;
        
        /// <summary>
        /// Charges.
        /// </summary>
        private List<PointND> chargePoints;

        /// <summary>
        /// Debris for charges. Index
        /// </summary>
        private List<PointND>[] debris;

        private IContUniformGenerator uniformRand;

        private INormalGenerator normalRand;

        private PointND bestSolution;


        /// <summary>
        /// Structure for internal computation.
        /// </summary>
        private struct WeightOfPoint
        {
            public double Distance { get;  set; }

            public int Index { get; private set; }

            public WeightOfPoint(int index, double dist)
            {
                this.Index = index;
                this.Distance = dist;
            }
        }



        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        public PointND Solution
        {
            get
            {
                return bestSolution;
            }
        }

        /// <summary>
        /// Parameters for Fireworks method. <see cref="FWParams"/>.
        /// </summary>
        public FWParams Parameters
        {
            get
            {
                return parametrs;
            }
        }

        /// <summary>
        /// Create object which use default implementation for random generators.
        /// </summary>
        public FWOptimizer() : this(new ContUniformDistribution(), new NormalDistribution())
        {

        }

        /// <summary>
        /// Create object which use custom implementation for random generators.
        /// </summary>
        /// <param name="uniformGen">Object, which implements <see cref="IContUniformGenerator"/> interface, for generating uniform random value.</param>
        /// <param name="normalGen">Object, which implements <see cref="INormalGenerator"/> interface, for generating uniform random value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FWOptimizer(IContUniformGenerator uniformGen, INormalGenerator normalGen)
        {
            if (uniformGen == null)
            {
                throw new ArgumentNullException(nameof(uniformGen));
            }

            if (normalGen == null)
            {
                throw new ArgumentNullException(nameof(normalGen));
            }

            uniformRand = uniformGen;

            normalRand = normalGen;

            initParamsQ = true;
        }



        private void InitializePopulation(double[] a, double[] b)
        {
            Dimension = a.Length;

            chargePoints = new List<PointND>(this.parametrs.NP);

            debris = new List<PointND>[this.parametrs.NP];

            for (int i = 0; i < this.parametrs.NP; i++)
            {
                this.debris[i] = null;
            }

            PointND temp = new PointND(0, Dimension + 1);

            // Create points of explosion.
            for (int i = 0; i < this.parametrs.NP; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    temp[j] = uniformRand.URandVal(a[j], b[j]);
                }

                this.chargePoints.Add(temp.Clone());
            }

           
        }


        private void CalculateFunction(Func<double[], double> function)
        {
            double[] temp = new double[Dimension];

            for (int i = 0; i < this.parametrs.NP; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    temp[j] = chargePoints[i][j];
                }

                chargePoints[i][Dimension] = function(temp);
            }
        }

        private void FindFMaxMin()
        {
            // Searching maximum and minimum value of target function for charge points.
            fmin = chargePoints.Min(a => a[Dimension]);

            fmax = chargePoints.Max(a => a[Dimension]);

        }


        /// <summary>
        /// Find amount debris for each point of charge.
        /// </summary>
        private void FindAmountDebris()
        {
            double s = 0;

            double denumerator = 0;

            for (int i = 0; i < this.parametrs.NP; i++)
            {
                denumerator += fmax - this.chargePoints[i][Dimension];
            }

            denumerator += Constants.Psi;


            for (int i = 0; i < this.parametrs.NP; i++)
            {
                s = parametrs.M * (fmax - this.chargePoints[i][Dimension] + Constants.Psi) / denumerator;

                if (s < parametrs.Alpha * parametrs.M)
                    s = Math.Round(parametrs.Alpha * parametrs.M);
                else if (s > parametrs.Beta * parametrs.M)
                    s = Math.Round(parametrs.Beta * parametrs.M);
                else
                    s = Math.Round(s);

                if ((int)s == 0 && i == 0)
                    s = 1;

                if (this.debris[i] != null)
                {
                    this.debris[i].Clear();
                }
                else
                {
                    this.debris[i] = new List<PointND>((int)s);
                }
            }
        }

   
        /// <summary>
        /// First method for determination of position debris.
        /// </summary>
        /// <param name="CountOfDimension"></param>
        /// <param name="Amplitude"></param>
        /// <param name="IndexCurrentCharge"></param>
        /// <param name="IndexCurrentDebris"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void FirstMethodDeterminationOfPosition(int CountOfDimension, double Amplitude,
            int IndexCurrentCharge, int IndexCurrentDebris, double[] a, double[] b)
        {
            int[] numOfAxis = GenerateIndexesOfAxes(CountOfDimension);

            double h = 0;

            PointND temp = null;

            temp = this.debris[IndexCurrentCharge][IndexCurrentDebris];

            // Calculate position of debris.
            foreach (int k in numOfAxis)
            {
                h = Amplitude * this.uniformRand.URandVal(-1,1);

                temp[k] += h;

                // If point leave region that she return to random position.
                if (temp[k] < a[k] || temp[k] > b[k])
                {
                    temp[k] = uniformRand.URandVal(a[k], b[k]);
                }
            }
        }

        /// <summary>
        /// Generate randomly <paramref name="CountOfDimension"/> indexes of axes for choosing.
        /// </summary>
        /// <param name="CountOfDimension"></param>
        /// <returns></returns>
        private int[] GenerateIndexesOfAxes(int CountOfDimension)
        {
            // Coordinate numbers.
            int[] coordNumbers = new int[Dimension];

            // Set coordinate numbers.
            for (int i = 0; i < Dimension; i++)
            {
                coordNumbers[i] = i;
            }

            ML.Math.Сombinatorics.RandomPermutation(coordNumbers, SyncRandom.Get());

           
            // Randomly choose indices's.
            // Select first CountOfDimension indexes.
            return coordNumbers.Take(CountOfDimension).ToArray();
        }

        /// <summary>
        /// Second method for determination of position debris.
        /// </summary>
        /// <param name="CountOfDimension"></param>
        /// <param name="IndexCurrentCharge"></param>
        /// <param name="IndexCurrentDebris"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void SecondMethodDeterminationOfPosition(int CountOfDimension,
            int IndexCurrentCharge, int IndexCurrentDebris, double[] a, double[] b)
        {
            int[] numofAxes = GenerateIndexesOfAxes(CountOfDimension);

            double g = 0;

            PointND temp;

            temp = this.debris[IndexCurrentCharge][IndexCurrentDebris];

            // Calculate position of debris.
            foreach (int k in numofAxes)
            {
                g = this.normalRand.NRandVal(1,1);

                temp[k] *= g;

                // If point leave region that she return to random position.
                if (temp[k] < a[k] || temp[k] > b[k])
                {
                    temp[k] = uniformRand.URandVal(a[k], b[k]);
                }
            }
        }

        /// <summary>
        /// Determine debris position.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="function"></param>
        private void GenerateDebris(double[] a, double[] b, Func<double[], double> function)
        {
            double denumerator = 0;

            for (int j = 0; j < this.parametrs.NP; j++)
            {
                denumerator += chargePoints[j][Dimension] - fmin;
            }

            denumerator += Constants.Psi;

            PointND temp;

            
            for (int i = 0; i < this.parametrs.NP; i++)
            {

                double amplitude = 0;

                // Amplitude of explosion.
                amplitude = parametrs.Amax * (chargePoints[i][Dimension] - fmin + Constants.Psi) / denumerator;


                // For each debris.
                for (int k = 0; k < this.debris[i].Capacity; k++)
                {
                    this.debris[i].Add(this.chargePoints[i].Clone());

                    double ksi = uniformRand.URandVal(0, 1);

                    int CountOfDimension = (int)Math.Round(Dimension * ksi);

                    if (ksi < 0.5)
                    {
                        FirstMethodDeterminationOfPosition(CountOfDimension, amplitude, i, k, a, b);
                    }
                    else
                    {
                        SecondMethodDeterminationOfPosition(CountOfDimension, i, k, a, b);
                    }
                }

            }

            for (int i = 0; i < this.parametrs.NP; i++)
            {
                for (int j = 0; j < this.debris[i].Count; j++)
                {
                    temp = this.debris[i][j];
                    temp[Dimension] = function(temp.Coordinates.Take(Dimension).ToArray());
                }
            }
        }

        /// <summary>
        /// Find best solution among debris and charges.
        /// </summary>
        private void FindBestSolution()
        {
            double min1 = this.chargePoints[0][Dimension];

            // The index of the best solution among charges.
            int indexmin1 = 0;

            // Searching best solution among charges.
            for (int i = 1; i < this.parametrs.NP; i++)
            {
                if (this.chargePoints[i][Dimension] < min1)
                {
                    min1 = this.chargePoints[i][Dimension];
                    indexmin1 = i;
                }
            }

            // The indexes of the best solutions among debris.
            int indexmin2 = 0;
            int indexmin3 = 0;

            double min2 = this.debris[0][0][Dimension];

            // Searching best solution among debris.
            for (int j = 0; j < this.parametrs.NP; j++)
            {
                for (int k = 0; k < this.debris[j].Count; k++)
                {
                    if (this.debris[j][k][Dimension] < fmin)
                    {
                        fmin = this.debris[j][k][Dimension];

                        indexmin2 = j;
                        indexmin3 = k;
                    }
                }
            }

            // Select best solution among debris and charges.
            if (min1 < min2)
            {
                this.bestSolution = this.chargePoints[indexmin1];
                this.chargePoints.RemoveAt(indexmin1);
            }
            else
            {
                this.bestSolution = this.debris[indexmin2][indexmin3];
                this.debris[indexmin2].RemoveAt(indexmin3);
            }
        }

        /// <summary>
        /// Generate current population.
        /// </summary>
        /// <param name="distanceFunction">Function for compute distance between points.</param>
        private void GenerateCurrentPopulation(Func<PointND, PointND, double> distanceFunction)
        {
            // Copy all debris to one list.
            for (int j = 0; j < this.parametrs.NP; j++)
            {
                this.chargePoints.AddRange(this.debris[j]);

                this.debris[j].Clear();
            }

            // Array for charges and debris.
            PointND[] allPoints = this.chargePoints.ToArray();

            this.chargePoints.Clear();

            // Structure for storing weight of point and her index in chargePoints list.
            WeightOfPoint[] weightes = new WeightOfPoint[allPoints.Length];

            PointND point1 = new PointND(0, Dimension);
            PointND point2 = new PointND(0, Dimension);

            double denumeratorForP = 0;

            SymmetricMatrix matrixOfDistances = new SymmetricMatrix(allPoints.Length, 0);


            // Calculate distance between all points.
            for (int i = 0; i < allPoints.Length; i++)
            {
                for (int j = i + 1; j < allPoints.Length; j++)
                {
                    // Last coordinate of point for storing the value of target function in this point.
                    // Do not need take last coordinate.
                    for (int l = 0; l < Dimension; l++)
                    {
                        point1[l] = allPoints[i][l];
                        point2[l] = allPoints[j][l];
                    }

                    matrixOfDistances[i, j] = distanceFunction(point1, point2);
                }           
            }

            for (int ii = 0; ii < allPoints.Length; ii++)
            {
                double dist = 0;

                for (int j = 0; j < matrixOfDistances.ColumnCount; j++)
                {
                    dist += matrixOfDistances[ii, j];
                }

                weightes[ii] = new WeightOfPoint(ii, dist);

                denumeratorForP += dist;
            }

            // Probability of explosion.
            for (int jj = 0; jj < allPoints.Length; jj++)
            {
                weightes[jj].Distance /= denumeratorForP;
            }

            // Sort by descending probability of explosion.
            Array.Sort<WeightOfPoint>(weightes, (x, y) => -1 * x.Distance.CompareTo(y.Distance));

            
            this.chargePoints.Add(this.bestSolution.Clone());

            // Points for new explosions (new charges).
            for (int i = 0; i < this.parametrs.NP - 1; i++)
            {
                this.chargePoints.Add(allPoints[weightes[i].Index]);
            }

        }

        private void FirstStep(GeneralParams genParams)
        {
            if (initParamsQ)
                throw new InvalidOperationException($"First you need invoke {nameof(InitializeParameters)}.");

            InitializePopulation(genParams.LeftBound, genParams.RightBound);

            CalculateFunction(genParams.ObjectiveFunction);

            FindFMaxMin();

            FindAmountDebris();

            GenerateDebris(genParams.LeftBound, genParams.RightBound, genParams.ObjectiveFunction);

            FindBestSolution();
        }

        private void NextStep(GeneralParams genParams)
        {
            GenerateCurrentPopulation(parametrs.DistanceFunction);

            CalculateFunction(genParams.ObjectiveFunction);

            FindFMaxMin();

            FindAmountDebris();

            GenerateDebris(genParams.LeftBound, genParams.RightBound, genParams.ObjectiveFunction);

            FindBestSolution();
        }



        /// <summary>
        /// <see cref="IOptimizer{T}.InitializeParameters(T)"/>
        /// </summary>
        /// <param name="Parameters">Parameters for method. Must be type <see cref="FWParams"/>.</param>
        public void InitializeParameters(FWParams Parameters)
        {

            if (Parameters == null)
            {
                throw new ArgumentNullException(nameof(Parameters));
            }

            this.parametrs = Parameters;

            this.chargePoints = null;
            this.debris = null;

            initParamsQ = false;
        }


        /// <summary>
        /// <see cref="IOptimizer{T}.Optimize(GeneralParams)"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Optimize(GeneralParams genParams)
        {
            FirstStep(genParams);

            for (int i = 1; i < this.parametrs.Imax; i++)
            {
                NextStep(genParams);
            }
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Optimize(GeneralParams, CancellationToken)"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="cancelToken"><see cref="CancellationToken"/></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Optimize(GeneralParams genParams, CancellationToken cancelToken)
        {
            FirstStep(genParams);

            for (int i = 1; i < this.parametrs.Imax; i++)
            {
                cancelToken.ThrowIfCancellationRequested();
                NextStep(genParams);
            }
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Tuple{object, int, int, int}})"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface <see cref="IProgress{T}"/>, 
        /// where T is tuple, first item in tuple is the self object, second item initial value, third item is the end value, fourth item is the current progress value. 
        /// <seealso cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Tuple{object, int, int, int}})"/>
        /// </param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void Optimize(GeneralParams genParams, IProgress<Tuple<object, int, int, int>> reporter)
        {
            if (reporter == null)
            {
                throw new ArgumentNullException(nameof(reporter));
            }

            FirstStep(genParams);

            reporter.Report(new Tuple<object, int, int, int>(this, 0, this.parametrs.Imax - 1, 0));

            for (int i = 1; i < this.parametrs.Imax; i++)
            {
                NextStep(genParams);

                reporter.Report(new Tuple<object, int, int, int>(this, 0, this.parametrs.Imax - 1, i));
            }
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Tuple{object, int, int, int}})"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface <see cref="IProgress{T}"/>, 
        /// where T is tuple, first item in tuple is the self object, second item initial value, third item is the end value, fourth item is the current progress value. 
        /// <seealso cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Tuple{object, int, int, int}})"/>
        /// <param name="cancelToken"><see cref="CancellationToken"/></param>
        /// </param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void Optimize(GeneralParams genParams, IProgress<Tuple<object, int, int, int>> reporter, CancellationToken cancelToken)
        {
            if (reporter == null)
            {
                throw new ArgumentNullException(nameof(reporter));
            }

            FirstStep(genParams);

            reporter.Report(new Tuple<object, int, int, int>(this, 0, this.parametrs.Imax - 1, 0));

            for (int i = 1; i < this.parametrs.Imax; i++)
            {
                cancelToken.ThrowIfCancellationRequested();

                NextStep(genParams);

                reporter.Report(new Tuple<object, int, int, int>(this, 0, this.parametrs.Imax - 1, i));
            }
        }
    }
}

