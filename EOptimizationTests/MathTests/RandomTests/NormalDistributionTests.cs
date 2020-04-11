namespace EOpt.Math.Random.Tests
{
    using System;

    using EOpt.Math.Random;

    using Xunit;

    public class NormalDistributionTests
    {
        [Fact]
        public void NormalDistributionConstrTest()
        {
            NormalDist dist = new NormalDist();

            Assert.True(dist.Mean == 0 && dist.StdDev == 1);
        }

        [Fact]
        public void NormalDistributionConstrTest1()
        {
            Assert.Throws<ArgumentException>(() => new NormalDist(1, -2));
        }

        [Fact]
        public void NormalDistributionConstrTest3()
        {
            NormalDist dist = new NormalDist(2, 5);

            Assert.True(dist.Mean == 2 && dist.StdDev == 5);
        }

        [Fact]
        public void NRandValTest()
        {
            NormalDist dist = new NormalDist(0, 2);

            Assert.Throws<ArgumentException>(() => dist.NRandVal(1, -1));
        }
    }
}