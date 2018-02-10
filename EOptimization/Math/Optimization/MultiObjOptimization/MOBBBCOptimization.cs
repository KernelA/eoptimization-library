// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.MOOpt
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using EOpt.Help;
    using EOpt.Math;
    using EOpt.Math.Random;

    using Nds;

    public class MOBBBCOptimizer : BBBBC<MOOptimizationProblem>, IMOOptimizer<BBBCParams>
    {
        private PointND _centerOfMass;

        private Random _rand;

        private void EvalFunction(IEnumerable<Func<IReadOnlyList<double>, double>> Functions)
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                _agents[i].Eval(Functions);
            }
        }

        private void FindCenterOfMass(IEnumerable<int> IndicesCurrentFront)
        {
            double denominator = 0.0;
            double temp = 0.0;

            _centerOfMass.MultiplyByInplace(0);

            int length = 0;

            foreach (int index in IndicesCurrentFront)
            {
                //temp = PointND.Distance(_agents[index].Objs, _idealPoint, 1);

                //if (temp < Constants.VALUE_AVOID_DIV_BY_ZERO)
                //{
                //    temp += Constants.VALUE_AVOID_DIV_BY_ZERO;
                //}

                //temp = 1 / temp;

                //denominator += temp;

                temp = 1;

                length++;

                for (int coordIdx = 0; coordIdx < _centerOfMass.Count; coordIdx++)
                {
                    _centerOfMass[coordIdx] += temp * _agents[index].Point[coordIdx];
                }
            }

            _centerOfMass.MultiplyByInplace(1.0 / length);
        }

        private void GenerateNextAgents(MOOptimizationProblem Problem, int IterNum)
        {
            int[] fronts = Ndsort.NonDominSort(_agents, agent => agent.Objs);

            Dictionary<int, List<int>> frontIndicesDict = new Dictionary<int, List<int>>();

            for (int i = 0; i < fronts.Length; i++)
            {
                if (frontIndicesDict.ContainsKey(fronts[i]))
                {
                    frontIndicesDict[fronts[i]].Add(i);
                }
                else
                {
                    frontIndicesDict.Add(fronts[i], new List<int>() { i });
                }
            }

            List<int> indicesFirstFront = frontIndicesDict[0];

            //_idealPoint.SetAt(_agents[indicesFirstFront[0]].Objs);

            //foreach (int indexFirstFront in indicesFirstFront)
            //{
            //    for (int j = 0; j < _agents[indexFirstFront].Objs.Count; j++)
            //    {
            //        if (_agents[indexFirstFront].Objs[j] < _idealPoint[j])
            //        {
            //            _idealPoint[j] = _agents[indexFirstFront].Objs[j];
            //        }
            //    }
            //}

            FindCenterOfMass(frontIndicesDict[0]);

            for (int front = 1; front < frontIndicesDict.Count; front++)
            {
                //if(frontIndicesDict[front - 1].Count == 1)
                //{
                //    int indexBestSol = frontIndicesDict[front - 1].First();

                // foreach (int index in frontIndicesDict[front]) { for (int j = 0; j <
                // Problem.LowerBounds.Count; j++) { _agents[index].Point[j] =
                // _agents[indexBestSol].Point[j] + _normalRand.NRandVal(0, 1) * _parameters.Alpha *
                // (Problem.UpperBounds[j] - Problem.LowerBounds[j]) / IterNum;

                //            // If point leaves region then it returns to random point.
                //            if (_agents[index].Point[j] < Problem.LowerBounds[j] || _agents[index].Point[j] > Problem.UpperBounds[j])
                //            {
                //                _agents[index].Point[j] = _uniformRand.URandVal(Problem.LowerBounds[j], Problem.UpperBounds[j]);
                //            }
                //            //_agents[index].Point[j] = Math.Max(Math.Min(_agents[index].Point[j], Problem.LowerBounds[j]), Problem.UpperBounds[j]);
                //        }
                //    }
                //}
                //else
                //{
                foreach (int index in frontIndicesDict[front])
                {
                    int indexBestSol = Сombinatorics.RandomChoice(frontIndicesDict[0], SyncRandom.Get());

                    //FindCenterOfMass(frontIndicesDict[front - 1]);

                    for (int j = 0; j < Problem.LowerBounds.Count; j++)
                    {
                        _agents[index].Point[j] = _parameters.Beta * _centerOfMass[j] + (1 - _parameters.Beta) * _agents[indexBestSol].Point[j] + _normalRand.NRandVal(0, 1) * _parameters.Alpha * (Problem.UpperBounds[j] - Problem.LowerBounds[j]) / IterNum;

                        // If point leaves region then it returns to random point.
                        //if (_agents[index].Point[j] < Problem.LowerBounds[j] || _agents[index].Point[j] > Problem.UpperBounds[j])
                        //{
                        //    _agents[index].Point[j] = _uniformRand.URandVal(Problem.LowerBounds[j], Problem.UpperBounds[j]);
                        //}

                        _agents[index].Point[j] = ClampDouble.Clamp(_agents[index].Point[j], Problem.LowerBounds[j], Problem.UpperBounds[j]);

                        //if (_agents[index].Point[j] < Problem.LowerBounds[j])
                        //{
                        //    _agents[index].Point[j] = Problem.LowerBounds[j];
                        //}
                        //else if (_agents[index].Point[j] > Problem.UpperBounds[j])
                        //{
                        //    _agents[index].Point[j] = Problem.UpperBounds[j];
                        //}
                        //_agents[index].Point[j] = Math.Max(Math.Min(_agents[index].Point[j], Problem.LowerBounds[j]), Problem.UpperBounds[j]);
                    }
                }
            }
        }

        protected override void FirstStep(MOOptimizationProblem Problem)
        {
            InitAgents(Problem.LowerBounds, Problem.UpperBounds, Problem.TargetFunction.Count);
            EvalFunction(Problem.TargetFunction);
        }

        protected override void Init(BBBCParams Parameters, int Dimension, int DimObjs)
        {
            base.Init(Parameters, Dimension, DimObjs);
        }

        protected override void InitAgents(IReadOnlyList<double> LowerBounds, IReadOnlyList<double> UpperBounds, int DimObjs)
        {
            int dimPoint = LowerBounds.Count;

            base.InitAgents(LowerBounds, UpperBounds, DimObjs);

            if (_centerOfMass == null)
            {
                _centerOfMass = new PointND(0.0, dimPoint);
            }
            else if (_centerOfMass.Count != dimPoint)
            {
                _centerOfMass = new PointND(0.0, dimPoint);
            }
        }

        protected override void NextStep(MOOptimizationProblem Problem, int Iter)
        {
            GenerateNextAgents(Problem, Iter);
            EvalFunction(Problem.TargetFunction);
        }

        public IEnumerable<Agent> ParetoFront => base._agents;

        /// <summary>
        /// Create the object which uses default implementation for random generators. 
        /// </summary>
        public MOBBBCOptimizer() : this(new ContUniformDist(), new NormalDist())
        {
        }

        /// <summary>
        /// Create the object which uses custom implementation for random generators. 
        /// </summary>
        /// <param name="UniformGen"> Object, which implements <see cref="IContUniformGen"/> interface. </param>
        /// <param name="NormalGen">  Object, which implements <see cref="INormalGen"/> interface. </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="NormalGen"/> or <paramref name="UniformGen"/> is null.
        /// </exception>
        public MOBBBCOptimizer(IContUniformGen UniformGen, INormalGen NormalGen) : base(UniformGen, NormalGen)
        {
            _rand = SyncRandom.Get();
        }

        public override void Minimize(BBBCParams Parameters, MOOptimizationProblem Problem)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            _parameters = Parameters;

            FirstStep(Problem);

            for (int i = 2; i <= _parameters.Imax; i++)
            {
                NextStep(Problem, i);
            }
        }

        public override void Minimize(BBBCParams Parameters, MOOptimizationProblem Problem, CancellationToken cancelToken) => throw new NotImplementedException();

        public override void Minimize(BBBCParams Parameters, MOOptimizationProblem Problem, IProgress<Progress> Reporter)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            _parameters = Parameters;

            Progress pr = new Progress(this, 1, _parameters.Imax, 1);

            FirstStep(Problem);

            Reporter.Report(pr);

            for (int i = 2; i <= _parameters.Imax; i++)
            {
                pr.Current = i;
                NextStep(Problem, i);
                Reporter.Report(pr);
            }
        }

        public override void Minimize(BBBCParams Parameters, MOOptimizationProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken) => throw new NotImplementedException();
    }
}