// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.MOOpt
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct MOFWParams
    {
        private FWParams _parameters;

        private bool _isParamsInit;

        public FWParams FWParams => _parameters;

        public int NewStrategyIter { get; private set; }

        /// <summary>
        /// Minimum number of debris for each charge. 
        /// </summary>
        public int Smin => _parameters.Smin;

        /// <summary>
        /// The maximum amplitude of explosion. 
        /// </summary>
        public double Amax => _parameters.Amax;

        /// <summary>
        /// Maximum number of debris for each charge. 
        /// </summary>
        public int Smax => _parameters.Smax;

        /// <summary>
        /// The number of iteration. 
        /// </summary>
        public int Imax => -_parameters.Imax;

        /// <summary>
        /// A value indicates, if parameters are set or not. 
        /// </summary>
        public bool IsParamsInit => _isParamsInit && _parameters.IsParamsInit;



        /// <summary>
        /// Parameter affecting the number of debris. 
        /// </summary>
        public double M => _parameters.M;

        /// <summary>
        /// Number of charges on each iteration. 
        /// </summary>
        public int NP => _parameters.NP;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Parameters"></param>
        /// <param name="NewStrategyIter"></param>
        public MOFWParams(FWParams Parameters, int NewStrategyIter)
        {
            if(NewStrategyIter < 0 || NewStrategyIter > Parameters.Imax)
            {
                throw new ArgumentException($"{nameof(NewStrategyIter)} is invalid.", nameof(NewStrategyIter));
            }

            _isParamsInit = true;
            _parameters = Parameters;
            this.NewStrategyIter = NewStrategyIter;
        }
    }
}
