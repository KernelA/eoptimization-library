namespace EOpt.Math.Optimization.MOOpt
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IMOOptProblem : IConstrOptProblem<double, IEnumerable<double>>
    {
        int CountObjs { get; }

        double ObjFunction(IReadOnlyList<double> Point, int NumObj);
    }
}
