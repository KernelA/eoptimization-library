namespace EOpt.Math.Tests
{
    using EOpt.Math;

    using Xunit;

    public class KahanSumTests
    {
        [Fact]
        public void ZeroSumTest()
        {
            KahanSum sum = new KahanSum();

            for (int i = 0; i < 10; i++)
            {
                sum.Add(0.0);
            }

            Assert.Equal(0.0, sum.Sum);
        }

        [Fact]
        public void SumTest()
        {
            KahanSum sum = new KahanSum();

            for (int i = 0; i < 100; i++)
            {
                sum.Add(i);
            }

            Assert.Equal(4950.0, sum.Sum);
        }

        [Fact]
        public void SumResetTest()
        {
            KahanSum sum = new KahanSum();

            sum.Add(-1);
            sum.Add(-2);
            sum.Add(-3);

            sum.SumResest();

            sum.Add(1);
            sum.Add(2);

            Assert.Equal(3.0, sum.Sum);
        }
    }
}