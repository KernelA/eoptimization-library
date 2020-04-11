// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization.MOOpt
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using EOpt.Help;
    using EOpt.Math;
    using EOpt.Math.LA;
    using EOpt.Math.Random;

    using Nds;

    public class MOBBBCOptimizer : BBBBC<IEnumerable<double>, IMOOptProblem>, IMOOptimizer<BBBCParams>
    {
        private PointND _centerOfMass;

        private Ndsort<double> _nds;

        private Random _rand;

        private DynSymmetricMatrix _distances;

        private int _nearestAgentsCount;

        private class IdxDistance : IComparable<IdxDistance>
        {
            public int Index { get; set; }

            public double Distance { get; set; }

            public IdxDistance(int Index, double Dist)
            {
                this.Index = Index;
                this.Distance = Dist;
            }

            public int CompareTo(IdxDistance Other)
            {
                return Distance.CompareTo(Other.Distance);
            }
        }

        private void EvalFunction(Func<IReadOnlyList<double>, IEnumerable<double>> Function)
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                _agents[i].Eval(Function);
            }
        }

        protected override void Clear()
        {
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

        private void GenerateNextAgents(IMOOptProblem Problem, int IterNum)
        {
            int[] fronts = _nds.NonDominSort(_agents, agent => agent.Objs);

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

            LinkedList<int> allIndices = new LinkedList<int>();

            foreach (int front in frontIndicesDict.Keys)
            {
                if (allIndices.Count <= 0.3 * _parameters.NP)
                {
                    for (int i = 0; i < frontIndicesDict[front].Count; i++)
                    {
                        allIndices.AddLast(frontIndicesDict[front][i]);
                    }
                }
            }

            List<int> indicesFirstFront = frontIndicesDict[0];

            //_idealPoint.SetAt(_agents[indicesFirstFront[0]].Objs);

            //foreach (int indexFirstFront in indicesFirstFront)
            //{
            //    for (iFt j = 0; j < _agents[indexFirstFront].Objs.Count; j++)
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

        protected override void FirstStep(IMOOptProblem Problem)
        {
            InitAgents(Problem, Problem.CountObjs);
            EvalFunction(Problem.TargetFunction);
        }

        protected override void Init(BBBCParams Parameters, IMOOptProblem Problem, int DimObjs)
        {
            base.Init(Parameters, Problem, DimObjs);
            _distances = new DynSymmetricMatrix(Parameters.NP);
        }

        protected override void InitAgents(IMOOptProblem Problem, int DimObjs)
        {
            int dimPoint = Problem.LowerBounds.Count;

            base.InitAgents(Problem, DimObjs);

            if (_centerOfMass == null)
            {
                _centerOfMass = new PointND(0.0, dimPoint);
            }
            else if (_centerOfMass.Count != dimPoint)
            {
                _centerOfMass = new PointND(0.0, dimPoint);
            }
        }

        protected override void NextStep(IMOOptProblem Problem, int Iter)
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
            _nds = new Ndsort<double>(CmpDouble.DoubleCompare);
            _nearestAgentsCount = 5;
        }

        public override void Minimize(BBBCParams Parameters, IMOOptProblem Problem)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            Init(Parameters, Problem, Problem.CountObjs);

            FirstStep(Problem);

            for (int i = 2; i <= _parameters.Imax; i++)
            {
                NextStep(Problem, i);
            }
        }

        private void ComputeDistances()
        {
            for (int i = 0; i < _parameters.NP; i++)
            {
                for (int j = i + 1; j < _parameters.NP; j++)
                {
                    _distances[i, j] = PointND.Distance(_agents[i].Point, _agents[j].Point);
                }
            }
        }

        private int[] FindNearestForAgent(int IndexAgent, IReadOnlyCollection<int> IndicesOtherAgents)
        {
            IdxDistance[] nearestAgents = new IdxDistance[IndicesOtherAgents.Count];

            int j = 0;

            foreach (int index in IndicesOtherAgents)
            {
                nearestAgents[j].Index = index;
                nearestAgents[j++].Distance = _distances[IndexAgent, index];
            }

            Array.Sort(nearestAgents);

            int[] indicesNearest = new int[_nearestAgentsCount];

            for (int i = 0; i < indicesNearest.Length; i++)
            {
                indicesNearest[i] = nearestAgents[i].Index;
            }

            return indicesNearest;
        }

        public override void Minimize(BBBCParams Parameters, IMOOptProblem Problem, CancellationToken cancelToken) => throw new NotImplementedException();

        public override void Minimize(BBBCParams Parameters, IMOOptProblem Problem, IProgress<Progress> Reporter)
        {
            if (Problem == null)
            {
                throw new ArgumentNullException(nameof(Problem));
            }

            Init(Parameters, Problem, Problem.CountObjs);

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

        public override void Minimize(BBBCParams Parameters, IMOOptProblem Problem, IProgress<Progress> Reporter, CancellationToken CancelToken) => throw new NotImplementedException();
    }
}