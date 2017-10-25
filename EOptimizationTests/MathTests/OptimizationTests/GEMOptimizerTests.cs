namespace EOpt.Math.Optimization.Tests
{
    using Xunit;
    using EOpt.Math.Optimization;
    using System;


    public class GEMOptimizerTests
    {
        [Fact]
        public void GEMOptimizerTestOptimization()
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, GeneralOptimizerTests.IterMax, 0.6, 10);

            gem.InitializeParameters(param);

            bool error = GeneralOptimizerTests.TestOptimizer(gem, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.False(error);
        }

        [Fact]
        public void GEMOptimizerTestWrongParams()
        {
            GEMOptimizer gem = new GEMOptimizer();

            bool error = GeneralOptimizerTests.TestWrongParams(gem);

            Assert.False(error);
        }

        [Fact]
        public void GEMOptimizerTestWrongInvoke()
        {
            GEMOptimizer gem = new GEMOptimizer();

            bool error = GeneralOptimizerTests.TestWrongInvoke(gem);

            Assert.False(error);

        }

        [Fact]
        public void GEMOptimizerTestReporter()
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, GeneralOptimizerTests.IterMax, 0.6, 10);

            gem.InitializeParameters(param);

            var reporter = new TestReporter(typeof(BBBCOptimizer), 1, GeneralOptimizerTests.IterMax);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            gem.Minimize(new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound), reporter);

            Assert.False(reporter.Error);
        }

        [Fact]
        public void GEMOptimizerCancel()
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, 1000 * 1000, 0.6, 10);

            bool error = GeneralOptimizerTests.TestCancel(gem, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.False(error);

        }
    }
}