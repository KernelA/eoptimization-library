namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Math.Random;
    using Math;

    /// <summary>
    /// Optimization method BBBC.
    /// </summary>
    public class BBBCOptimizer : IOptimizer<BBBCParams>
    {
        /// <summary>
        /// If initParamsQ = true, then not set parameters.
        /// </summary>
        private bool initParamsQ;
        
        private int indexBestSolution;

        /// <summary>
        /// Dimension of space.
        /// </summary>
        private int dimension;

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

            initParamsQ = true;
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
            double min = points.Min(point => point[point.Dimension - 1]);

            indexBestSolution = points.FindIndex(point => point[point.Dimension - 1] == min);
        }


        private void FindCenterOfMass()
        {
            double denominator = 0.0;

            double temp = 0.0;

            PointND temppoint = new PointND(0, dimension + 1);

            for (int i = 0; i < parametrs.NP; i++)
            {
                temp = 1 / (points[i][dimension] + Constants.Psi);

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

                    // If point leave region that she return to random point.
                    if (tempPoint[j] < a[j])
                        tempPoint[j] = uniformRand.URandVal(a[j], (a[j] + b[j]) / 2);
                    if (tempPoint[j] > b[j])
                        tempPoint[j] = uniformRand.URandVal((a[j] + b[j]) / 2, b[j]);
                }

                points.Add(tempPoint.Clone());
            }

        }

        private void FirstStep(GeneralParams genParams)
        {
            if (initParamsQ)
                throw new InvalidOperationException($"Before you need invoke {nameof(InitializeParameters)}.");

            InitializePopulation(genParams.LeftBound, genParams.RightBound);

            CalculateFunction(genParams.ObjectiveFunction);

            FindBestSolution();
        }

        private void NextStep(GeneralParams genParams, int iter)
        {
            FindCenterOfMass();

            GenerateCurrentPopulation(genParams.LeftBound, genParams.RightBound, iter);

            CalculateFunction(genParams.ObjectiveFunction);

            FindBestSolution();
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.InitializeParameters(T)"/>
        /// </summary>
        /// <param name="Parameters">Parameters for method. Parameters must be type <see cref="BBBCParams"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void InitializeParameters(BBBCParams Parameters)
        {
            if (Parameters == null)
            {
                throw new ArgumentNullException(nameof(Parameters));
            }

            this.parametrs = Parameters;

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

            for (int i = 2; i <= this.parametrs.Imax; i++)
            {
                NextStep(genParams, i);
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

            for (int i = 2; i <= this.parametrs.Imax; i++)
            {
                cancelToken.ThrowIfCancellationRequested();             
                NextStep(genParams, i);
            }
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Progress})"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface <see cref="IProgress{T}"/>, 
        /// where T is <see cref="Progress"/>. 
        /// <seealso cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Progress})"/>
        /// </param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void Optimize(GeneralParams genParams, IProgress<Progress> reporter)
        {
            if (reporter == null)
            {
                throw new ArgumentNullException(nameof(reporter));
            }

            FirstStep(genParams);

            Progress progress = new Progress(this, 1, this.parametrs.Imax, 1);

            reporter.Report(progress);

            for (int i = 2; i <= this.parametrs.Imax; i++)
            {
                NextStep(genParams, i);
                progress.Current = i;
                reporter.Report(progress);
            }
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Progress})"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface <see cref="IProgress{T}"/>, 
        /// where T is <see cref="Progress"/>. 
        /// <seealso cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Progress})"/>
        /// </param>
        /// <param name="cancelToken"><see cref="CancellationToken"/></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void Optimize(GeneralParams genParams, IProgress<Progress> reporter, CancellationToken cancelToken)
        {
            if (reporter == null)
            {
                throw new ArgumentNullException(nameof(reporter));
            }

            FirstStep(genParams);

            Progress progress = new Progress(this, 1, this.parametrs.Imax, 1);

            reporter.Report(progress);

            for (int i = 2; i <= this.parametrs.Imax; i++)
            {
                cancelToken.ThrowIfCancellationRequested();

                NextStep(genParams, i);
                progress.Current = i;
                reporter.Report(progress);
            }
        }


    }
}
