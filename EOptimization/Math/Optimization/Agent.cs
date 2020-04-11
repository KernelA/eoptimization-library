// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.Optimization
{
    using System;

    using EOpt.Exceptions;

    using TMOTargetFunction = System.Func<System.Collections.Generic.IReadOnlyList<double>, System.Collections.Generic.IEnumerable<double>>;
    using TOOTargetFunction = System.Func<System.Collections.Generic.IReadOnlyList<double>, double>;

    public class Agent : IEquatable<Agent>
    {
        private PointND _point, _objs;

        /// <summary>
        /// </summary>
        public PointND Objs => _objs;

        /// <summary>
        /// </summary>
        public PointND Point => _point;

        public Agent(PointND Point, PointND Objectives)
        {
            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            if (Objectives == null)
            {
                throw new ArgumentNullException(nameof(Objectives));
            }

            _point = Point;
            _objs = Objectives;
        }

        public Agent(int DimPoint, int DimObjs) : this(new PointND(0.0, DimPoint), new PointND(0.0, DimObjs))
        {
        }

        public Agent DeepCopy() => new Agent(Point.DeepCopy(), Objs.DeepCopy());

        public bool Equals(Agent Other)
        {
            if (Other == null)
            {
                return false;
            }

            return Point.Equals(Other.Point) && Objs.Equals(Other.Objs);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Agent);
        }

        public void Eval(TMOTargetFunction Function)
        {
            int position = 0;

            DoubleTypeValue typeValue;

            foreach (double value in Function(_point))
            {
                typeValue = CheckDouble.GetTypeValue(value);

                if (typeValue != DoubleTypeValue.Valid)
                {
                    throw new InvalidValueFunctionException($"An invalid value ({typeValue.ToString()}) of the {position}th function at point.\n{_point.ToString()}", _point);
                }

                _objs[position] = value;

                position++;
            }
        }

        public void Eval(TOOTargetFunction Function)
        {
            double value = Function(_point);

            DoubleTypeValue typeValue;

            typeValue = CheckDouble.GetTypeValue(value);

            if (typeValue != DoubleTypeValue.Valid)
            {
                throw new InvalidValueFunctionException($"An invalid value ({typeValue.ToString()}) of the function at point.\n{_point.ToString()}", _point);
            }

            _objs[0] = value;
        }

        public override int GetHashCode() => _point.GetHashCode() ^ _objs.GetHashCode();

        public void SetAt(Agent Agent)
        {
            Point.SetAt(Agent.Point);
            Objs.SetAt(Agent.Objs);
        }
    }
}