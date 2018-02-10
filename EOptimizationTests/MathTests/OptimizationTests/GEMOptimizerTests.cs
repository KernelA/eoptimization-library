namespace EOpt.Math.Optimization.Tests
{
    using System;

    using EOpt.Exceptions;
    using EOpt.Math.Optimization;
    using EOpt.Math.Optimization.OOOpt;

    using Xunit;

    public class GEMOptimizerTests
    {
        [Fact]
        public void GEMOptimizerCancel()
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, 1000 * 1000, 0.6, 10);

            bool error = GeneralOptimizerTests.TestCancel(gem, param);

            Assert.False(error);
        }

        [Theory]
        [InlineData(InvalidFunc.NaNFunc)]
        [InlineData(InvalidFunc.PosInfFunc)]
        [InlineData(InvalidFunc.NegInfFunc)]
        public void GEMOptimizerTestArithmeticException(InvalidFunc TypeFunc)
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, 1000 * 1000, 0.6, 10);

            Assert.Throws<InvalidValueFunctionException>(() => GeneralOptimizerTests.TestInavlidFunction(gem, param, TypeFunc));
        }

        [Fact]
        public void GEMOptimizerTestOptimization()
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, GeneralOptimizerTests.ITER_MAX, 0.6, 10);

            bool error = GeneralOptimizerTests.TestOptimizer(gem, param);

            Assert.False(error);
        }

        [Fact]
        public void GEMOptimizerTestReporter()
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, GeneralOptimizerTests.ITER_MAX, 0.6, 10);

            var reporter = new TestReporter(typeof(GEMOptimizer), 1, GeneralOptimizerTests.ITER_MAX);

            gem.Minimize(param, new OOOptimizationProblem(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LowerBounds, GeneralOptimizerTests.UpperBounds), reporter);

            Assert.False(reporter.Error);
        }

        [Fact]
        public void GEMOptimizerTestWrongParams()
        {
            GEMOptimizer gem = new GEMOptimizer();

            Assert.Throws<ArgumentException>(() => GeneralOptimizerTests.TestWrongParams(gem));
        }
    }
}