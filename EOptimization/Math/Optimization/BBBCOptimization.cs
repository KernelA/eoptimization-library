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
    /// Optimization method BBBC.
    /// </summary>
    public class BBBCOptimizer : IOptimizer<BBBCParams>
    {
        /// <summary>
        /// If isInitiParams = true, then parameters did not set.
        /// </summary>
        private bool isInitiParams;

        // The temporary array has a 'Length' is equal 'dimension'.
        // Used in the calculation the value of the target function.
        private double[] tempArray;
        
        /// <summary>
        /// Dimension of space.
        /// </summary>
        private int dimension;

        private PointND centerOfMass, solution;

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
        public BBBCParams Parameters => parametrs;

        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        public PointND Solution => solution;


        /// <summary>
        /// Create the object which uses default implementation for random generators.
        /// </summary>
        public BBBCOptimizer() : this(new ContUniformDistribution(), new NormalDistribution())
        {

        }

        /// <summary>
        /// Create the object which uses custom implementation for random generators.
        /// </summary>
        /// <param name="UniformGen">Object, which implements <see cref="IContUniformGenerator"/> interface.</param>
        /// <param name="NormalGen">Object, which implements <see cref="INormalGenerator"/> interface.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="NormalGen"/> or <paramref name="UniformGen"/> is null.</exception>
        public BBBCOptimizer(IContUniformGenerator UniformGen, INormalGenerator NormalGen)
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

            isInitiParams = true;
        }


 
        private void InitializePopulation(double[] LowerBound, double[] UpperBounds)
        {
            dimension = LowerBound.Length;

            if (tempArray == null)
                tempArray = new double[dimension];
            else if(tempArray.Length != dimension)
                tempArray = new double[dimension];

            if (centerOfMass == null)
                centerOfMass = new PointND(0, dimension + 1);
            else if(centerOfMass.Dimension != dimension + 1)
                centerOfMass = new PointND(0, dimension + 1);

            if(points == null)
            {
                points = new List<PointND>(parametrs.NP);
            }

            
            // Last coordinate of point for storing the value of the target function in this point.
            PointND point =  new PointND(0, dimension + 1);

            for (int i = 0; i < parametrs.NP; i++)
            {
                for (int j  = 0; j < dimension; j++)
                {
                    point[j] = uniformRand.URandVal(LowerBound[j], UpperBounds[j]);
                }

                points.Add(point.DeepCopy());
            }           
        }


        private void CalculateFunction(Func<double[], double> Func)
        {
            // Last coordinate of the point is storing the value of the target function.
            double value = 0;

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    tempArray[j] = points[i][j];
                }

                value = Func(tempArray);

                try
                {
                    CheckDouble.CheckInvalidValue(value);
                }
                catch(ArithmeticException exc)
                {
                    throw new InvalidValueFunctionException($"Function has an invalid value at point.\n{exc.Message}", new PointND(tempArray),
                        value);
                }
                
                points[i][dimension] = value;
            }
        }


        private void FindBestSolution()
        {
            double min = points[0][points[0].Dimension - 1];
            solution = points[0];

            for(int i = 1; i < points.Count; i++)
            {
                if(points[i][points[i].Dimension - 1] < min)
                {
                    min = points[i][points[i].Dimension - 1];
                    solution = points[i];
                }
            }
        }


        private void FindCenterOfMass()
        {
            double denominator = 0.0;

            double temp = 0.0;

            centerOfMass.MultiplyByInplace(0);

            for (int i = 0; i < parametrs.NP; i++)
            {
                temp = 1 / (points[i][dimension] + Constants.ValueForAvoidDivByZero);

                denominator += temp;

                for(int coordNum = 0; coordNum < centerOfMass.Dimension; coordNum++)
                {
                    centerOfMass[coordNum] += temp * points[i][coordNum];
                }
            }

            centerOfMass.MultiplyByInplace(1 / denominator);
        }


        private void GenerateCurrentPopulation(double[] LowerBound, double[] UpperBound, int IterNum)
        {

            int excludeIndex = points.FindIndex(p => p == Solution);

            for( int i = 0 ; i < parametrs.NP; i++)
            {
                if (i == excludeIndex)
                    continue;

                for (int j = 0; j < dimension; j++)
                {
                    points[i][j] = parametrs.Beta * centerOfMass[j] + (1 - parametrs.Beta) * Solution[j] + normalRand.NRandVal(0,1)
                        * parametrs.Alpha * (UpperBound[j] - LowerBound[j]) / IterNum;

                    // If point leaves region then it returns to random point.
                    if (points[i][j] < LowerBound[j])
                        points[i][j] = uniformRand.URandVal(LowerBound[j], (LowerBound[j] + UpperBound[j]) / 2);
                    if (points[i][j] > UpperBound[j])
                        points[i][j] = uniformRand.URandVal((LowerBound[j] + UpperBound[j]) / 2, UpperBound[j]);
                }
            }

        }


        private void Clear()
        {
            points.Clear();
        }

        private void FirstStep(GeneralParams GenParams)
        {

            if (GenParams == null)
            {
                throw new ArgumentNullException(nameof(GenParams));
            }

            if (isInitiParams)
                throw new InvalidOperationException($"Before you need invoke {nameof(InitializeParameters)}.");

            InitializePopulation(GenParams.LowerBound, GenParams.UpperBound);

            CalculateFunction(GenParams.TargetFunction);

            FindBestSolution();
        }

        private void NextStep(GeneralParams GenParams, int Iter)
        {
            FindCenterOfMass();

            GenerateCurrentPopulation(GenParams.LowerBound, GenParams.UpperBound, Iter);

            CalculateFunction(GenParams.TargetFunction);

            FindBestSolution();
        }

        /// <summary>
        /// <see cref="IOptimizer{T}.InitializeParameters(T)"/>
        /// </summary>
        /// <param name="Parameters">Parameters for method. Parameters must be type <see cref="BBBCParams"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="Parameters"/> is null.</exception>
        public void InitializeParameters(BBBCParams Parameters)
        {
            if (Parameters == null)
            {
                throw new ArgumentNullException(nameof(Parameters));
            }

            this.parametrs = Parameters;

            isInitiParams = false;
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

            for (int i = 2; i <= this.parametrs.Imax; i++)
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

            for (int i = 2; i <= this.parametrs.Imax; i++)
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

            Progress progress = new Progress(this, 1, this.parametrs.Imax, 1);

            Reporter.Report(progress);

            for (int i = 2; i <= this.parametrs.Imax; i++)
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
        /// <param name="Reporter">Object which implement interface <see cref="IProgress{T}"/>, 
        /// where T is <see cref="Progress"/>. 
        /// <seealso cref="IOptimizer{T}.Minimize(GeneralParams, IProgress{Progress})"/>
        /// </param>
        /// <param name="CancelToken"><see cref="CancellationToken"/></param>
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

            for (int i = 2; i <= this.parametrs.Imax; i++)
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
