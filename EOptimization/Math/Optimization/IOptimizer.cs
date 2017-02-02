namespace EOpt.Math.Optimization
{
    using System;

    /// <summary>
    /// Interface for optimization methods.
    /// </summary>
    public interface IOptimizer
    {
        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        PointND Solution { get; }

        /// <summary>
        /// Initializing parameters of methods.
        /// </summary>
        /// <param name="parametrs"></param>
        void InitializeParameters(object parametrs);

        /// <summary>
        /// Find solution of the constrained optimization problem. If you want see progress, then you need set <paramref name="reporter"/>.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface <see cref="IProgress{Tuple{int,int,int}}"/>, where first item in tuple is the initial value, second item is the end value, third item is the current progress value.</param>
        void Optimize(GeneralParams parametrs,  IProgress<Tuple<int,int,int>> reporter = null);
    }

    
}
