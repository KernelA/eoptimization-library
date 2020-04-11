// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math
{
    using System.Collections.Generic;

    /// <summary>
    /// Combinatorics algorithms.
    /// </summary>
    public static class Сombinatorics
    {
        /// <summary>
        /// A random choice.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Collection"></param>
        /// <param name="RandGen">   </param>
        /// <returns></returns>
        public static T RandomChoice<T>(IReadOnlyList<T> Collection, System.Random RandGen)
        {
            return Collection[RandGen.Next(Collection.Count)];
        }

        /// <summary>
        /// A random permutation with uniform distribution.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Collection"></param>
        /// <param name="RandGen">   </param>
        public static void RandomPermutation<T>(IList<T> Collection, System.Random RandGen)
        {
            T temp;

            for (int i = 0; i < Collection.Count; i++)
            {
                int j = RandGen.Next(i + 1);
                temp = Collection[i];
                Collection[i] = Collection[j];
                Collection[j] = temp;
            }
        }
    }
}