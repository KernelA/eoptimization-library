namespace EOpt.Math.Optimization.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;


    static class GeneralOptimizerTests
    {
        public static readonly double[] LeftBound = { -10, -10 };

        public static readonly double[] RightBound = { 10, 10 };

        public const int IterMax = 10;

        public static double TargetFunction(double[] point)
        {
            return point.Sum(coord => coord * coord);
        }

        public static double FunctionNaN(double[] point)
        {
            return Math.Sqrt(-1);
        }

        public static double FunctionPosInf(double[] point)
        {
            return Double.PositiveInfinity;
        }

        public static double FunctionNegInf(double[] points)
        {
            return Double.NegativeInfinity;
        }


        public static bool TestInavlidFunction<T>(IOptimizer<T> opt, T parameters, Func<double[], double> function)
        {
            bool error = true;

            opt.InitializeParameters(parameters);

            try
            {
                opt.Minimize(new GeneralParams(function, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));
            }
            catch (ArithmeticException exc)
            {
                error = false;
            }

            return error;
        }

        public static bool TestWrongInvoke<T>(IOptimizer<T> opt)
        {
            bool error = true;

            try
            {
                opt.Minimize(new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));
            }
            catch (InvalidOperationException exc)
            {
                error = false;
                //Debug.Indent();
                //Debug.WriteLine(exc.Message);
            }

            return error;
        }

        public static bool TestWrongParams<T>(IOptimizer<T> opt) where T : class
        {
            bool error = true;

            try
            {
                opt.InitializeParameters(null);
            }
            catch (ArgumentNullException exc)
            {
                error = false;
                //Debug.Indent();
                //Debug.WriteLine(exc.Message);
            }

            return error;
        }

        public static bool TestOptimizer<T>(IOptimizer<T> Opt, T Parameters, GeneralParams GenParams)
        {
            Opt.InitializeParameters(Parameters);
                      
            Opt.Minimize(GenParams);

            return Opt.Solution == null && Opt.Solution.Dimension != 3;
        }

        public static bool TestCancel<T>(IOptimizer<T> Opt, T Parameters, GeneralParams GenParams)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            CancellationToken token = tokenSource.Token;

            Opt.InitializeParameters(Parameters);

            Task task = Task.Factory.StartNew(() => { Thread.Sleep(3000); Opt.Minimize(GenParams, token); }, token);

            tokenSource.Cancel();

            try
            {
                task.Wait();
            }
            catch(AggregateException)
            {
                
            }

            return !task.IsCanceled;
        }

    }
}
