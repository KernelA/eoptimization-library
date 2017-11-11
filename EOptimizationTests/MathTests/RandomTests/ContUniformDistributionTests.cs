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
            bool error = true;

            try
            {
                ContUniformDistribution dist = new ContUniformDistribution(-2, -3);
            }
            catch(ArgumentException exc)
            {
                error = false;
            }

            Assert.False(error);
        }


        [Fact]
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

            Assert.False(error);
        }
    }
}