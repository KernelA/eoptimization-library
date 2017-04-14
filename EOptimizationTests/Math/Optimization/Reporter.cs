namespace EOpt.Math.Optimization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class TestReporter : IProgress<Tuple<object, int, int, int>>
    {
        public bool Error { get; private set; } = false;

        private int iterMin, iterMax;

        private Type optimizerType;

        public TestReporter(Type OptimizaerType, int IterMin, int IterMax)
        {
            iterMin = IterMin;
            iterMax = IterMax;
            optimizerType = OptimizaerType;
        }

        public void Report(Tuple<object, int, int, int> Progress)
        {
            if (!typeof(object).IsInstanceOfType(optimizerType) || iterMin != Progress.Item2 || iterMax != Progress.Item3)
                Error = true;
            if (Progress.Item4 < iterMin || Progress.Item4 > iterMax)
                Error = true;             
        }

    }
}
