namespace EOpt.Math.Optimization
{

    using System;

    /// <summary>
    /// General parameters for  <see cref="IOptimizer.Optimize(GeneralParams)"/>. See interface <see cref="IOptimizer"/>.
    /// </summary>
    public class GeneralParams
    {
        /// <summary>
        /// Coordinates of first vertex.
        /// </summary>
        public double[] LeftBound { get; private set; }

        /// <summary>
        /// Coordinates of second vertex.
        /// </summary>
        public double[] RightBound { get; private set; }

        /// <summary>
        /// Target function.
        /// </summary>
        public Func<double[], double> ObjectiveFunction { get; private set; }

        /// <summary>
        /// <para>
        /// Parameters for <see cref="IOptimizer.InitializeParameters(object)"/>.
        /// </para>
        /// <para>
        /// Constraints is rectangular parallelepiped. First vertex of rectangular parallelepiped has coordinates <paramref name="leftBound[i]"/>, where i from 1 to dimension of space.
        /// Second vertex of rectangular parallelepiped has coordinates <paramref name="rightBound[i]"/>, where i from 1 to dimension of space.
        /// </para>
        /// </summary>
        /// <param name="objFunction">Target function.</param>
        /// <param name="leftBound">Coordinates of first vertex.</param>
        /// <param name="rightBound">Coordinates of second vertex.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public GeneralParams(Func<double[], double> objFunction, double[] leftBound, double[] rightBound)
        {
            if (leftBound == null)
                throw new ArgumentNullException(nameof(leftBound));
            if (rightBound == null)
                throw new ArgumentNullException(nameof(rightBound));
            if (objFunction == null)
                throw new ArgumentNullException(nameof(objFunction));
            if (leftBound.Length != rightBound.Length)
                throw new ArgumentException("Length " + nameof(leftBound) + " and " + nameof(rightBound) + " must be equal.");

            for (int i = 0; i < leftBound.Length; i++)
            {
                if (leftBound[i] > rightBound[i])
                    throw new ArgumentException(nameof(leftBound) + $"[{i}]" + " greater than " + nameof(rightBound) + $"[{i}].");
            }

            LeftBound = leftBound;
            RightBound = rightBound;
            ObjectiveFunction = objFunction;
        }
    }
}
