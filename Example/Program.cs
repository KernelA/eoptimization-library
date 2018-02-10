using System;
using System.Collections.Generic;

using EOpt.Math.Optimization;
using EOpt.Math.Optimization.OOOpt;

namespace Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /// See http://www.sfu.ca/~ssurjano/rastr.html
            double RastriginFunction(IReadOnlyList<double> x)
            {
                return 20 + (x[0] * x[0] - 10 * Math.Cos(2 * Math.PI * x[0])) +
               (x[1] * x[1] - 10 * Math.Cos(2 * Math.PI * x[1]));
            }

            IOOOptimizer<BBBCParams> bbbc = new BBBCOptimizer();
            IOOOptimizer<FWParams> fw = new FWOptimizer();
            IOOOptimizer<GEMParams> gem = new GEMOptimizer();

            BBBCParams param1 = new BBBCParams(20, 100, 0.4, 0.5);
            FWParams param2 = new FWParams(20, 100, 20);
            GEMParams param3 = new GEMParams(1, 100, 50, 2 * Math.Sqrt(2), 100);

            double[] constr1 = { -5.12, -5.12 };
            double[] constr2 = { 5.12, 5.12 };

            OOOptimizationProblem param = new OOOptimizationProblem(RastriginFunction, constr1, constr2);

            Console.WriteLine("Exact solution: f(x) = 0, x = (0, 0).");
            Console.WriteLine();

            Test(bbbc, param1, param, "BBBC");
            Test(fw, param2, param, "Fireworks");
            Test(gem, param3, param, "GEM");

            Console.WriteLine("Complete");
            Console.ReadKey();
        }

        private static void Test<T>(IOOOptimizer<T> Opt, T Parameters, OOOptimizationProblem Problem, string Method)
        {
            Agent bestSolution = null;

            Opt.Minimize(Parameters, Problem);

            bestSolution = Opt.Solution;

            for (int i = 1; i < 10; i++)
            {
                Opt.Minimize(Parameters, Problem);

                if (Opt.Solution.Objs[0] < bestSolution.Objs[0])
                {
                    bestSolution = Opt.Solution;
                }
            }

            Console.WriteLine($"Method: {Method}.");
            Console.WriteLine($"Solution: f(x) = {bestSolution.Objs[0]}, x = ({bestSolution.Point[0]}, {bestSolution.Point[1]}).");
            Console.WriteLine();
        }
    }
}