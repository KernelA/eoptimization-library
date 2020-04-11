// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// Random generator for multithreaded application. Each thread get own instance of Random.
    /// </summary>
    public static class SyncRandom
    {
        private static readonly object _locker = new object();
        private static Random _globalRnd = new Random();

        [ThreadStatic]
        private static Random _localRnd;

        /// <summary>
        /// Get Random instance. Each thread get own instance of Random.
        /// </summary>
        /// <returns></returns>
        public static Random Get()
        {
            Random inst = _localRnd;

            if (inst == null)
            {
                int seed = 0;

                lock (_locker)
                {
                    seed = _globalRnd.Next();
                }

                _localRnd = inst = new Random(seed);
            }

            return inst;
        }
    }
}