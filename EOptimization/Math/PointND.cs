// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Class is representing a point in N dimension space.
    /// </summary>
    public class PointND : IEquatable<PointND>
    {
        private const string NotEqualDimMessage = "The number of coordinates is unequal.";

        /// <summary>
        /// Coordinates of point.
        /// </summary>
        private double[] coordinates;

        /// <summary>
        /// Coordinates of point. Only for read.
        /// </summary>
        public ReadOnlyCollection<double> Coordinates => new ReadOnlyCollection<double>(coordinates);


        /// <summary>
        /// Number of coordinates.
        /// </summary>
        public int Dimension => coordinates.Length;


        /// <summary>
        /// Create point with number of coordinates is equal  <paramref name="Dimension"/> and value is <paramref name="DefaultValue"/>.
        /// </summary>
        /// <param name="DefaultValue">The value of the coordinate.</param>
        /// <param name="Dimension">Number of coordinates.</param>
        /// <exception cref="ArgumentException">If <paramref name="Dimension"/> &lt; 1.</exception>
        public PointND(double DefaultValue, int Dimension)
        {
            if (Dimension < 1)
                throw new ArgumentException($"{nameof(Dimension)} must be > 0.", nameof(Dimension));

            coordinates = new double[Dimension];

            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = DefaultValue;
            }
        }

        /// <summary>
        /// Create point from array <paramref name="Coordinates"/>.
        /// </summary>
        /// <param name="Coordinates">Array of coordinates.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="Coordinates"/> is null.</exception>
        public PointND(double[] Coordinates)
        {
            if (Coordinates == null)
                throw new ArgumentNullException(nameof(Coordinates));
            coordinates = new double[Coordinates.Length];

            Coordinates.CopyTo(coordinates, 0);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param
        /// <returns></returns>
        public override bool Equals(object obj) => obj is PointND ? Equals((PointND)obj) : false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => coordinates.GetHashCode();
   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        public bool Equals(PointND Point)
        {
            if (Point == null)
                return false;

            if (this.Dimension != Point.Dimension)
               return false;

            bool isEqual = true;

            for (int i = 0; i < Dimension; i++)
            {
                if(coordinates[i] != Point.coordinates[i])
                {
                    isEqual = false;
                    break;
                }
            }

            return isEqual;
        }

        /// <summary>
        /// Create a deep copy.
        /// </summary>
        /// <returns></returns>
        public PointND DeepCopy() => new PointND(coordinates);


        /// <summary>
        /// Get <paramref name="i"/>-th coordinate.
        /// </summary>
        /// <param name="i">Index of coordinate.</param>
        /// <returns></returns>
        public double this[int i]
        {
            get
            {
                return this.coordinates[i];
            }

            set
            {
                this.coordinates[i] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string cords = "(";

            int length = this.Dimension - 1;

            for (int i = 0; i < length; i++)
            {
                cords += this[i].ToString() + "; ";
            }

            return $"{cords} {this[length].ToString()})";
        }

        /// <summary>
        /// Copy coordinates from <paramref name="Point"/>.
        /// </summary>
        /// <param name="Point"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="Point"/> is null.</exception>
        /// <exception cref="ArgumentException">If dimensions are not equal.</exception>
        public void SetAt(PointND Point)
        {

            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            if (Dimension != Point.Dimension)
                throw new ArgumentException(NotEqualDimMessage, nameof(Point));

            for(int i= 0; i < Dimension; i++)
            {
                this[i] = Point[i];
            }
        }

        /// <summary>
        /// To all coordinates of point add coordinates of <paramref name="Point"/>. This is making inplace.
        /// </summary>
        /// <param name="Point"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="Point"/> is null.</exception>
        /// <exception cref="ArgumentException">If dimensions are not equal.</exception>
        public void AddInplace(PointND Point)
        {

            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            if (Dimension != Point.Dimension)
                throw new ArgumentException(NotEqualDimMessage, nameof(Point));

            for (int i = 0; i < Dimension; i++)
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
            for (int i = 0; i < Dimension; i++)
            {
                this[i] += Value;
            }
        }

        /// <summary>
        /// All coordinates multiply by <paramref name="Value"/>. This is making inplace.
        /// </summary>
        /// <param name="Value"></param>
        public void MultiplyByInplace(double Value)
        {
            for (int i = 0; i < Dimension; i++)
            {
                this[i] *= Value;
            }
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

            if (Point1.Dimension != Point2.Dimension)
                throw new ArgumentException(NotEqualDimMessage);

            PointND temp = new PointND(0, Point1.Dimension);

            for (int i = 0; i < Point1.Dimension; i++)
            {
                temp[i] = Point1[i] + Point2[i];
            }

            return temp;
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

            if (Point1.Dimension != Point2.Dimension)
                throw new ArgumentException(NotEqualDimMessage);

            PointND temp = new PointND(0, Point1.Dimension);

            for (int i = 0; i < Point1.Dimension; i++)
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
        /// <exception cref="ArgumentNullException">If <paramref name="Point"/> is null.</exception>
        public static PointND operator -(PointND Point)
        {
            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            PointND temp = new PointND(0, Point.Dimension);

            for (int i = 0; i < Point.Dimension; i++)
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
        /// <exception cref="ArgumentNullException">If <paramref name="Point"/>  is null.</exception>
        public static PointND operator *(PointND Point, double Value)
        {
            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            PointND temp = new PointND(0, Point.Dimension);

            for (int i = 0; i < Point.Dimension; i++)
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
        /// <exception cref="ArgumentNullException">If <paramref name="Point"/>  is null.</exception>
        public static PointND operator *(double Value, PointND Point)
        {

            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }

            PointND temp = new PointND(0, Point.Dimension);

            for (int i = 0; i < Point.Dimension; i++)
            {
                temp[i] = Point[i] * Value;
            }

            return temp;
        }

    } 
}