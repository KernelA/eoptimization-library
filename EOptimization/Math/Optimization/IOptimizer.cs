namespace EOpt.Math.Optimization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using EOpt.Math;

    /// <summary>
    /// Interface for optimization methods.
    /// </summary>
    public interface IOptimizer<T>
    {
        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        PointND Solution { get; }

        /// <summary>
        /// Initializing parameters of methods.
        /// </summary>
        /// <param name="parametrs">Parameters for method.</param>
        void InitializeParameters(T parametrs);

        

        /// <summary>
        /// Finding solution of the constrained optimization problem.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        void Minimize(GeneralParams parametrs);

        /// <summary>
        /// Finding solution of the constrained optimization problem.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        /// <param name="cancelToken"><see cref="CancellationToken"/></param>
        void Minimize(GeneralParams parametrs, CancellationToken cancelToken);

        /// <summary>
        /// Finding solution of the constrained optimization problem. If you want see progress, then you need set <paramref name="reporter"/>.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface IProgress&lt;Progress&gt; <see cref="Progress"/>.</param>
        void Minimize(GeneralParams parametrs, IProgress<Progress> reporter);

        /// <summary>
        /// Finding solution of the constrained optimization problem. If you want see progress, then you need set <paramref name="reporter"/>.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface IProgress&lt;Progress&gt; <see cref="Progress"/>.</param>
        /// <param name="cancelToken"><see cref="CancellationToken"/></param>
        void Minimize(GeneralParams parametrs, IProgress<Progress> reporter, CancellationToken cancelToken);
    }
}