namespace EOpt.Math.Random.Tests
{
    using System;

    using EOpt.Math.Random;

    using Xunit;

    public class ContUniformDistributionTests
    {
        [Fact]
        public void ContUniformDistributionConstrTest()
        {
            ContUniformDist dist = new ContUniformDist();

            Assert.True(dist.LowBound == 0 && dist.UpperBound == 1);
        }

        [Fact]
        public void ContUniformDistributionConstrTest1()
        {
            Assert.Throws<ArgumentException>(() => new ContUniformDist(-2, -3));
        }

        [Fact]
        public void URandValTest()
        {
            ContUniformDist dist = new ContUniformDist();

            Assert.Throws<ArgumentException>(() => dist.URandVal(3, 2));
        }
    }
}