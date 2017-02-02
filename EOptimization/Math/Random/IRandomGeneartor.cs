namespace EOpt.Math.Random
{
    using System;

    /// <summary>
    /// General interface for random generator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRandomGeneartor<T>
    {
        /// <summary>
        /// Get random value. 
        /// </summary>
        /// <returns></returns>
        T RandVal();

    }
}
