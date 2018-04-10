﻿namespace EOpt
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using EOpt.Math.Optimization;

    class AgenCreator : IAgentCreator
    {
        private int _dimPoint, _dimObjs;

        public AgenCreator(int DimPoimt, int DimObjs)
        {
            _dimPoint = DimPoimt;

            _dimObjs = DimObjs;
        }
        public Agent Create()
        {
            return new Agent(_dimPoint, _dimObjs);
        }
    }
}