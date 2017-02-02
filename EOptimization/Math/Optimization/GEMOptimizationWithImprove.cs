namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Math.Random;

    /// <summary>
    /// Optimization method GEM. 
    /// </summary>
    public class GEMOptimizer : IOptimizer
    {
        const double eps = 1E-10;

        private double radius_grenade,
            radius_length,
            radius_initial,
            mosd;
        private int dimension;

        private IContUniformGenerator uniformRand;

        private INormalGenerator normalRand;

        private bool initParams;

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
        /// 
        /// </summary>
        /// <param name="uniformGen">Object, which implements <see cref="IContUniformGenerator"/> interface, 
        /// for generating uniform random value. If it equal null then use the default implementation.</param>
        /// <param name="normalGen">Object, which implements <see cref="INormalGenerator"/> interface, 
        /// for generating uniform random value. If it equal null then use the default implementation.</param>
        public GEMOptimizer(IContUniformGenerator uniformGen = null, INormalGenerator normalGen = null)
        {
            if (uniformGen == null)
                uniformRand = new ContUniformDistribution();

            if (normalGen == null)
                normalRand = new NormalDistribution();

            xrnd = xosd = xcur = dosd = dgrs = null;

            initParams = false;
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
                    temp[i] = uniformRand.URandVal(-1, 1);
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

            for (int i = 0; i < ortogonalArray.Count; i++)
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

                    if (EuclideanDistance(temp, grenades[WhichGrenade]) <= radius_grenade)
                    {
                        addToArray = false;
                        break;
                    }
                }

                if (addToArray)
                    ortogonalArray.Add(temp.Clone());
            }

            double min =  0;

            int indexmin = -1;

            if(ortogonalArray.Count != 0)
            {
                min = ortogonalArray[0][dimension];
                indexmin = 0;
            }

            for (int i = 1; i < ortogonalArray.Count; i++)
            {
                if(ortogonalArray[i][dimension] < min)
                {
                    min = ortogonalArray[i][dimension];
                    indexmin = i;
                }
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

                if (EuclideanDistance(grenades[j], grenades[WhichGrenade]) <= radius_grenade)
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
                 Math.Log10(radius_grenade / radius_length) / Math.Log10(parametrs.Pts));

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
                    r1 = Math.Pow(r1, p) * this.radius_length;
                    temp = grenades[WhichGrenade] + r1 * dgrs;

                }
                else
                {
                    r1 = Math.Pow(r1, p) * this.radius_length;
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

                    if (s <= radius_grenade)
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
            radius_grenade = radius_initial / Math.Pow(parametrs.RadiusReduct, (double)iter / parametrs.Imax);

            double m = this.parametrs.Mmax - (double)iter / parametrs.Imax * (this.parametrs.Mmax - this.parametrs.Mmin);

            radius_length = Math.Pow(2 * Math.Sqrt(dimension), m) * Math.Pow(radius_grenade, 1 - m);

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
            double f_best = grenades[0][dimension];

            int index_best = 0;

            for (int i = 1; i < parametrs.NGrenade; i++)
            {
                if (grenades[i][dimension] < f_best)
                {
                    f_best = grenades[i][dimension];
                    index_best = i;
                }
            }

            double[] x = this.grenades[index_best].Coordinates.ToArray();

            TransformCoord(x, a, b);

            this.solution = new PointND(x);

        }


        /// <summary>
        /// <see cref="IOptimizer.InitializeParameters(object)"/>
        /// </summary>
        /// <param name="parameters">Parameters for method. Must be type <see cref="GEMParams"/>.</param>
        public void InitializeParameters(object parametrs)
        {
            this.parametrs = parametrs as GEMParams;

            if (this.parametrs == null)
                throw new ArgumentException($"{nameof(parametrs)} must be type as {nameof(GEMParams)}.", nameof(parametrs));


            this.radius_length = 2 * Math.Sqrt(dimension);
            this.radius_grenade = this.parametrs.InitRadiusGrenade;
            this.radius_initial = this.parametrs.InitRadiusGrenade;

            mosd = 0;

            initParams = true;
        }

        /// <summary>
        /// <see cref="IOptimizer.Optimize(GeneralParams)"/>
        /// </summary>
        /// <param name="genParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface <see cref="IProgress{Tuple{int,int,int}}"/>,
        /// where first item in tuple is the initial value, second item is the end value, 
        /// third item is the current progress value. 
        /// <seealso cref="IOptimizer.Optimize(GeneralParams, IProgress{Tuple{int, int, int}})"/>. </param>
        public void Optimize(GeneralParams genParams, IProgress<Tuple<int,int,int>> reporter = null)
        {
            if (!initParams)
                throw new InvalidOperationException($"Before you need invoke {nameof(InitializeParameters)}.");

            dimension = genParams.LeftBound.Length;

            reporter?.Report(new Tuple<int, int, int>(0, this.parametrs.Imax, 0));

            InitializePopulation();

            CalculateFunctionForGrenade(genParams.ObjectiveFunction, genParams.LeftBound, genParams.RightBound);

            for (int i = 1; i <= this.parametrs.Imax; i++)
            {
                ArrangeGrenades();

                for (int j = 0; j < this.parametrs.NGrenade; j++)
                {
                    GenerateShrapneles(genParams.ObjectiveFunction, genParams.LeftBound, genParams.RightBound, j, i);

                    CalculateFunctionForShrapnel(genParams.ObjectiveFunction, genParams.LeftBound, genParams.RightBound, j);

                    FindBestPosition(j);
                }

                UpdateParams(i);

                FindBestSolution(genParams.LeftBound, genParams.RightBound);

                reporter?.Report(new Tuple<int, int, int>(0, this.parametrs.Imax, i));
            }
        }

    }
}
