namespace ML.Math
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Combinatorics algorithms.
    /// </summary>
    public static class Сombinatorics
    {

        /// <summary>
        /// Random permutation with uniform distribution.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="rand"></param>
        public static void RandomPermutation<T>(T[] array, Random rand)
        {
            int j = 0;

            T temp;

            for (int i = 0; i < array.Length; i++)
            {
                j = rand.Next(i + 1);
                temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
    }
}
