// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.MOOpt
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for multiobjective optimization methods. 
    /// </summary>
    /// <typeparam name="TParams"></typeparam>
    public interface IMOOptimizer<TParams> : IBaseOptimizer<TParams, MOOptimizationProblem>
    {
        /// <summary>
        /// The solution of the constrained optimization problem. 
        /// </summary>
        IEnumerable<Agent> ParetoFront { get; }
    }
}