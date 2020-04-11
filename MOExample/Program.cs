namespace MOExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EOpt.Math.Optimization;
    using EOpt.Math.Optimization.MOOpt;

    internal class ZDT1 : IMOOptProblem
    {
        private double[] _res;

        private double[] _lowerBounds, _upperBounds;

        public int CountObjs => 2;

        public IReadOnlyList<double> LowerBounds => _lowerBounds;

        public IReadOnlyList<double> UpperBounds => _upperBounds;

        private double F1(IReadOnlyList<double> Point) => Point[0];

        private double F2(IReadOnlyList<double> Point)
        {
            double temp = G(Point);

            return temp * (1 - Math.Sqrt(Point[0] / temp));
        }

        private double G(IReadOnlyList<double> Point)
        {
            double sum = 0;

            for (int i = 1; i < Point.Count; i++)
            {
                sum += Point[i];
            }

            return 1 + 9 / (_lowerBounds.Length - 1) * sum;
        }

        public ZDT1(IReadOnlyCollection<double> LowerBounds, IReadOnlyCollection<double> UpperBounds)
        {
            if (LowerBounds.Count != UpperBounds.Count)
            {
                throw new ArgumentException("Not equal size of bounds.");
            }

            _lowerBounds = LowerBounds.ToArray();
            _upperBounds = UpperBounds.ToArray();

            for (int i = 0; i < _lowerBounds.Length; i++)
            {
                if (_lowerBounds[i] >= _upperBounds[i])
                {
                    throw new ArgumentException($"The lower bound is greater or equal upper bound at position {i}.");
                }
            }

            _res = new double[2];
        }

        public double ObjFunction(IReadOnlyList<double> Point, int NumObj)
        {
            if (NumObj == 0)
            {
                return F1(Point);
            }
            else
            {
                return F2(Point);
            }
        }

        public IEnumerable<double> TargetFunction(IReadOnlyList<double> Point)
        {
            _res[0] = F1(Point);
            _res[1] = F2(Point);

            return _res;
        }
    }

    internal class ProgressReporter : IProgress<EOpt.Help.Progress>
    {
        public void Report(EOpt.Help.Progress progress) => Console.Write($"Progress: {progress.Current / (float)progress.End:P2}\r");
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            IMOOptProblem problem = new ZDT1(Enumerable.Repeat(0.0, 10).ToArray(), Enumerable.Repeat(1.0, 10).ToArray());

            MOFWOptimizer optimizer = new MOFWOptimizer();

            FWParams parameters = new FWParams(50, 200, 20, 10, 30, 2);

            ProgressReporter reporter = new ProgressReporter();

            optimizer.Minimize(parameters, problem, reporter);

            foreach (Agent agent in optimizer.ParetoFront)
            {
                Console.WriteLine($"F1 = {agent.Objs[0]} F2 = {agent.Objs[1]}");
            }

            Console.ReadKey();
        }
    }
}