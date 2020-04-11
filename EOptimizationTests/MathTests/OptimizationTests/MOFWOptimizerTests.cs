namespace EOpt.Math.Optimization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EOpt.Math.Optimization;
    using EOpt.Math.Optimization.MOOpt;

    using Xunit;

    public class MOFWTest
    {
        private static double Obj1(IReadOnlyList<double> point)
        {
            return point[0];
        }

        private static double Obj2(IReadOnlyList<double> point)
        {
            return point[1];
        }

        [Fact]
        public void MOFWOptimizerTest()
        {
            MOFWOptimizer optimizer = new MOFWOptimizer();
            FWParams param = new FWParams(20, 5, 0.1, 2, 5, 0.2);

            MOOptimizationProblem problem = new MOOptimizationProblem(
                new List<Func<IReadOnlyList<double>, double>>()
                {
                   Obj1,
                   Obj2
                },
                Enumerable.Repeat(0.0, 5).ToArray(),
                Enumerable.Repeat(1.0, 5).ToArray()
                );

            optimizer.Minimize(param, problem);

            Assert.NotNull(optimizer.ParetoFront);
        }

        [Fact]
        public void MOFWOptimizerTest2()
        {
            MOFWOptimizer optimizer = new MOFWOptimizer();
            FWParams param = new FWParams(20, 5, 0.1, 2, 5, 0.2);

            MOOptimizationProblem problem = new MOOptimizationProblem(
                new List<Func<IReadOnlyList<double>, double>>()
                {
                   Obj1,
                   Obj2
                },
                Enumerable.Repeat(0.0, 5).ToArray(),
                Enumerable.Repeat(1.0, 5).ToArray()
                );

            optimizer.Minimize(param, problem);

            Assert.True(optimizer.ParetoFront.Count() == param.NP);
        }
    }
}