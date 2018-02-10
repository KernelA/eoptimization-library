namespace EOpt.Math.Optimization.Tests
{
    using System;

    using EOpt.Exceptions;
    using EOpt.Math.Optimization;
    using EOpt.Math.Optimization.OOOpt;

    using Xunit;

    public class FireworksOptimizerTests
    {
        [Theory]
        [InlineData(InvalidFunc.NaNFunc)]
        [InlineData(InvalidFunc.PosInfFunc)]
        [InlineData(InvalidFunc.NegInfFunc)]
        public void FWOptimizerTestArithmeticException(InvalidFunc TypeFunc)
        {
            FWOptimizer fw = new FWOptimizer();

            FWParams param = new FWParams(10, 1000 * 1000, 3);

            Assert.Throws<InvalidValueFunctionException>(() => GeneralOptimizerTests.TestInavlidFunction(fw, param, TypeFunc));
        }

        [Fact]
        public void FWOptimizerTestCancel()
        {
            FWOptimizer fw = new FWOptimizer();

            // Distance is Manhattan distance.
            FWParams param = new FWParams(10, 1000 * 1000, 3);

            bool error = GeneralOptimizerTests.TestCancel(fw, param);

            Assert.False(error);
        }

        [Fact]
        public void FWOptimizerTestOptimization()
        {
            FWOptimizer fw = new FWOptimizer();

            FWParams param = new FWParams(10, GeneralOptimizerTests.ITER_MAX, 3);

            bool error = GeneralOptimizerTests.TestOptimizer(fw, param);

            Assert.False(error);
        }

        [Fact]
        public void FWOptimizerTestReporter()
        {
            FWOptimizer fw = new FWOptimizer();

            FWParams param = new FWParams(10, GeneralOptimizerTests.ITER_MAX, 3);

            var reporter = new TestReporter(typeof(FWOptimizer), 0, GeneralOptimizerTests.ITER_MAX - 1);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            fw.Minimize(param, new OOOptimizationProblem(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LowerBounds, GeneralOptimizerTests.UpperBounds), reporter);

            Assert.False(reporter.Error);
        }

        [Fact]
        public void FWOptimizerTestWrongParams()
        {
            FWOptimizer fw = new FWOptimizer();

            Assert.Throws<ArgumentException>(() => GeneralOptimizerTests.TestWrongParams(fw));
        }
    }
}