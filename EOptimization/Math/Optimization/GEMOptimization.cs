namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Math.Random;
    using Math;

    /// <summary>
    /// Optimization method GEM. 
    /// </summary>
    public class GEMOptimizer : IOptimizer<GEMParams>
    {
        private double radiusGrenade,
            radiusExplosion,
            radiusInitial,
            mosd;

        private int dimension;

        private IContUniformGenerator uniformRand;

        private INormalGenerator normalRand;

        private bool initParamsQ;

        PointND xrnd, xcur, xosd, dosd, dgrs;

        PointND solution;

        private GEMParams parametrs;


        /// <summary>
        /// Grenades.
        /// </summary>
        private List<PointND> grenades;

        /// <summary>
        /// Shrapnels.
        /// </summary>
        private List<PointND>[] shrapnels;


        /// <summary>
        /// Parameters for method. <see cref="GEMParams"/>.
        /// </summary>
        public GEMParams Parameters
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
                return solution;
            }
        }

        /// <summary>
        /// Create object which use default implementation for random generators.
        /// </summary>
        public GEMOptimizer() : this(new ContUniformDistribution(), new NormalDistribution())
        {

        }

        /// <summary>
        /// Create object which use custom implementation for random generators.
        /// </summary>
        /// <param name="uniformGen">Object, which implements <see cref="IContUniformGenerator"/> interface, 
        /// for generating uniform random value.</param>
        /// <param name="normalGen">Object, which implements <see cref="INormalGenerator"/> interface, 
        /// for generating uniform random value.</param>
        public GEMOptimizer(IContUniformGenerator uniformGen, INormalGenerator normalGen)
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

            xrnd = xosd = xcur = dosd = dgrs = null;

            initParamsQ = false;
        }




        private double EuclideanDistance(PointND a, PointND b)
        {
            double sum = 0;

            // Last coordinate of point for storing the value of target function in this point.
            for (int i = 0; i < a.Dimension - 1; i++)
            {
                sum += (b[i] - a[i]) * (b[i] - a[i]);
            }

            return Math.Sqrt(sum);
        }


        private double EuclideanNorm(PointND p)
        {
            // Last coordinate of point for storing the value of target function in this point.
            double sum = p.Coordinates.Take(dimension).Sum(a => a * a);

            return Math.Sqrt(sum);
        }


        /// <summary>
        /// Create grenades.
        /// </summary>
        private void InitializePopulation()
        {
            grenades = new List<PointND>(parametrs.NGrenade);

            shrapnels = new List<PointND>[parametrs.NGrenade];


            for (int i = 0; i < parametrs.NGrenade; i++)
            {
                this.shrapnels[i] = new List<PointND>(parametrs.NShrapnel);
            }

            PointND temp = new PointND(0, dimension + 1);

            for (int i = 0; i < parametrs.NGrenade; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    temp[j] = uniformRand.URandVal(-1, 1);
                }

                grenades.Add(temp.Clone());
            }
        }

        /// <summary>
        /// Coordinates transformation  [-1; 1] -> [a[i]; b[i]].
        /// </summary>
        /// <param name="x">Input coordinates.</param>
        /// <param name="a">Array of left boundaries.</param>
        /// <param name="b">Array of right boundaries.</param>
        private void TransformCoord(double[] x, double[] a, double[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                x[i] = (a[i] + b[i] + (b[i] - a[i]) * x[i]) / 2;
            }
        }


        /// <summary>
        /// Calculate target function for grenades.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void CalculateFunctionForGrenade(Func<double[], double> function, double[] a, double[] b)
        {
            double[] x = new double[dimension];

            for (int i = 0; i < parametrs.NGrenade; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    x[i] = grenades[i][j];

                }

                TransformCoord(x, a, b);

                grenades[i][dimension] = function(x);
            }
        }

        /// <summary>
        /// Calculate target function for shrapnels. Shrapnels from grenade under number <paramref name="WhichGrenade"/>.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="WhichGrenade"></param>
        private void CalculateFunctionForShrapnel(Func<double[], double> function, double[] a, double[] b, int WhichGrenade)
        {
            double[] x = new double[dimension];

            for (int i = 0; i < parametrs.NShrapnel; i++)
            {
                if (shrapnels[WhichGrenade][i] != null)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        x[j] = shrapnels[WhichGrenade][i][j];
                    }

                    TransformCoord(x, a, b);

                    shrapnels[WhichGrenade][i][dimension] = function(x);
                }
            }
        }

        /// <summary>
        /// Searching OSD and Xosd position.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="WhichGrenade"></param>
        /// <param name="NumberIter"></param>
        private void FindOSD(Func<double[], double> function, double[] a, double[] b, int WhichGrenade, int NumberIter)
        {
            PointND temp = new PointND(0, dimension + 1);

            // Generate 2 * n shrapnels along coordinate axis.
            // 1 in positive direction.
            // 1 in negative direction.
            List<PointND> ortogonalArray = new List<PointND>(2 * dimension);

            bool addToArray = true;

            for (int i = 0; i < ortogonalArray.Capacity; i++)
            {
                addToArray = true;

                temp = grenades[WhichGrenade].Clone();

                // Positive direction along coordinate axis.
                if (i % 2 == 0)
                {
                    temp[i / 2] = uniformRand.URandVal(grenades[WhichGrenade][i / 2], 1);
                }
                // Negative direction along coordinate axis.
                else
                {
                    temp[i / 2] = uniformRand.URandVal(-1, grenades[WhichGrenade][i / 2]);
                }

                double[] x = temp.Coordinates.Take(dimension).ToArray();

                TransformCoord(x, a, b);

                temp[dimension] = function(x);

                // If shrapnel and grenade too near, then shrapnel deleted.
                for (int j = 0; j < parametrs.NGrenade; j++)
                {
                    if (j == WhichGrenade)
                        continue;

                    if (EuclideanDistance(temp, grenades[WhichGrenade]) <= radiusGrenade)
                    {
                        addToArray = false;
                        break;
                    }
                }

                if (addToArray)
                    ortogonalArray.Add(temp.Clone());
            }

            double min = 0; 

            int indexmin = -1; 

            if(ortogonalArray.Count != 0)
            {
                min = ortogonalArray.Min(point => point[dimension]); 
                indexmin = ortogonalArray.FindIndex(point => point[dimension] == min);
            }


            // Determine position Xosd.
            xosd = indexmin == -1 ? null : ortogonalArray[indexmin];

            // Determine position Xcur.
            xcur = grenades[WhichGrenade];

            // If grenade do not in a neighborhood of other grenades, then xcur = null.
            for (int j = 0; j < parametrs.NGrenade; j++)
            {
                if (j == WhichGrenade)
                    continue;

                if (EuclideanDistance(grenades[j], grenades[WhichGrenade]) <= radiusGrenade)
                {
                    xcur = null;
                    break;
                }
            }

            // Vector dosd.
            dosd = indexmin == -1 ?
                null : ortogonalArray[indexmin] - grenades[WhichGrenade];
            
            // Normalization vector.
            if (dosd != null)
            {
                dosd *= (1 / EuclideanNorm(dosd));
            }
        }

        /// <summary>
        /// Determine shrapnels position.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="function"></param>
        /// <param name="WhichGrenade"></param>
        /// <param name="NumberIter"></param>
        private void GenerateShrapneles(Func<double[], double> function, double[] a, double[] b, int WhichGrenade, int NumberIter)
        {

            double p = Math.Max(1.0 / dimension,
                 Math.Log10(radiusGrenade / radiusExplosion) / Math.Log10(parametrs.Pts));

            PointND temp = new PointND(0, dimension + 1);

            PointND drnd = new PointND(0, dimension + 1);

            // Determine OSD and Xosd.
            if (NumberIter <= 0.1 * parametrs.Imax && WhichGrenade < parametrs.DesiredMin)
            {
                FindOSD(function, a, b, WhichGrenade, NumberIter);
            }

            double r1, r2;

            // Generating shrapnels.
            for (int i = 0; i < parametrs.NShrapnel; i++)
            {
                r1 = uniformRand.URandVal(0, 1);

                // Random search direction.
                for (int w = 0; w < drnd.Dimension; w++)
                {
                    drnd[w] = normalRand.NRandVal(0, 1);
                }

                drnd *= (1 / EuclideanNorm(drnd));

                // If exist OSD.
                if (dosd != null)
                {
                    r2 = uniformRand.URandVal(0, 1);
                    dgrs = mosd * r1 * dosd + (1 - mosd) * r2 * drnd;
                    dgrs *= (1 / EuclideanNorm(dgrs));
                    r1 = Math.Pow(r1, p) * this.radiusExplosion;
                    temp = grenades[WhichGrenade] + r1 * dgrs;

                }
                else
                {
                    r1 = Math.Pow(r1, p) * this.radiusExplosion;
                    temp = grenades[WhichGrenade] + r1 * drnd;
                }

                shrapnels[WhichGrenade].Add(temp);

                // Out of range [-1,1]^n.
                for (int j = 0; j < dimension; j++)
                {
                    if (this.shrapnels[WhichGrenade][i][j] < -1 || this.shrapnels[WhichGrenade][i][j] > 1)
                    {
                        double rand_num = uniformRand.URandVal(0,1);

                        double large_comp = shrapnels[WhichGrenade][i].Coordinates.Max(num => Math.Abs(num));

                        temp = this.shrapnels[WhichGrenade][i] * (1 / large_comp);

                        this.shrapnels[WhichGrenade][i] = rand_num * (temp - this.grenades[WhichGrenade]) + this.grenades[WhichGrenade];

                        break;
                    }
                }

                // Calculate distance to grenades.
                for (int l = 0; l < parametrs.NGrenade; l++)
                {
                    if (l == WhichGrenade)
                        continue;

                    double s = EuclideanDistance(shrapnels[WhichGrenade][i], grenades[l]);

                    if (s <= radiusGrenade)
                    {
                        // Shrapnel do not accept (she in  a neighborhood other grenades).
                        this.shrapnels[WhichGrenade][i] = null;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Update parameters.
        /// </summary>
        /// <param name="iter">Current iteration.</param>
        private void UpdateParams(int iter)
        {
            radiusGrenade = radiusInitial / Math.Pow(parametrs.RadiusReduct, (double)iter / parametrs.Imax);

            double m = this.parametrs.Mmax - (double)iter / parametrs.Imax * (this.parametrs.Mmax - this.parametrs.Mmin);

            radiusExplosion = Math.Pow(2 * Math.Sqrt(dimension), m) * Math.Pow(radiusGrenade, 1 - m);

            mosd = Math.Sin(Math.PI / 2 * Math.Pow(Math.Abs((iter - 0.1 * parametrs.Imax)) / (0.9 * parametrs.Imax), this.parametrs.Psin));
        }

        /// <summary>
        /// Sort by ascending.
        /// </summary>
        private void ArrangeGrenades()
        {
            grenades.Sort((x, y) => x[dimension].CompareTo(y[dimension]));
        }


        /// <summary>
        /// Search best position to grenade.
        /// </summary>
        /// <param name="WhichGrenade"></param>
        private void FindBestPosition(int WhichGrenade)
        {
            int index_best = -1;

            double f_best = 0;

            // Find shrapnel with minimum value of target function.
            for (int i = 0; i < parametrs.NShrapnel; i++)
            {
                if (this.shrapnels[WhichGrenade][i] != null)
                {
                    index_best = i;
                    f_best = this.shrapnels[WhichGrenade][i][dimension];
                    break;
                }
            }

            for (int i = index_best + 1; i < parametrs.NShrapnel; i++)
            {
                if (this.shrapnels[WhichGrenade][i] != null)
                {
                    if (this.shrapnels[WhichGrenade][i][dimension] < f_best)
                    {
                        index_best = i;
                        f_best = this.shrapnels[WhichGrenade][i][dimension];
                    }

                }
            }

            xrnd = index_best == -1 ? null : shrapnels[WhichGrenade][index_best];



            List<PointND> points = new List<PointND>(3);

            if (xcur != null)
                points.Add(xcur);

            if (xrnd != null)
                points.Add(xrnd);

            if (xosd != null)
                points.Add(xosd);

            double min = 0;

            int indexMin = -1;

            if(points.Count != 0)
            {
                // Find best position with minimum value of target function among: xcur, xrnd, xosd.
                min = points.Min(a => a[dimension]);
                indexMin = points.FindIndex(a => a[dimension] == min);
            }

            // If exist best position then move grenade.
            if (indexMin != -1)
            {
                grenades[WhichGrenade] = points[indexMin];
            }

            xosd = null;
            xrnd = null;
            xcur = null;
            dosd = null;
            dgrs = null;

            this.shrapnels[WhichGrenade].Clear();
        }


        /// <summary>
        /// Find best solution.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void FindBestSolution(double[] a, double[] b)
        {
            double f_best = grenades.Min(point => point[dimension]);

            int index_best = grenades.FindIndex(point => point[dimension] == f_best);

            double[] x = this.grenades[index_best].Coordinates.ToArray();

            TransformCoord(x, a, b);

            this.solution = new PointND(x);

        }

        private void FirstStep(GeneralParams genParams)
        {
            if (!initParamsQ)
                throw new InvalidOperationException($"Before you need invoke {nameof(InitializeParameters)}.");

            dimension = genParams.LeftBound.Length;

            this.radiusExplosion = 2 * Math.Sqrt(dimension);

            InitializePopulation();

            CalculateFunctionForGrenade(genParams.ObjectiveFunction, genParams.LeftBound, genParams.RightBound);
        }


        private void NextStep(GeneralParams genParams, int iter)
        {
            ArrangeGrenades();

            for (int j = 0; j < this.parametrs.NGrenade; j++)
            {
                GenerateShrapneles(genParams.ObjectiveFunction, genParams.LeftBound, genParams.RightBound, j, iter);

                CalculateFunctionForShrapnel(genParams.ObjectiveFunction, genParams.LeftBound, genParams.RightBound, j);

                FindBestPosition(j);
            }

            UpdateParams(iter);

            FindBestSolution(genParams.LeftBound, genParams.RightBound);
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.InitializeParameters(T)"/>
        /// </summary>
        /// <param name="Parameters"></param>
        public void InitializeParameters(GEMParams Parameters)
        {

            if (Parameters == null)
            {
                throw new ArgumentNullException(nameof(Parameters));
            }

            this.parametrs = Parameters;
            this.radiusGrenade = this.parametrs.InitRadiusGrenade;
            this.radiusInitial = this.parametrs.InitRadiusGrenade;

            mosd = 0;

            initParamsQ = true;
        }


        /// <summary>
        /// <see cref="IOptimizer{T}.Optimize(GeneralParams)"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Optimize(GeneralParams genParams)
        {
            FirstStep(genParams);

            for (int i = 1; i <= this.parametrs.Imax; i++)
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

            for (int i = 1; i <= this.parametrs.Imax; i++)
            {
                cancelToken.ThrowIfCancellationRequested();
                NextStep(genParams, i);
            }
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Progress})"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface IProgress{Progress}.
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

            for (int i = 1; i <= this.parametrs.Imax; i++)
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
        /// <param name="reporter">Object which implement interface IProgress{Progress}. 
        /// <seealso cref="IOptimizer{T}.Optimize(GeneralParams, IProgress{Progress})"/>
        /// <param name="cancelToken"><see cref="CancellationToken"/></param>
        /// </param>
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

            for (int i = 1; i <= this.parametrs.Imax; i++)
            {
                cancelToken.ThrowIfCancellationRequested();

                NextStep(genParams, i);
                progress.Current = i;
                reporter.Report(progress);
            }
        }
    }
}
