using Microsoft.VisualStudio.TestTools.UnitTesting;
using EOpt.Math.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOpt.Math.Random.Tests
{
    [TestClass()]
    public class ContUniformDistributionTests
    {
        [TestMethod()]
        public void ContUniformDistributionConstrTest()
        {
            ContUniformDistribution dist = new ContUniformDistribution();

            Assert.IsTrue(dist.LeftBound == 0 && dist.RightBound == 1);
        }

        [TestMethod()]
        public void ContUniformDistributionConstrTest1()
        {
            bool error = true;

            try
            {
                ContUniformDistribution dist = new ContUniformDistribution(-2, -3);
            }
            catch(ArgumentException exc)
            {
                error = false;
            }

            Assert.IsFalse(error);
        }


        [TestMethod()]
        public void URandValTest()
        {
            bool error = true;

            ContUniformDistribution dist = new ContUniformDistribution();

            try
            {
                dist.URandVal(3, 2);
            }
            catch (ArgumentException exc)
            {
                error = false;
            }

            Assert.IsFalse(error);
        }
    }
}