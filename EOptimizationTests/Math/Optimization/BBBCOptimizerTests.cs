namespace EOptimizationTests.Math.Optimization
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    using EOpt.Math.Optimization;
    using EOpt.Math.Optimization.Tests;

    [TestClass]
    public class BBBCTest
    {
        [TestMethod]
        public void BBBCTestOptimization()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, GeneralOptimizerTests.IterMax, 0.4, 0.3);

            bool error = GeneralOptimizerTests.TestOptimizer(bb, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.IsFalse(error);
        }


        [TestMethod()]
        public void BBBCTestWrongParams()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            bool error = GeneralOptimizerTests.TestWrongParams(bb);

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void BBBCTestWrongInvoke()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            bool error = GeneralOptimizerTests.TestWrongInvoke(bb);

            Assert.IsFalse(error);           
        }

        [TestMethod()]
        public void BBBCTestReporter()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, GeneralOptimizerTests.IterMax, 0.4, 0.3);

            bb.InitializeParameters(param);

            var reporter = new TestReporter(typeof(BBBCOptimizer), 1, GeneralOptimizerTests.IterMax);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            bb.Optimize(new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound), reporter);

            Assert.IsFalse(reporter.Error);
        }

        [TestMethod()]
        public void BBBCTestCancel()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, 1000 * 1000, 0.4, 0.3);

            bool error = GeneralOptimizerTests.TestCancel(bb, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.IsFalse(error);

        }

    }
}
