namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// Random generator for multithreaded application. Each thread get own instance of Random.
    /// </summary>
    public static class SyncRandom
    {
        private static Random globalRnd = new Random();

        private static readonly object locker = new object();

        [ThreadStatic]
        private static Random localRnd;


        /// <summary>
        /// Get Random instance. Each thread get own instance of Random.
        /// </summary>
        /// <returns></returns>
        public static Random Get()
        {
            Random inst = localRnd;

            if (inst == null)
            {
                int seed = 0;

                lock (locker)
                {
                   seed = globalRnd.Next();
                }

                localRnd = inst = new Random(seed);                        
            }

            return inst;
        }

    }

 
}
