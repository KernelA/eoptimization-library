// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using EOpt.Math.Optimization.OOOpt;

    /// <summary>
    /// Interface for optimization methods. 
    /// </summary>
    public interface IOOOptimizer<TParams> : IBaseOptimizer<TParams, IOOOptProblem>
    {
        /// <summary>
        /// The solution of the constrained optimization problem. 
        /// </summary>
        Agent Solution { get; }
    }
}