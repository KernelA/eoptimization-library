namespace EOpt.Math.Optimization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class GeneralOptimizerTests
    {
        public const int ITER_MAX = 10;
        public static readonly double[] LowerBounds = { -10, -10 };

        public static readonly double[] UpperBounds = { 10, 10 };

        public static double FunctionNaN(IReadOnlyList<double> point) => Math.Sqrt(-1);

        public static double FunctionNegInf(IReadOnlyList<double> points) => Double.NegativeInfinity;

        public static double FunctionPosInf(IReadOnlyList<double> point) => Double.PositiveInfinity;

        public static double TargetFunction(IReadOnlyList<double> point) => point.Sum(coord => coord * coord);

        public static bool TestCancel<T>(IOOOptimizer<T> Opt, T Parameters)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            CancellationToken token = tokenSource.Token;

            OOOptimizationProblem problem = new OOOptimizationProblem(TargetFunction, LowerBounds, UpperBounds);

            Task task = Task.Factory.StartNew(() => { Thread.Sleep(3000); Opt.Minimize(Parameters, problem, token); }, token);

            tokenSource.Cancel();

            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
            }

            return !task.IsCanceled;
        }

        public static void TestInavlidFunction<T>(IOOOptimizer<T> opt, T parameters, InvalidFunc TypeFunc)
        {
            OOOptimizationProblem problem = null;

            switch (TypeFunc)
            {
                case InvalidFunc.NaNFunc:
                    {
                        problem = new OOOptimizationProblem(FunctionNaN, LowerBounds, UpperBounds);
                        break;
                    }
                case InvalidFunc.NegInfFunc:
                    {
                        problem = new OOOptimizationProblem(FunctionNegInf, LowerBounds, UpperBounds);
                        break;
                    }
                case InvalidFunc.PosInfFunc:
                    {
                        problem = new OOOptimizationProblem(FunctionPosInf, LowerBounds, UpperBounds);
                        break;
                    }
            }

            opt.Minimize(parameters, problem);
        }

        public static bool TestOptimizer<T>(IOOOptimizer<T> Opt, T Parameters)
        {
            Opt.Minimize(Parameters, new OOOptimizationProblem(TargetFunction, LowerBounds, UpperBounds));

            return Opt.Solution == null;
        }

        public static void TestWrongParams<T>(IOOOptimizer<T> opt) where T : struct
        {
            T parameters = default(T);

            opt.Minimize(parameters, new OOOptimizationProblem(TargetFunction, LowerBounds, UpperBounds));
        }
    }

    public enum InvalidFunc { NaNFunc, PosInfFunc, NegInfFunc };
}