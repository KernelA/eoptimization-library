// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;
    using System.Threading;

    using Help;

    /// <summary>
    /// </summary>
    /// <typeparam name="TParams"> A type of the parameters. </typeparam>
    /// <typeparam name="TProblem"> A type of the optimization problem. </typeparam>
    public interface IBaseOptimizer<TParams, TProblem>
    {
        /// <summary>
        /// Finding solution of the constrained optimization problem. 
        /// </summary>
        /// <param name="Parameters"> Parameters for method. </param>
        /// <param name="Problem">    An optimization problem. </param>
        void Minimize(TParams Parameters, TProblem Problem);

        /// <summary>
        /// Finding solution of the constrained optimization problem. 
        /// </summary>
        /// <param name="Parameters">  Parameters for method. </param>
        /// <param name="Problem">     An optimization problem. </param>
        /// <param name="CancelToken"> <see cref="CancellationToken"/> </param>
        void Minimize(TParams Parameters, TProblem Problem, CancellationToken CancelToken);

        /// <summary>
        /// Finding solution of the constrained optimization problem. If you want to see progress,
        /// then you need to set <paramref name="Reporter"/>.
        /// </summary>
        /// <param name="Parameters"> Parameters for method. </param>
        /// <param name="Problem">    An optimization problem. </param>
        /// <param name="Reporter">   An object which implements interface <see cref="IProgress{T}"/>. </param>
        void Minimize(TParams Parameters, TProblem Problem, IProgress<Progress> Reporter);

        /// <summary>
        /// Finding solution of the constrained optimization problem. If you want to see progress,
        /// then you need to set <paramref name="Reporter"/>.
        /// </summary>
        /// <param name="Parameters">  Parameters for method. </param>
        /// <param name="Problem">     An optimization problem. </param>
        /// <param name="Reporter">    An object which implements interface <see cref="IProgress{T}"/>. </param>
        /// <param name="CancelToken"> <see cref="CancellationToken"/> </param>
        void Minimize(TParams Parameters, TProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken);
    }
}