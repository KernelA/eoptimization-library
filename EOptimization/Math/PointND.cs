namespace EOpt.Math
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Point in N dimension space.
    /// </summary>
    public class PointND : IEquatable<PointND>
    {
        private const string NotEqualDimMessage = "Unequal the number of coordinates.";

        /// <summary>
        /// Coordinates of point.
        /// </summary>
        private double[] coordinates;

        /// <summary>
        /// Coordinates of point. Only for read.
        /// </summary>
        public ReadOnlyCollection<double> Coordinates
        {
            get
            {
                return new ReadOnlyCollection<double>(coordinates);
            }
        }

        /// <summary>
        /// Number of coordinates.
        /// </summary>
        public int Dimension
        {
            get
            {
                return coordinates.Length;
            }
        }

        /// <summary>
        /// Create point with number of coordinates equal  <paramref name="dimension"/> and value <paramref name="x"/>.
        /// </summary>
        /// <param name="x">The value of the coordinate.</param>
        /// <param name="dimension">Number of coordinates.</param>
        /// <exception cref="ArgumentException"></exception>
        public PointND(double x, int dimension)
        {
            if (dimension < 1)
                throw new ArgumentException($"{nameof(dimension)} must be > 0", nameof(dimension));

            coordinates = new double[dimension];
    
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = x;
            }
        }

        /// <summary>
        /// Create point from array <paramref name="x"/>.
        /// </summary>
        /// <param name="x">Array of coordinates.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PointND(double[] x)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            coordinates = new double[x.Length];

            x.CopyTo(coordinates, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is PointND ? Equals((PointND)obj) : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return coordinates.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Equals(PointND point)
        {
            if (point == null)
                return false;

            if (this.Dimension != point.Dimension)
               return false;

            bool equal = true;

            for (int i = 0; i < coordinates.Length; i++)
            {
                if(coordinates[i] != point.coordinates[i])
                {
                    equal = false;
                    break;
                }
            }

            return equal;
        }

        /// <summary>
        /// Create deep copy.
        /// </summary>
        /// <returns></returns>
        public PointND Clone()
        {
            PointND temp = new PointND(0, this.Dimension);

            for (int i = 0; i < this.Dimension; i++)
            {
                temp[i] = this[i];
            }

            return temp;
        }


        /// <summary>
        /// Get <paramref name="i"/> - th coordinate.
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

            int Length = this.Dimension - 1;

            for (int i = 0; i < Length; i++)
            {
                cords += this[i].ToString() + "; ";
            }

            return cords + this[Length].ToString() + ")";
        }

        ///<summary>
        /// Add two points.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static PointND operator +(PointND p1, PointND p2)
        {

            if (p1 == null)
            {
                throw new ArgumentNullException(nameof(p1));
            }

            if (p2 == null)
            {
                throw new ArgumentNullException(nameof(p2));
            }

            if (p1.Dimension != p2.Dimension)
                throw new ArgumentException(NotEqualDimMessage);

            PointND temp = new PointND(0, p1.Dimension);

            for (int i = 0; i < p1.Dimension; i++)
            {
                temp[i] = p1[i] + p2[i];
            }

            return temp;
        }

        ///<summary>
        /// Subtraction two points.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static PointND operator -(PointND p1, PointND p2)
        {
            if (p1 == null)
            {
                throw new ArgumentNullException(nameof(p1));
            }


            if (p2 == null)
            {
                throw new ArgumentNullException(nameof(p2));
            }

            if (p1.Dimension != p2.Dimension)
                throw new ArgumentException(NotEqualDimMessage);

            PointND temp = new PointND(0, p1.Dimension);

            for (int i = 0; i < p1.Dimension; i++)
            {
                temp[i] = p1[i] - p2[i];
            }

            return temp;
        }

        /// <summary>
        /// Multiplication by -1 each coordinate.
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PointND operator -(PointND p1)
        {
            if (p1 == null)
            {
                throw new ArgumentNullException(nameof(p1));
            }

            PointND temp = new PointND(0, p1.Dimension);

            for (int i = 0; i < p1.Dimension; i++)
            {
                temp[i] = -p1[i];
            }

            return temp;
        }

        /// <summary>
        /// Multiplication by <paramref name="a"/>.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PointND operator *(PointND p1, double a)
        {
            if (p1 == null)
            {
                throw new ArgumentNullException(nameof(p1));
            }

            PointND temp = new PointND(0, p1.Dimension);

            for (int i = 0; i < p1.Dimension; i++)
            {
                temp[i] = p1[i] * a;
            }

            return temp;
        }

        /// <summary>
        /// Multiplication by <paramref name="a"/>. 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PointND operator *(double a, PointND p1)
        {

            if (p1 == null)
            {
                throw new ArgumentNullException(nameof(p1));
            }

            PointND temp = new PointND(0, p1.Dimension);

            for (int i = 0; i < p1.Dimension; i++)
            {
                temp[i] = p1[i] * a;
            }

            return temp;
        }


        /// <summary>
        /// Euclidean distance between two points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static double EuclidianDistance(PointND p1, PointND p2)
        {
            if (p1 == null)
            {
                throw new ArgumentNullException(nameof(p1));
            }

            if (p2 == null)
            {
                throw new ArgumentNullException(nameof(p2));
            }

            if (p1.Dimension != p2.Dimension)
                throw new ArgumentException(NotEqualDimMessage);

            double distance = 0;

            for (int i = 0; i < p1.Dimension; i++)
            {
                distance += (p1[i] - p2[i]) * (p1[i] - p2[i]);
            }

            return Math.Sqrt(distance);
        }


    } 
}