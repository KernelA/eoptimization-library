using System;


using EOpt.Math.Optimization;
using EOpt.Math;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            Func<double[], double> func = (x) => 20 + (x[0] * x[0] - 10 * Math.Cos(2 * Math.PI * x[0])) +
               (x[1] * x[1] - 10 * Math.Cos(2 * Math.PI * x[1]));


            IOptimizer[] opts = {
                 new BBBCOptimizer(),
                 new FireworksOptimizer(),
                 new GEMOptimizer()
            };

            // Distance between points need for Fireworks method.
            // It is squared Euclidean distance.
            Func<PointND, PointND, double> distance = (a, b) => (a[0] - b[0]) * (a[0] - b[0]) + (a[1] - b[1]) * (a[1] - b[1]);


            object[] parameters =  {
                new BBBCParams(20, 100, 0.4, 0.5),
                new FireWorksParams(20, 50, distance, 20),
                new GEMParams(1, 20, 50, 2 * Math.Sqrt(2), 100)
             };
         
            double[] constr1 = { -5.12, -5.12 };
            double[] constr2 = { 5.12, 5.12 };

            GeneralParams param = new GeneralParams(func, constr1, constr2);

            string[] names =
            {
                "BBBC",
                "Fireworks",
                "GEM"
            };


            for (int i = 0; i < opts.Length; i++)
            {
                opts[i].InitializeParameters(parameters[i]);

                opts[i].Optimize(param);

                Console.WriteLine($"Method: {names[i]}.");
                Console.WriteLine(opts[i].Solution);
                Console.WriteLine();
            }
            
            Console.WriteLine("Complete");
            Console.ReadKey();
        }
    }
}
