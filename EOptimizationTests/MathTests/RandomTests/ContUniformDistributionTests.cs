namespace EOpt.Math.Random.Tests
{
	using Xunit;
	using EOpt.Math.Random;
	using System;

    public class ContUniformDistributionTests
    {
        [Fact]
        public void ContUniformDistributionConstrTest()
        {
            ContUniformDistribution dist = new ContUniformDistribution();

            Assert.True(dist.LowBound == 0 && dist.UpperBound == 1);
        }

        [Fact]
        public void ContUniformDistributionConstrTest1()
        {

            Assert.Throws<ArgumentException>(() => new ContUniformDistribution(-2, -3));
        }


        [Fact]
        public void URandValTest()
        {

            ContUniformDistribution dist = new ContUniformDistribution();

            Assert.Throws<ArgumentException>(() => dist.URandVal(3, 2));

        }
    }
}