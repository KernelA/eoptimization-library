namespace EOpt.Math.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EOpt.Math;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass()]
    public class PointNDTests
    {

        [TestMethod()]
        public void PointNDConstrTest4()
        {
            PointND point = new PointND(12, 5);

            bool error = false;

            foreach (double item in point.Coordinates)
            {
                if (item != 12)
                    error = true;
            }

            Assert.IsTrue(!error && point.Dimension == 5);
        }

        [TestMethod()]
        public void PointNDConstrTest5()
        {
            double[] x = new double[4] { 45, 56, 8, 10 };

            PointND point = new PointND(x);

            Assert.IsTrue(point.Coordinates.SequenceEqual(x) && point.Dimension == 4);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            PointND a = new PointND(2, 3);

            PointND b = new PointND(21, 4);

            PointND c = a.Clone();

            Assert.IsTrue(a.Equals(c) && !a.Equals(b));
        }


        [TestMethod()]
        public void CloneTest()
        {
            PointND p1, p2;

            p1 = new PointND(1, 2);

            p2 = p1.Clone();

            Assert.IsTrue(p1.Equals(p2));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            PointND p1 = new PointND(90, 2);

            Assert.IsNotNull(p1.ToString());
        }
    }
}