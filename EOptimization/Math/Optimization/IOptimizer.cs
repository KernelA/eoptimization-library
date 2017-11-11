// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using EOpt.Math;

    using Help;

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
        /// Finding solution of the constrained optimization problem. If you want to see progress, then you need to set <paramref name="reporter"/>.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">An object which implements interface <see cref="IProgress{T}"/>.</param>
        void Minimize(GeneralParams parametrs, IProgress<Progress> reporter);

        /// <summary>
        /// Finding solution of the constrained optimization problem. If you want to see progress, then you need to set <paramref name="reporter"/>.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">An object which implements interface <see cref="IProgress{T}"/>.</param>
        /// <param name="cancelToken"><see cref="CancellationToken"/></param>
        void Minimize(GeneralParams parametrs, IProgress<Progress> reporter, CancellationToken cancelToken);
    }
}