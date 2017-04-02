namespace EOpt.Math.Optimization.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EOpt.Math.Optimization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System.Diagnostics;

    [TestClass()]
    public class GEMOptimizerTests
    {
        [TestMethod]
        public void GEMOptimizerTestOptimization()
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, GeneralOptimizerTests.IterMax, 0.6, 10);

            gem.InitializeParameters(param);

            bool error = GeneralOptimizerTests.TestOptimizer(gem, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void GEMOptimizerTestWrongParams()
        {
            GEMOptimizer gem = new GEMOptimizer();

            bool error = GeneralOptimizerTests.TestWrongParams(gem);

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void GEMOptimizerTestWrongInvoke()
        {
            GEMOptimizer gem = new GEMOptimizer();

            bool error = GeneralOptimizerTests.TestWrongInvoke(gem);

            Assert.IsFalse(error);

        }

        [TestMethod()]
        public void GEMOptimizerTestReporter()
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, GeneralOptimizerTests.IterMax, 0.6, 10);

            gem.InitializeParameters(param);

            var reporter = new TestReporter(typeof(BBBCOptimizer), 1, GeneralOptimizerTests.IterMax);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            gem.Optimize(new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound), reporter);

            Assert.IsFalse(reporter.Error);
        }

        [TestMethod()]
        public void GEMOptimizerCancel()
        {
            GEMOptimizer gem = new GEMOptimizer();

            GEMParams param = new GEMParams(1, 5, 1000 * 1000, 0.6, 10);

            gem.InitializeParameters(param);

            bool error = GeneralOptimizerTests.TestCancel(gem, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.IsFalse(error);

        }
    }
}