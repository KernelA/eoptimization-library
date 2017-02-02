namespace EOpt.Math.Random.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EOpt.Math.Random;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    [TestClass()]
    public class NormalDistributionTests
    {
        [TestMethod()]
        public void NormalDistributionConstrTest()
        {
            NormalDistribution dist = new NormalDistribution();

            Assert.IsTrue(dist.Mean == 0 && dist.StdDev == 1);
        }

        [TestMethod()]
        public void NormalDistributionConstrTest1()
        {
            bool error = true;

            try
            {
                NormalDistribution dist = new NormalDistribution(1, -2);
            }
            catch(ArgumentException exc)
            {
                error = false;
            }

            Assert.IsFalse(error);
            
        }

        [TestMethod()]
        public void NormalDistributionConstrTest3()
        {
            NormalDistribution dist = new NormalDistribution(2, 5);

            Assert.IsTrue(dist.Mean == 2 && dist.StdDev == 5);
        }

        [TestMethod()]
        public void NRandValTest()
        {
            NormalDistribution dist = new NormalDistribution(0, 2);

            bool error = true;

            try
            {
                dist.NRandVal(1, -1);
            }
            catch(ArgumentException exc)
            {
                error = false;
            }

            Assert.IsFalse(error);
        }

   
    }
}