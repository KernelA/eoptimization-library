// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IConstrOptProblem<TDecision, TObj>
    {
        IReadOnlyList<TDecision> LowerBounds { get; }

        IReadOnlyList<TDecision> UpperBounds { get; }

        TObj TargetFunction(IReadOnlyList<TDecision> Point);
    }
}
