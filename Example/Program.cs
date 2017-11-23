using System;

using EOpt.Math.Optimization;
using EOpt.Math;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            /// See http://www.sfu.ca/~ssurjano/rastr.html
            Func<double[], double> func = (x) => 20 + (x[0] * x[0] - 10 * Math.Cos(2 * Math.PI * x[0])) +
               (x[1] * x[1] - 10 * Math.Cos(2 * Math.PI * x[1]));


            IOptimizer<BBBCParams> bbbc = new BBBCOptimizer();
            IOptimizer<FWParams> fw = new FWOptimizer();
            IOptimizer<GEMParams> gem = new GEMOptimizer();

            // Distance between points need for Fireworks method.
            // It is squared Euclidean distance.
            Func<PointND, PointND, double> distance = (a, b) => (a[0] - b[0]) * (a[0] - b[0]) + (a[1] - b[1]) * (a[1] - b[1]);


            BBBCParams param1 = new BBBCParams(20, 100, 0.4, 0.5);
            FWParams param2 = new FWParams(20, 100, distance, 20);
            GEMParams param3 = new GEMParams(1, 100, 50, 2 * Math.Sqrt(2), 100);


            double[] constr1 = { -5.12, -5.12 };
            double[] constr2 = { 5.12, 5.12 };

            GeneralParams param = new GeneralParams(func, constr1, constr2);


            Console.WriteLine("Exact solution: f(x) = 0, x = (0,0).");
            Console.WriteLine();

            Test(bbbc, param1, param, "BBBC");
            Test(fw, param2, param, "Fireworks");
            Test(gem, param3, param, "GEM");

            Console.WriteLine("Complete");
            Console.ReadKey();
        }

        static void Test<T>(IOptimizer<T> Opt, T Parameters, GeneralParams GenParam, string Method)
        {
            Opt.InitializeParameters(Parameters);

            PointND bestSolution = null;

            double min = 0;

            Opt.Minimize(GenParam);

            bestSolution = Opt.Solution;

            for (int i = 1; i < 10; i++)
            {
                Opt.Minimize(GenParam);

                if (Opt.Solution[2] < min)
                {
                    bestSolution = Opt.Solution;
                    min = bestSolution[2];
                }
            }


            Console.WriteLine($"Method: {Method}.");
            Console.WriteLine($"Solution: f(x) = {bestSolution[2]}, x = ({bestSolution[0]}, {bestSolution[1]}).");
            Console.WriteLine();
        }
    }
}