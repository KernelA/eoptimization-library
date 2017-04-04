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
    public class FireworksOptimizerTests
    {
        [TestMethod]
        public void FWOptimizerTestOptimization()
        {
            FWOptimizer fw = new FWOptimizer();

            // Distance it is Manhattan distance.
            FWParams param = new FWParams(10, GeneralOptimizerTests.IterMax, (a, b) => (a - b).Coordinates.Sum(item => Math.Abs(item)), 3);

            bool error = GeneralOptimizerTests.TestOptimizer(fw, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void FWOptimizerTestWrongParams()
        {
            FWOptimizer fw = new FWOptimizer();

            bool error = GeneralOptimizerTests.TestWrongParams(fw);

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void FWOptimizerTestWrongInvoke()
        {
            FWOptimizer fw = new FWOptimizer();

            bool error = GeneralOptimizerTests.TestWrongInvoke(fw);

            Assert.IsFalse(error);

        }

        [TestMethod]
        public void FWOptimizerTestReporter()
        {
            FWOptimizer fw = new FWOptimizer();

            // Distance it is Manhattan distance.
            FWParams param = new FWParams(10, GeneralOptimizerTests.IterMax, (a, b) => (a - b).Coordinates.Sum(item => Math.Abs(item)), 3);

            fw.InitializeParameters(param);

            var reporter = new TestReporter(typeof(BBBCOptimizer), 0, GeneralOptimizerTests.IterMax - 1);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            fw.Optimize(new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound), reporter);

            Assert.IsFalse(reporter.Error);
        }

        [TestMethod]
        public void FWOptimizerTestCancel()
        {
            FWOptimizer fw = new FWOptimizer();

            // Distance it is Manhattan distance.
            FWParams param = new FWParams(10, 1000 * 1000, (a, b) => (a - b).Coordinates.Sum(item => Math.Abs(item)), 3);


            bool error = GeneralOptimizerTests.TestCancel(fw, param, new GeneralParams(GeneralOptimizerTests.TargetFunction, GeneralOptimizerTests.LeftBound, GeneralOptimizerTests.RightBound));

            Assert.IsFalse(error);

        }


    }
}