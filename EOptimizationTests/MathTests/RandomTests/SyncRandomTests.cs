namespace EOpt.Math.Random.Tests
{
    using System;
    using System.Threading;

    using EOpt.Math.Random;

    using Xunit;

    public class SyncRandomTests
    {
        private class TempResult
        {
            public Random Rand = null;
        }

        [Fact]
        public void GetDiffThreadTest()
        {
            Thread t1 = new Thread(obj => ((TempResult)obj).Rand = SyncRandom.Get());

            Random r2 = null;

            r2 = SyncRandom.Get();

            TempResult res = new TempResult();

            t1.Start(res);

            t1.Join();

            Assert.True(res.Rand != r2 && res.Rand != null);
        }

        [Fact]
        public void GetTest()
        {
            Random rand = SyncRandom.Get();

            Assert.NotNull(rand);
        }
    }
}