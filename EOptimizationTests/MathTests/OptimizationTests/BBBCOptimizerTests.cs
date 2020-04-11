namespace EOpt.Math.Optimization.Tests
{
    using System;

    using EOpt.Exceptions;
    using EOpt.Math.Optimization;
    using EOpt.Math.Optimization.OOOpt;

    using Xunit;

    public class BBBCTest
    {
        [Theory]
        [InlineData(InvalidFunc.NaNFunc)]
        [InlineData(InvalidFunc.PosInfFunc)]
        [InlineData(InvalidFunc.NegInfFunc)]
        public void BBBCTestArithmeticException(InvalidFunc TypeFunc)
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, GeneralOptimizerTests.ITER_MAX, 0.4, 0.3);

            Assert.Throws<InvalidValueFunctionException>(() => GeneralOptimizerTests.TestInavlidFunction(bb, param, TypeFunc));
        }

        [Fact]
        public void BBBCTestCancel()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, 1000 * 1000, 0.4, 0.3);

            bool error = GeneralOptimizerTests.TestCancel(bb, param);

            Assert.False(error);
        }

        [Fact]
        public void BBBCTestOptimization()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, GeneralOptimizerTests.ITER_MAX, 0.4, 0.3);

            bool error = GeneralOptimizerTests.TestOptimizer(bb, param);

            Assert.False(error);
        }

        [Fact]
        public void BBBCTestReporter()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, GeneralOptimizerTests.ITER_MAX, 0.4, 0.3);

            var reporter = new TestReporter(typeof(BBBCOptimizer), 1, GeneralOptimizerTests.ITER_MAX);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            bb.Minimize(param, new OOOptimizationProblem(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LowerBounds, GeneralOptimizerTests.UpperBounds), reporter);

            Assert.False(reporter.Error);
        }

        [Fact]
        public void BBBCTestWrongParams()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            Assert.Throws<ArgumentException>(() => GeneralOptimizerTests.TestWrongParams(bb));
        }
    }
}