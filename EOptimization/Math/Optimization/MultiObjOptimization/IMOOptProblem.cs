// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.MOOpt
{
    using System.Collections.Generic;

    /// <summary>
    /// Multiobjective problem
    /// </summary>
    public interface IMOOptProblem : IConstrOptProblem<double, IEnumerable<double>>
    {
        /// <summary>
        /// Number of ovjectives
        /// </summary>
        int CountObjs { get; }

        /// <summary>
        /// Target function
        /// </summary>
        /// <param name="Point"></param>
        /// <param name="NumObj"></param>
        /// <returns></returns>
        double ObjFunction(IReadOnlyList<double> Point, int NumObj);
    }
}