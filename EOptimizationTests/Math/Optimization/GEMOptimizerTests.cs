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

            GEMParams param = new GEMParams(1, 5, 10, 0.6, 10);

            gem.InitializeParameters(param);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            gem.Optimize(new GeneralParams(x => x[0] * x[0] + x[1] * x[1], new double[] { -10, -10 }, new double[] { 10, 10 }));

            Assert.IsTrue(gem.Solution != null && gem.Solution.Dimension == 3);
        }

        [TestMethod()]
        public void GEMOptimizerTestWrongParams()
        {
            GEMOptimizer gem = new GEMOptimizer();

            bool error = true;

            try
            {
                gem.InitializeParameters(new object());
            }
            catch (ArgumentException exc)
            {
                error = false;
                Debug.Indent();
                Debug.WriteLine(exc.Message);
            }

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void GEMOptimizerTestWrongInvoke()
        {
            BBBCOptimizer gem = new BBBCOptimizer();

            bool error = true;

            try
            {
                gem.Optimize(new GeneralParams(x => x[0] * x[0] + x[1] * x[1], new double[] { -10, -10 }, new double[] { 10, 10 }));
            }
            catch (InvalidOperationException exc)
            {
                error = false;
                Debug.Indent();
                Debug.WriteLine(exc.Message);
            }

            Assert.IsFalse(error);

        }
    }
}