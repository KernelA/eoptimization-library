namespace EOpt.Math.Tests
{
    using System.Linq;

    using EOpt.Math;

    using Xunit;

    public class PointNDTests
    {
        [Fact]
        public void CloneTest()
        {
            PointND p1, p2;

            p1 = new PointND(1, 2);

            p2 = p1.DeepCopy();

            Assert.True(p1.Equals(p2));
        }

        [Fact]
        public void EqualsTest()
        {
            PointND a = new PointND(2, 3);

            PointND b = new PointND(21, 4);

            PointND c = a.DeepCopy();

            Assert.True(a.Equals(c) && !a.Equals(b));
        }

        [Fact]
        public void PointNDConstrTest4()
        {
            PointND point = new PointND(12, 5);

            bool error = false;

            foreach (double item in point)
            {
                if (item != 12)
                    error = true;
            }

            Assert.True(!error && point.Count == 5);
        }

        [Fact]
        public void PointNDConstrTest5()
        {
            double[] x = new double[4] { 45, 56, 8, 10 };

            PointND point = new PointND(x);

            Assert.True(point.SequenceEqual(x) && point.Count == 4);
        }

        [Fact]
        public void ToStringTest()
        {
            PointND p1 = new PointND(90, 2);

            Assert.NotNull(p1.ToString());
        }
    }
}