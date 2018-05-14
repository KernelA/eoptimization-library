using System;
using System.Collections.Generic;

using EOpt.Math.Optimization;
using EOpt.Math.Optimization.OOOpt;

namespace Example
{
    /// See http://www.sfu.ca/~ssurjano/rastr.html
    internal class RastriginProblem : IOOOptProblem
    {
        private double[] _lowerBounds, _upperBounds;


        public IReadOnlyList<double> LowerBounds => _lowerBounds;

        public IReadOnlyList<double> UpperBounds => _upperBounds;

        public RastriginProblem()
        {
            _lowerBounds = new double[2] { -5.12, -5.12 };
            _upperBounds = new double[2] {5.12, 5.12 };
        }

        public double TargetFunction(IReadOnlyList<double> Point)
        {
            return 20 + (Point[0] * Point[0] - 10 * Math.Cos(2 * Math.PI * Point[0])) +
              (Point[1] * Point[1] - 10 * Math.Cos(2 * Math.PI * Point[1]));
        }
    }
    internal class Program
    {
        private static void Main(string[] args)
        {
            
            IOOOptimizer<BBBCParams> bbbc = new BBBCOptimizer();
            IOOOptimizer<FWParams> fw = new FWOptimizer();
            IOOOptimizer<GEMParams> gem = new GEMOptimizer();

            BBBCParams param1 = new BBBCParams(20, 100, 0.4, 0.5);
            FWParams param2 = new FWParams(20, 100, 20, 10, 20, 40);
            GEMParams param3 = new GEMParams(1, 100, 50, 2 * Math.Sqrt(2), 100);


            IOOOptProblem param = new RastriginProblem();

            Console.WriteLine("Exact solution: f(x) = 0, x = (0, 0).");
            Console.WriteLine();

            Test(bbbc, param1, param, "BBBC");
            Test(fw, param2, param, "Fireworks");
            Test(gem, param3, param, "GEM");

            Console.WriteLine("Complete");
            Console.ReadKey();
        }

        private static void Test<T>(IOOOptimizer<T> Opt, T Parameters, IOOOptProblem Problem, string Method)
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