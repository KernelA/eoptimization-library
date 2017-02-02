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
            FireworksOptimizer fw = new FireworksOptimizer();

            // Distance it is Manhattan distance.
            FireWorksParams param = new FireWorksParams(10, 10, (a, b) => (a - b).Coordinates.Sum(item => Math.Abs(item)), 3);
            
            fw.InitializeParameters(param);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            fw.Optimize(new GeneralParams(x => x[0] * x[0] + x[1] * x[1], new double[] { -10, -10 }, new double[] { 10, 10 }));

            Assert.IsTrue(fw.Solution != null && fw.Solution.Dimension == 3);
        }

        [TestMethod()]
        public void FWOptimizerTestWrongParams()
        {
            FireworksOptimizer fw = new FireworksOptimizer();

            bool error = true;

            BBBCParams param = new BBBCParams(1, 3, 0.5, 0.3);

            try
            {
                fw.InitializeParameters(param);
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
        public void FWOptimizerTestWrongInvoke()
        {
            FireworksOptimizer fw = new FireworksOptimizer();

            bool error = true;

            try
            {
                fw.Optimize(new GeneralParams(x => x[0] * x[0] + x[1] * x[1], new double[] { -10, -10 }, new double[] { 10, 10 }));
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