namespace EOpt.Math.Optimization.Tests
{
    using System;

    class TestReporter : IProgress<Progress>
    {
        public bool Error { get; private set; } = false;

        private int iterMin, iterMax;

        private Type optimizerType;

        public TestReporter(Type OptimizerType, int IterMin, int IterMax)
        {
            iterMin = IterMin;
            iterMax = IterMax;
            optimizerType = OptimizerType;
        }

        public void Report(Progress Progress)
        {
            if (typeof(object).Equals(optimizerType) || iterMin != Progress.Start || iterMax != Progress.End)
                Error = true;
            if (Progress.Current < iterMin || Progress.Current > iterMax)
                Error = true;             
        }

    }
}
