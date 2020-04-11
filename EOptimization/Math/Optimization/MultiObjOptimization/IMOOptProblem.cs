// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.MOOpt
{
    using System.Collections.Generic;

    public interface IMOOptProblem : IConstrOptProblem<double, IEnumerable<double>>
    {
        int CountObjs { get; }

        double ObjFunction(IReadOnlyList<double> Point, int NumObj);
    }
}