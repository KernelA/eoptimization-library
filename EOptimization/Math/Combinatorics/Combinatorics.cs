// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace ML.Math
{
    using System;

    /// <summary>
    /// Combinatorics algorithms.
    /// </summary>
    public static class Сombinatorics
    {

        /// <summary>
        /// Random permutation with uniform distribution.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Array"></param>
        /// <param name="RandGen"></param>
        public static void RandomPermutation<T>(T[] Array, Random RandGen)
        {
            int j = 0;

            T temp;

            for (int i = 0; i < Array.Length; i++)
            {
                j = RandGen.Next(i + 1);
                temp = Array[i];
                Array[i] = Array[j];
                Array[j] = temp;
            }
        }
    }
}
