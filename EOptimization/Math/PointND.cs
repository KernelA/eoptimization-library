// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class is representing a point in N dimension space.
    /// </summary>
    public class PointND : IEquatable<PointND>, IReadOnlyList<double>
    {
        private const string NotEqualDimMessage = "The number of coordinates is unequal.";

        /// <summary>
        /// Coordinates of point.
        /// </summary>
        private double[] _coordinates;

        /// <summary>
        /// Number of coordinates.
        /// </summary>
        public int Count => _coordinates.Length;

        /// <summary>
        /// Create point with number of coordinates is equal <paramref name="Dimension"/> and value
        /// is <paramref name="DefaultValue"/>.
        /// </summary>
        /// <param name="DefaultValue"> The value of the coordinate. </param>
        /// <param name="Dimension">    Number of coordinates. </param>
        /// <exception cref="ArgumentException"> If <paramref name="Dimension"/> &lt; 1. </exception>
        public PointND(double DefaultValue, int Dimension)
        {
            if (Dimension < 1)
                throw new ArgumentException($"{nameof(Dimension)} must be > 0.", nameof(Dimension));

            _coordinates = new double[Dimension];

            for (int i = 0; i < _coordinates.Length; i++)
            {
                _coordinates[i] = DefaultValue;
            }
        }

        /// <summary>
        /// Create point from array <paramref name="Coordinates"/>.
        /// </summary>
        /// <param name="Coordinates"> Array of coordinates. </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="Coordinates"/> is null.
        /// </exception>
        public PointND(double[] Coordinates)
        {
            if (Coordinates == null)
            {
                throw new ArgumentNullException(nameof(Coordinates));
            }
            _coordinates = new double[Coordinates.Length];

            Coordinates.CopyTo(_coordinates, 0);
        }

        /// <summary>
        /// Get <paramref name="i"/>-th coordinate.
        /// </summary>
        /// <param name="i"> Index of coordinate. </param>
        /// <returns></returns>
        public double this[int i]
        {
            get => _coordinates[i];

            set => _coordinates[i] = value;
        }

        /// <summary>
        /// A distance between two points.
        /// </summary>
        /// <param name="Point1"></param>
        /// <param name="Point2"></param>
        /// <param name="P">      Parameter of the distance. </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="Point1"/> or <paramref name="Point2"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para> If <paramref name="Point1"/> and <paramref name="Point2"/> have unequal dimension. </para>
        /// <para> If <paramref name="P"/> less than 1. </para>
        /// </exception>
        public static double Distance(PointND Point1, PointND Point2, int P = 2)
        {
            if (Point1 == null)
            {
                throw new ArgumentNullException(nameof(Point1));
            }

            if (Point2 == null)
            {
                throw new ArgumentNullException(nameof(Point2));
            }

            if (Point1.Count != Point2.Count)
            {
                throw new ArgumentException(NotEqualDimMessage);
            }

            double diff, sum = 0.0, maxDiff;

            maxDiff = Math.Abs(Point1[0] - Point2[0]);

            for (int i = 1; i < Point1.Count; i++)
            {
                diff = Math.Abs(Point1[i] - Point2[i]);

                if (diff > maxDiff)
                {
                    maxDiff = diff;
                }
            }

            if (P == 1)
            {
                for (int i = 0; i < Point1.Count; i++)
                {
                    sum += Math.Abs(Point1[i] - Point2[i]) / maxDiff;
                }
            }
            else if (P == 2)
            {
                for (int i = 0; i < Point1.Count; i++)
                {
                    diff = (Point1[i] - Point2[i]) / maxDiff;

                    sum += diff * diff;
                }

                sum = Math.Sqrt(sum);
            }
            else if (P > 2)
            {
                for (int i = 0; i < Point1.Count; i++)
                {
                    sum += Math.Pow(Math.Abs(Point1[i] - Point2[i]) / maxDiff, P);
                }

                sum = Math.Pow(sum, 1.0 / P);
            }
            else
            {
                throw new ArgumentException($"{nameof(P)} must be >= 1.", nameof(P));
            }

            return maxDiff * sum;
        }

        ///<summary>
        /// Subtract two points.
        /// </summary>
        /// <exception cref="ArgumentException">If dimensions of <paramref name="Point1"/> is not equal dimension of <paramref name="Point2"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="Point1"/> or <paramref name="Point2"/> is null.</exception>
        public static PointND operator -(PointND Point1, PointND Point2)
        {
            if (Point1 == null)
            {
                throw new ArgumentNullException(nameof(Point1));
            }

            if (Point2 == null)
            {
                throw new ArgumentNullException(nameof(Point2));
            }

            if (Point1.Count != Point2.Count)
                throw new ArgumentException(NotEqualDimMessage);

            PointND temp = new PointND(0, Point1.Count);

            for (int i = 0; i < Point1.Count; i++)
            {
                temp[i] = Point1[i] - Point2[i];
            }

            return temp;
        }

        /// <summary>
        /// All coordinates multiply by -1.
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="Point"/> is null. </exception>
        public static PointND operator -(PointND Point)
        {
            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            PointND temp = new PointND(0, Point.Count);

            for (int i = 0; i < Point.Count; i++)
            {
                temp[i] = -Point[i];
            }

            return temp;
        }

        /// <summary>
        /// Multiplication by <paramref name="Value"/>.
        /// </summary>
        /// <param name="Point"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="Point"/> is null. </exception>
        public static PointND operator *(PointND Point, double Value)
        {
            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            PointND temp = new PointND(0, Point.Count);

            for (int i = 0; i < Point.Count; i++)
            {
                temp[i] = Point[i] * Value;
            }

            return temp;
        }

        /// <summary>
        /// Multiplication by <paramref name="Value"/>.
        /// </summary>
        /// <param name="Point"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="Point"/> is null. </exception>
        public static PointND operator *(double Value, PointND Point)
        {
            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            PointND temp = new PointND(0, Point.Count);

            for (int i = 0; i < Point.Count; i++)
            {
                temp[i] = Point[i] * Value;
            }

            return temp;
        }

        ///<summary>
        /// Add two points.
        /// </summary>
        /// <exception cref="ArgumentException">If dimensions of <paramref name="Point1"/> is not equal dimension of <paramref name="Point2"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="Point1"/> or <paramref name="Point2"/> is null.</exception>
        public static PointND operator +(PointND Point1, PointND Point2)
        {
            if (Point1 == null)
            {
                throw new ArgumentNullException(nameof(Point1));
            }

            if (Point2 == null)
            {
                throw new ArgumentNullException(nameof(Point2));
            }

            if (Point1.Count != Point2.Count)
                throw new ArgumentException(NotEqualDimMessage);

            PointND temp = new PointND(0, Point1.Count);

            for (int i = 0; i < Point1.Count; i++)
            {
                temp[i] = Point1[i] + Point2[i];
            }

            return temp;
        }

        /// <summary>
        /// To all coordinates of point add coordinates of <paramref name="Point"/>. This is making inplace.
        /// </summary>
        /// <param name="Point"></param>
        /// <exception cref="ArgumentNullException"> If <paramref name="Point"/> is null. </exception>
        /// <exception cref="ArgumentException"> If dimensions are not equal. </exception>
        public void AddInplace(PointND Point)
        {
            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            if (Count != Point.Count)
            {
                throw new ArgumentException(NotEqualDimMessage, nameof(Point));
            }

            for (int i = 0; i < Count; i++)
            {
                this[i] += Point[i];
            }
        }

        /// <summary>
        /// Add a <paramref name="Value"/> to the all coordinates. This is making inplace.
        /// </summary>
        /// <param name="Value"></param>
        public void AddInplace(double Value)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] += Value;
            }
        }

        /// <summary>
        /// Create a deep copy.
        /// </summary>
        /// <returns></returns>
        public PointND DeepCopy() => new PointND(_coordinates);

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            PointND temp = obj as PointND;

            return temp == null ? false : Equals(temp);
        }

        /// <summary>
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        public bool Equals(PointND Point)
        {
            if (Point == null)
                return false;

            if (Count != Point.Count)
                return false;

            bool isEqual = true;

            for (int i = 0; i < Count; i++)
            {
                if (_coordinates[i] != Point._coordinates[i])
                {
                    isEqual = false;
                    break;
                }
            }

            return isEqual;
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<double> GetEnumerator() => _coordinates.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _coordinates.GetEnumerator();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => _coordinates.GetHashCode();

        /// <summary>
        /// All coordinates multiply by <paramref name="Value"/>. This is making inplace.
        /// </summary>
        /// <param name="Value"></param>
        public void MultiplyByInplace(double Value)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] *= Value;
            }
        }

        /// <summary>
        /// LP norm.
        /// </summary>
        /// <param name="P"> Parameter of the norm. </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> If <paramref name="P"/> &lt; 1. </exception>
        public double Norm(int P = 2)
        {
            double max = Math.Abs(_coordinates[0]), temp = 0, sum = 0;

            for (int i = 1; i < _coordinates.Length; i++)
            {
                temp = Math.Abs(_coordinates[i]);

                if (temp > max)
                {
                    max = temp;
                }
            }

            if (max == 0.0)
            {
                return 0.0;
            }

            if (P == 1)
            {
                for (int i = 0; i < _coordinates.Length; i++)
                {
                    sum += Math.Abs(_coordinates[i]) / max;
                }
            }
            else if (P == 2)
            {
                for (int i = 0; i < _coordinates.Length; i++)
                {
                    temp = _coordinates[i] / max;
                    sum += temp * temp;
                }

                sum = Math.Sqrt(sum);
            }
            else if (P > 2)
            {
                for (int i = 0; i < _coordinates.Length; i++)
                {
                    sum += Math.Pow(_coordinates[i] / max, P);
                }

                sum = Math.Pow(sum, 1.0 / P);
            }
            else
            {
                throw new ArgumentException($"{nameof(P)} must be >= 1.", nameof(P));
            }

            return max * sum;
        }

        /// <summary>
        /// Copy coordinates from <paramref name="Point"/>.
        /// </summary>
        /// <param name="Point"></param>
        /// <exception cref="ArgumentNullException"> If <paramref name="Point"/> is null. </exception>
        /// <exception cref="ArgumentException"> If dimensions are not equal. </exception>
        public void SetAt(PointND Point)
        {
            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            if (Count != Point.Count)
            {
                throw new ArgumentException(NotEqualDimMessage, nameof(Point));
            }

            Point._coordinates.CopyTo(this._coordinates, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder coords = new StringBuilder("(");

            int length = Count - 1;

            for (int i = 0; i < length; i++)
            {
                coords.Append(this[i]);
                coords.Append(";");
            }

            coords.Append(this[length]);

            return coords.Append(")").ToString();
        }
    }
}