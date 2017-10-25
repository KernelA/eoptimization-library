namespace EOptimizationTests.Math.Optimization
{
    using System;
    using Xunit;

    using EOpt.Math.Optimization;
    using EOpt.Math.Optimization.Tests;

    public class BBBCTest
    {
        [Fact]
        public void BBBCTestOptimization()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, GeneralOptimizerTests.IterMax, 0.4, 0.3);

            bool error = GeneralOptimizerTests.TestOptimizer(bb, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.False(error);
        }


        [Fact]
        public void BBBCTestWrongParams()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            bool error = GeneralOptimizerTests.TestWrongParams(bb);

            Assert.False(error);
        }

        [Fact]
        public void BBBCTestWrongInvoke()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            bool error = GeneralOptimizerTests.TestWrongInvoke(bb);

            Assert.False(error);           
        }

        [Fact]
        public void BBBCTestReporter()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, GeneralOptimizerTests.IterMax, 0.4, 0.3);

            bb.InitializeParameters(param);

            var reporter = new TestReporter(typeof(BBBCOptimizer), 1, GeneralOptimizerTests.IterMax);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            bb.Minimize(new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound), reporter);

            Assert.False(reporter.Error);
        }

        [Fact]
        public void BBBCTestCancel()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, 1000 * 1000, 0.4, 0.3);

            bool error = GeneralOptimizerTests.TestCancel(bb, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.False(error);

        }

    }
}
