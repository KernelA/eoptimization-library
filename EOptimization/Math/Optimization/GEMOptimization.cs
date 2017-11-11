// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Math.Random;
    using Math;

    using Help;

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

        private bool isInitParams;

        PointND xrnd, xcur, xosd, dosd, dgrs, solution, tempPoint, drnd;

        private GEMParams parametrs;

        // The temporary array has a 'Length' is equal 'dimension'.
        // Used in the calculation the value of the target function.
        private double[] tempArray;


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
        public GEMParams Parameters => parametrs;


        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        public PointND Solution => solution;

        /// <summary>
        /// Create object which uses custom implementation for random generators.
        /// </summary>
        public GEMOptimizer() : this(new ContUniformDistribution(), new NormalDistribution())
        {

        }

        /// <summary>
        /// Create object which uses custom implementation for random generators.
        /// </summary>
        /// <param name="UniformGen">Object, which implements <see cref="IContUniformGenerator"/> interface.</param>
        /// <param name="NormalGen">Object, which implements <see cref="INormalGenerator"/> interface.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="NormalGen"/> or <paramref name="UniformGen"/> is null.</exception>
        public GEMOptimizer(IContUniformGenerator UniformGen, INormalGenerator NormalGen)
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


            isInitParams = false;
        }

        private double EuclideanDistance(PointND Point1, PointND Point2)
        {
            double sum = 0;

            // Last coordinate of point is storing the value of the target function.
            for (int i = 0; i < Point1.Dimension - 1; i++)
            {
                sum += (Point2[i] - Point1[i]) * (Point2[i] - Point1[i]);
            }

            return Math.Sqrt(sum);
        }

        private double EuclideanNorm(PointND Point)
        {
            // Last coordinate of point is storing the value of the target function.
            double sum = 0;

            for(int  i= 0; i < Point.Dimension - 1; i++)
            {
                sum += Point[i] * Point[i];
            }

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Create grenades.
        /// </summary>
        private void InitializePopulation()
        {
            if(grenades == null)
            {
                grenades = new List<PointND>(parametrs.NGrenade);
            }

            if (shrapnels == null)
                InitShrapnels();
            else if (shrapnels.Length != parametrs.NGrenade)
                InitShrapnels();
            

            if (tempArray == null)
                tempArray = new double[dimension];
            else if (tempArray.Length != dimension)
                tempArray = new double[dimension];

            if (tempPoint == null)
                tempPoint = new PointND(0, dimension + 1);
            else if (tempPoint.Dimension != dimension + 1)
                tempPoint = new PointND(0, dimension + 1);

            if (drnd == null)
                drnd = new PointND(0, dimension + 1);
            else if (drnd.Dimension != dimension + 1)
            {
                drnd = new PointND(0, dimension + 1);
            }

            if (dgrs == null)
                dgrs = new PointND(0, dimension + 1);
            else if (dgrs.Dimension != dimension + 1)
            {
                dgrs = new PointND(0, dimension + 1);
            }

            if (solution == null)
                solution = new PointND(0, dimension + 1);
            else if (solution.Dimension != dimension + 1)
            {
                solution = new PointND(0, dimension + 1);
            }

            for (int i = 0; i < parametrs.NGrenade; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    tempPoint[j] = uniformRand.URandVal(-1, 1);
                }

                grenades.Add(tempPoint.DeepCopy());
            }
        }

        private void InitShrapnels()
        {
            shrapnels = new List<PointND>[parametrs.NGrenade];

            for (int i = 0; i < parametrs.NGrenade; i++)
            {
                this.shrapnels[i] = new List<PointND>(parametrs.NShrapnel);
            }
        }

        private void Clear()
        {
            grenades.Clear();

            for (int i = 0; i < parametrs.NGrenade; i++)
            {
                this.shrapnels[i].Clear();
            }
        }

        /// <summary>
        /// Coordinates transformation: [-1; 1] -> [a[i]; b[i]].
        /// </summary>
        /// <param name="X">An input coordinates.</param>
        /// <param name="LowerBound">An array of the lower boundaries.</param>
        /// <param name="UpperBound">An array of the upper boundaries.</param>
        private void TransformCoord(double[] X, double[] LowerBound, double[] UpperBound)
        {
            for (int i = 0; i < LowerBound.Length; i++)
            {
                X[i] = (LowerBound[i] + UpperBound[i] + (UpperBound[i] - LowerBound[i]) * X[i]) / 2;
            }
        }


        /// <summary>
        /// Calculate target function for the grenades.
        /// </summary>
        /// <param name="Function"></param>
        /// <param name="LowerBound"></param>
        /// <param name="UpperBound"></param>
        private void CalculateFunctionForGrenade(Func<double[], double> Function, double[] LowerBound, double[] UpperBound)
        {
            double value = 0;

            for (int i = 0; i < parametrs.NGrenade; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    tempArray[j] = grenades[i][j];

                }

                TransformCoord(tempArray, LowerBound, UpperBound);

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

                grenades[i][dimension] = value;
            }
        }

        /// <summary>
        /// Calculate target function for the shrapnels. Shrapnels from grenade under number <paramref name="WhichGrenade"/>.
        /// </summary>
        /// <param name="Function"></param>
        /// <param name="LowerBound"></param>
        /// <param name="UpperBound"></param>
        /// <param name="WhichGrenade"></param>
        private void CalculateFunctionForShrapnel(Func<double[], double> Function, double[] LowerBound, double[] UpperBound, int WhichGrenade)
        {
            double value = 0;

            for (int i = 0; i < parametrs.NShrapnel; i++)
            {
                if (shrapnels[WhichGrenade][i] != null)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        tempArray[j] = shrapnels[WhichGrenade][i][j];
                    }

                    TransformCoord(tempArray, LowerBound, UpperBound);

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

                    shrapnels[WhichGrenade][i][dimension] = value;
                }
            }
        }

        /// <summary>
        /// Searching OSD and Xosd position.
        /// </summary>
        /// <param name="Function"></param>
        /// <param name="LowerBound"></param>
        /// <param name="UpperBound"></param>
        /// <param name="WhichGrenade"></param>
        /// <param name="NumIter"></param>
        private void FindOSD(Func<double[], double> Function, double[] LowerBound, double[] UpperBound, int WhichGrenade, int NumIter)
        {
            List<PointND> ortogonalArray = new List<PointND>(2 * dimension);

            // Generate 2 * n shrapnels along coordinate axis.
            // 1 in positive direction.
            // 1 in negative direction.
            bool isAddToArray = true;

            for (int i = 0; i < ortogonalArray.Capacity; i++)
            {
                isAddToArray = true;

                tempPoint.SetAt(grenades[WhichGrenade]);

                // The positive direction along the coordinate axis.
                if (i % 2 == 0)
                {
                    tempPoint[i / 2] = uniformRand.URandVal(grenades[WhichGrenade][i / 2], 1);
                }
                // The negative direction along the coordinate axis.
                else
                {
                    tempPoint[i / 2] = uniformRand.URandVal(-1, grenades[WhichGrenade][i / 2]);
                }

                for(int j = 0; j < dimension; j++)
                {
                    tempArray[j] = tempPoint[j];
                }

                TransformCoord(tempArray, LowerBound, UpperBound);

                tempPoint[dimension] = Function(tempArray);

                // If shrapnel and grenade too near, then shrapnel deleted.
                for (int j = 0; j < parametrs.NGrenade; j++)
                {
                    if (j == WhichGrenade)
                        continue;

                    if (EuclideanDistance(tempPoint, grenades[WhichGrenade]) <= radiusGrenade)
                    {
                        isAddToArray = false;
                        break;
                    }
                }

                if (isAddToArray)
                    ortogonalArray.Add(tempPoint.DeepCopy());
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

            // If grenade do not in a neighborhood of the other grenades, then xcur = null.
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
                dosd.MultiplyByInplace(1 / EuclideanNorm(dosd));
            }
        }

        /// <summary>
        /// Determine shrapnels position.
        /// </summary>
        /// <param name="LowerBound"></param>
        /// <param name="UpperBound"></param>
        /// <param name="Function"></param>
        /// <param name="WhichGrenade"></param>
        /// <param name="NumIter"></param>
        private void GenerateShrapneles(Func<double[], double> Function, double[] LowerBound, double[] UpperBound, int WhichGrenade, int NumIter)
        {

            double p = Math.Max(1.0 / dimension,
                 Math.Log10(radiusGrenade / radiusExplosion) / Math.Log10(parametrs.Pts));

            // Determine OSD and Xosd.
            if (NumIter <= 0.1 * parametrs.Imax && WhichGrenade < parametrs.DesiredMin)
            {
                FindOSD(Function, LowerBound, UpperBound, WhichGrenade, NumIter);
            }

            double randomValue1, randomValue2;

            // Generating shrapnels.
            for (int i = 0; i < parametrs.NShrapnel; i++)
            {
                randomValue1 = uniformRand.URandVal(0, 1);

                // Random search direction.
                for (int w = 0; w < drnd.Dimension; w++)
                {
                    drnd[w] = normalRand.NRandVal(0, 1);
                }

                drnd.MultiplyByInplace(1 / EuclideanNorm(drnd));

                // If exist OSD.
                if (dosd != null)
                {
                    randomValue2 = uniformRand.URandVal(0, 1);

                    for(int coordIndex= 0; coordIndex < dgrs.Dimension; coordIndex++)
                    {
                        dgrs[coordIndex] = mosd * randomValue1 * dosd[coordIndex] + (1 - mosd) * randomValue2 * drnd[coordIndex];
                    }
                    
                    dgrs.MultiplyByInplace(1 / EuclideanNorm(dgrs));

                    randomValue1 = Math.Pow(randomValue1, p) * this.radiusExplosion;

                    for (int coordIndex = 0; coordIndex < dgrs.Dimension; coordIndex++)
                    {
                        tempPoint[coordIndex] = grenades[WhichGrenade][coordIndex] + randomValue1 * dgrs[coordIndex];
                    }


                }
                else
                {
                    randomValue1 = Math.Pow(randomValue1, p) * this.radiusExplosion;

                    for (int coordIndex = 0; coordIndex < dgrs.Dimension; coordIndex++)
                    {
                        tempPoint[coordIndex] = grenades[WhichGrenade][coordIndex] + randomValue1 * drnd[coordIndex];
                    }
                }

                shrapnels[WhichGrenade].Add(tempPoint.DeepCopy());

                // Out of range [-1,1]^n.
                for (int j = 0; j < dimension; j++)
                {
                    if (this.shrapnels[WhichGrenade][i][j] < -1 || this.shrapnels[WhichGrenade][i][j] > 1)
                    {
                        double rand_num = uniformRand.URandVal(0,1);

                        double large_comp = shrapnels[WhichGrenade][i].Coordinates.Max(num => Math.Abs(num));

                        for (int coordIndex = 0; coordIndex < dgrs.Dimension; coordIndex++)
                        {
                            tempPoint[coordIndex] = this.shrapnels[WhichGrenade][i][coordIndex] * (1 / large_comp);

                            this.shrapnels[WhichGrenade][i][coordIndex] = rand_num * (tempPoint[coordIndex] - this.grenades[WhichGrenade][coordIndex]) + this.grenades[WhichGrenade][coordIndex];
                        }

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
        /// <param name="NumIter"></param>
        private void UpdateParams(int NumIter)
        {
            radiusGrenade = radiusInitial / Math.Pow(parametrs.RadiusReduct, (double)NumIter / parametrs.Imax);

            double m = this.parametrs.Mmax - (double)NumIter / parametrs.Imax * (this.parametrs.Mmax - this.parametrs.Mmin);

            radiusExplosion = Math.Pow(2 * Math.Sqrt(dimension), m) * Math.Pow(radiusGrenade, 1 - m);

            mosd = Math.Sin(Math.PI / 2 * Math.Pow(Math.Abs(NumIter - 0.1 * parametrs.Imax) / (0.9 * parametrs.Imax), this.parametrs.Psin));
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

            this.shrapnels[WhichGrenade].Clear();
        }


        /// <summary>
        /// Find best solution.
        /// </summary>
        /// <param name="LowerBound"></param>
        /// <param name="UpperBound"></param>
        private void FindBestSolution(double[] LowerBound, double[] UpperBound)
        {
            double f_best = grenades.Min(point => point[dimension]);

            int index_best = grenades.FindIndex(point => point[dimension] == f_best);

            for(int i = 0; i < dimension; i++)
            {
                tempArray[i] = this.grenades[index_best][i];
            }

            TransformCoord(tempArray, LowerBound, UpperBound);

            for(int i = 0; i < dimension; i++)
            {
                solution[i] = tempArray[i];
            }

            solution[dimension] = f_best;

        }

        private void FirstStep(GeneralParams GenParams)
        {

            if (GenParams == null)
            {
                throw new ArgumentNullException(nameof(GenParams));
            }
            if (!isInitParams)
                throw new InvalidOperationException($"Before you need invoke {nameof(InitializeParameters)}.");

            dimension = GenParams.LowerBound.Length;

            this.radiusExplosion = 2 * Math.Sqrt(dimension);

            InitializePopulation();

            CalculateFunctionForGrenade(GenParams.TargetFunction, GenParams.LowerBound, GenParams.UpperBound);
        }


        private void NextStep(GeneralParams genParams, int iter)
        {
            ArrangeGrenades();

            for (int j = 0; j < this.parametrs.NGrenade; j++)
            {
                GenerateShrapneles(genParams.TargetFunction, genParams.LowerBound, genParams.UpperBound, j, iter);

                CalculateFunctionForShrapnel(genParams.TargetFunction, genParams.LowerBound, genParams.UpperBound, j);

                FindBestPosition(j);
            }

            UpdateParams(iter);

            FindBestSolution(genParams.LowerBound, genParams.UpperBound);
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.InitializeParameters(T)"/>
        /// </summary>
        /// <param name="Parameters">Parameters for method.<see cref="GEMParams"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="Parameters"/> is null.</exception>
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

            isInitParams = true;
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

            for (int i = 1; i <= this.parametrs.Imax; i++)
            {
                NextStep(GenParams, i);
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

            for (int i = 1; i <= this.parametrs.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();
                NextStep(GenParams, i);
            }
            Clear();
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// </summary>
        /// <param name="GenParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="Reporter">Object which implements interface IProgress{Progress}.
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

            Progress progress = new Progress(this, 1, this.parametrs.Imax, 1);

            Reporter.Report(progress);

            for (int i = 1; i <= this.parametrs.Imax; i++)
            {
                NextStep(GenParams, i);
                progress.Current = i;
                Reporter.Report(progress);
            }

            Clear();
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// </summary>
        /// <param name="GenParams">General parameters. <see cref="GeneralParams"/>.</param>
        /// <param name="Reporter">Object which implements interface IProgress{Progress}. 
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

            Progress progress = new Progress(this, 1, this.parametrs.Imax, 1);

            Reporter.Report(progress);

            for (int i = 1; i <= this.parametrs.Imax; i++)
            {
                CancelToken.ThrowIfCancellationRequested();

                NextStep(GenParams, i);
                progress.Current = i;
                Reporter.Report(progress);
            }

            Clear();
        }
    }
}
