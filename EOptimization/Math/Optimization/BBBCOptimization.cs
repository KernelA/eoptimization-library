namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Math.Random;

    /// <summary>
    /// Optimization method BBBC.
    /// </summary>
    public class BBBCOptimizer : IOptimizer
    {
        /// <summary>
        /// If error = true, then not set parameters.
        /// </summary>
        private bool error;
        
        private int indexBestSolution;

        /// <summary>
        /// Dimension of space.
        /// </summary>
        private int dimension;

        /// <summary>
        /// Number, which add for avoiding division by zero.
        /// </summary>
        private const double psi = 1E-10;

        private PointND centerOfMass;

        private BBBCParams parametrs;

        private List<PointND> points;

        private IContUniformGenerator uniformRand;

        private INormalGenerator normalRand;

        /// <summary>
        /// Parameters for method.
        /// </summary>
        /// <value>
        /// Get only.
        /// </value>
        public BBBCParams Parameters
        {
            get
            {
                return parametrs;
            }
        }

        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        public PointND Solution
        {
            get
            {
                return points[indexBestSolution].Clone();
            }
        }

        /// <summary>
        /// Create object which use default implementation for random generators.
        /// </summary>
        public BBBCOptimizer() : this(new ContUniformDistribution(), new NormalDistribution())
        {

        }

        /// <summary>
        /// Create object which use custom implementation for random generators.
        /// </summary>
        /// <param name="uniformGen">Object, which implements <see cref="IContUniformGenerator"/> interface, for generating uniform random value.</param>
        /// <param name="normalGen">Object, which implements <see cref="INormalGenerator"/> interface, for generating uniform random value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BBBCOptimizer(IContUniformGenerator uniformGen, INormalGenerator normalGen)
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

            error = true;
        }


 
        private void InitializePopulation(double[] a, double[] b)
        {
            dimension = a.Length;

            points = new List<PointND>(parametrs.NP);

            // Last coordinate of point for storing the value of target function in this point.
            PointND point =  new PointND(0, dimension + 1);

            for (int i = 0; i < parametrs.NP; i++)
            {
                for (int j  = 0; j < dimension; j++)
                {
                    point[j] = uniformRand.URandVal(a[j], b[j]);
                }

                points.Add(point.Clone());
            }           
        }


        private void CalculateFunction(Func<double[], double> func)
        {
            // Last coordinate of point for storing the value of target function in this point.
            // We need copy coordinates. 
            double[] temp = new double[dimension];

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    temp[j] = points[i][j];
                }

                points[i][dimension] = func(temp);
            }
        }


        private void FindBestSolution()
        {
            double min = points[0][dimension];

            indexBestSolution = 0;

            for (int i = 1; i < points.Count; i++)
            {
                if(points[i][dimension] < min)
                {
                    min = points[i][dimension];
                    indexBestSolution = i;
                }
            }
        }


        private void FindCenterOfMass()
        {
            double denominator = 0.0;

            double temp = 0.0;

            PointND temppoint = new PointND(0, dimension + 1);

            for (int i = 0; i < parametrs.NP; i++)
            {
                temp = 1 / (points[i][dimension] + psi);

                denominator += temp;

                temppoint += temp * points[i];

            }

            centerOfMass = temppoint * (1 / denominator);
        }


        private void GenerateCurrentPopulation(double[] a, double[] b, int iterNum)
        {
            PointND tempBestSol = Solution;

            PointND tempPoint = new PointND(0, dimension + 1);

            points.Clear();

            points.Add(tempBestSol);

            for( int i = 0 ; i < parametrs.NP - 1; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    tempPoint[j] = parametrs.Beta * centerOfMass[j] + (1 - parametrs.Beta) * tempBestSol[j] + normalRand.NRandVal(0,1)
                        * parametrs.Alpha * (b[j] - a[j]) / iterNum;

                    // If point leave region that she return to neighborhood bound.
                    if (tempPoint[j] < a[j])
                        tempPoint[j] = tempPoint[j] + Math.Abs(tempPoint[j] - a[j]) + 0.1;
                    if (tempPoint[j] > b[j])
                        tempPoint[j] = tempPoint[j] - Math.Abs(tempPoint[j] - b[j]) - 0.1;
                }

                points.Add(tempPoint.Clone());
            }

        }

        /// <summary>
        /// <see cref="IOptimizer.InitializeParameters(object)"/>
        /// </summary>
        /// <param name="parameters">Parameters for method. Parameters must be type <see cref="BBBCParams"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void InitializeParameters(object parameters)
        {
            this.parametrs = parameters as BBBCParams;

            if (this.parametrs == null)
                throw new ArgumentException(nameof(parameters) + " type must be as " + nameof(BBBCParams), nameof(parameters));

            error = false;
        }

        /// <summary>
        /// <see cref="IOptimizer.Optimize(GeneralParams)"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface <see cref="IProgress{Tuple{int,int,int}}"/>, where first item in tuple is the initial progress value, 
        /// second item is the end progress value, third item is the current progress value. <seealso cref="IOptimizer.Optimize(GeneralParams, IProgress{Tuple{int, int, int}})"/>.
        /// </param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Optimize(GeneralParams genParams, IProgress<Tuple<int,int,int>> reporter = null)
        {
            if (error)
                throw new InvalidOperationException($"Before you need invoke {nameof(InitializeParameters)}.");

            reporter?.Report(new Tuple<int,int,int>(1, this.parametrs.Imax, 1));

            InitializePopulation(genParams.LeftBound, genParams.RightBound);

            CalculateFunction(genParams.ObjectiveFunction);

            FindBestSolution();

            for (int i = 2; i <= this.parametrs.Imax; i++)
            {
                FindCenterOfMass();
                GenerateCurrentPopulation(genParams.LeftBound, genParams.RightBound, i);
                CalculateFunction(genParams.ObjectiveFunction);
                FindBestSolution();

                reporter?.Report(new Tuple<int, int, int>(1, this.parametrs.Imax, i));
            }
        }


    }
}
