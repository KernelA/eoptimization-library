namespace EOpt.Math.Optimization.Tests
{
    using Xunit;
    using EOpt.Math.Optimization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class FireworksOptimizerTests
    {
        [Fact]
        public void FWOptimizerTestOptimization()
        {
            FWOptimizer fw = new FWOptimizer();

            // Distance  is Manhattan distance.
            FWParams param = new FWParams(10, GeneralOptimizerTests.IterMax, (a, b) => (a - b).Coordinates.Sum(item => Math.Abs(item)), 3);

            bool error = GeneralOptimizerTests.TestOptimizer(fw, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.False(error);
        }

        [Fact]
        public void FWOptimizerTestWrongParams()
        {
            FWOptimizer fw = new FWOptimizer();

            bool error = GeneralOptimizerTests.TestWrongParams(fw);

            Assert.False(error);
        }

        [Fact]
        public void FWOptimizerTestWrongInvoke()
        {
            FWOptimizer fw = new FWOptimizer();

            bool error = GeneralOptimizerTests.TestWrongInvoke(fw);

            Assert.False(error);

        }

        [Fact]
        public void FWOptimizerTestReporter()
        {
            FWOptimizer fw = new FWOptimizer();

            // Distance is Manhattan distance.
            FWParams param = new FWParams(10, GeneralOptimizerTests.IterMax, (a, b) => (a - b).Coordinates.Sum(item => Math.Abs(item)), 3);

            fw.InitializeParameters(param);

            var reporter = new TestReporter(typeof(BBBCOptimizer), 0, GeneralOptimizerTests.IterMax - 1);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].

            fw.Minimize(new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound), reporter);

            Assert.False(reporter.Error);
        }

        [Fact]
        public void FWOptimizerTestCancel()
        {
            FWOptimizer fw = new FWOptimizer();

            // Distance  is Manhattan distance.
            FWParams param = new FWParams(10, 1000 * 1000, (a, b) => (a - b).Coordinates.Sum(item => Math.Abs(item)), 3);


            bool error = GeneralOptimizerTests.TestCancel(fw, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.False(error);

        }


        [Fact]
        public void FWOptimizerTestArithmeticException()
        {
            FWOptimizer fw = new FWOptimizer();

            FWParams param = new FWParams(10, 1000 * 1000, (a, b) => (a - b).Coordinates.Sum(item => Math.Abs(item)), 3);

            bool error = GeneralOptimizerTests.TestInavlidFunction(fw, param, GeneralOptimizerTests.FunctionNaN);

            error |= GeneralOptimizerTests.TestInavlidFunction(fw, param, GeneralOptimizerTests.FunctionNegInf);

            error |= GeneralOptimizerTests.TestInavlidFunction(fw, param, GeneralOptimizerTests.FunctionPosInf);

            Assert.False(error);
        }

    }
}