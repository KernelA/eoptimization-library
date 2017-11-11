// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.LA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Symmetric matrix.
    /// </summary>
    public class SymmetricMatrix
    {

        private const string InvalidSizeMessage = "Row count must be grater than 0.";

        private const string SqrMatrixMessage = "Matrix must be square.";

        private const string NotSymmetricMatrixMessage = "Matrix must be symmetric.";

        private double[] elements;

        private int size;

        /// <summary>
        /// Get row count of matrix.
        /// </summary>
        public int RowCount => size;

        /// <summary>
        /// Get column count of matrix.
        /// </summary>
        public int ColumnCount => size;

        /// <summary>
        /// Create a symmetric matrix. Row and column count are equal <paramref name="Size"/>. Elements of matrix are equal default value 0.
        /// </summary>
        /// <param name="Size">Row and column count.</param>
        /// <exception cref="ArgumentException">If <paramref name="Size"/> &lt;  1.</exception>
        public SymmetricMatrix(int Size) : this(Size, 0)
        {
        }

        /// <summary>
        /// Create symmetric matrix. Row and column count equal  <paramref name="Size"/>. Elements of matrix are equal <paramref name="DefaultValue"/>.
        /// </summary>
        /// <param name="Size">Row and column count.</param>
        /// <param name="DefaultValue">A default value for elements of matrix.</param>
        /// <exception cref="ArgumentException">If <paramref name="Size"/> &lt;  1.</exception>
        public SymmetricMatrix(int Size, double DefaultValue)
        {
            if (Size < 1)
                throw new ArgumentException(InvalidSizeMessage, nameof(Size));

            size = Size;

            elements = new double[(size + 1) * size / 2];

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = DefaultValue;
            }
        }

        /// <summary>
        /// Create symmetric matrix from array <paramref name="Elements"/>.
        /// </summary>
        /// <param name="Elements"></param>
        /// <exception cref="ArgumentException">If <paramref name="Elements"/> is not squared matrix or symmetric matrix.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="Elements"/> is null.</exception>
        public SymmetricMatrix(double[,] Elements)
        {

            if (Elements == null)
            {
                throw new ArgumentNullException(nameof(Elements));
            }
            if (Elements.GetLength(0) != Elements.GetLength(1))
                throw new ArgumentException(SqrMatrixMessage, nameof(Elements));
      
            size = Elements.GetLength(0);

            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (Elements[i, j] != Elements[j, i])
                        throw new ArgumentException(NotSymmetricMatrixMessage, nameof(Elements));
                }
            }

            elements = new double[(size + 1) * size / 2];


            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    this[i, j] = Elements[i, j];
                }
            }

        }

        /// <summary>
        /// Get or set value of element of the matrix.
        /// </summary>
        /// <param name="RowIndex"></param>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        public double this[int RowIndex, int ColumnIndex]
        {
            // Stored only a part of the matrix.
            // Matrix: (* - it is element of the matrix).
            //           0 1 2 ... n - 1 (column index)
            //       0   *
            //       1   * *
            //       2   * * *
            //           ...
            //       n-1 * * * *  *
            //(row index) 

            get
            {
                // If the element is above main diagonal then returned equal symmetric element. 
                if (RowIndex < ColumnIndex)
                    return elements[GetIndexInArray(ColumnIndex, RowIndex)];
                else
                    return elements[GetIndexInArray(RowIndex, ColumnIndex)];
            }
            set
            {
                if (RowIndex < ColumnIndex)
                    elements[GetIndexInArray(ColumnIndex, RowIndex)] = value;
                else
                    elements[GetIndexInArray(RowIndex, ColumnIndex)] = value;
            }
        }

        /// <summary>
        /// Transformation two-dimension index to one-dimension for determine position element in the array.
        /// Matrix. (* - it is element of the matrix).
        ///   0 1 2 3
        /// 0 *
        /// 1 * *
        /// 2 * * *
        /// 3 * * * *
        /// Linearization: 
        /// [*]  [* *] [* * *] [* * * *] 
        /// (0, 0) transform to  0;
        /// (1, 0) transform to  1;
        /// (1, 1) transform to  2;
        /// (2, 2) transform to  5;
        /// (i, j) transform to i * (i + 1) / 2 + j.
        /// The formula works for i >= j.
        /// </summary>
        /// <param name="RowIndex"></param>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        private int GetIndexInArray(int RowIndex, int ColumnIndex)
        {
            return RowIndex * (RowIndex + 1) / 2 + ColumnIndex;
        }

       /// <summary>
       /// Copy matrix into two-dimensional array.
       /// </summary>
       /// <returns></returns>
        public double[,] ToArray()
        {
            double[,] matrix = new double[RowCount, RowCount];

            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    matrix[i, j] = this[i, j];
                }
            }

            return matrix;
        }

    }
}
