namespace EOptimizationTests.Math.Optimization
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    using EOpt.Math.Optimization;

    [TestClass]
    public class BBBCTest
    {
        [TestMethod]
        public void BBBCTestOptimization()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            BBBCParams param = new BBBCParams(10, 10, 0.4, 0.3);

            bb.InitializeParameters(param);

            // Optimization f(x,y)=x^2 + y^2 on [-10;10]x[-10;10].
            bb.Optimize(new GeneralParams(x => x[0] * x[0] + x[1] * x[1], new double[] { -10, -10 }, new double[] { 10, 10 }));

            Assert.IsTrue(bb.Solution != null && bb.Solution.Dimension == 3);
        }


        [TestMethod()]
        public void BBBCTestWrongParams()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            bool error = true;

            try
            {
                bb.InitializeParameters(new object());
            }
            catch(ArgumentException exc)
            {
                error = false;
                Debug.Indent();
                Debug.WriteLine(exc.Message);
            }

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void BBBCTestWrongInvoke()
        {
            BBBCOptimizer bb = new BBBCOptimizer();

            bool error = true;

            try
            {
                bb.Optimize(new GeneralParams(x => x[0] * x[0] + x[1] * x[1], new double[] { -10, -10 }, new double[] { 10, 10 }));
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
