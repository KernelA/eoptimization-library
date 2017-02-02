namespace EOpt.Math.Random.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EOpt.Math.Random;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;

    [TestClass()]
    public class SyncRandomTests
    {

        private class TempResult
        {
            public Random Rand = null;
        }

        [TestMethod()]
        public void GetTest()
        {
            Random rand = SyncRandom.Get();

            Assert.IsNotNull(rand);
        }

        [TestMethod()]
        public void GetDiffThreadTest()
        {
            Thread t1 = new Thread(obj => ((TempResult)obj).Rand = SyncRandom.Get() );

            Random r2 = null;

            r2 = SyncRandom.Get();

            TempResult res = new TempResult();

            t1.Start(res);

            t1.Join();

            Assert.IsTrue(res.Rand != r2 && res.Rand != null);
        }


    }
}