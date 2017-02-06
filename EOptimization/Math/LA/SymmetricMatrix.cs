namespace EOptimization.Math.LA
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
        /// Create symmetric matrix. Row and column count equal  <paramref name="Size"/>. Elements of matrix are equal default value 0.
        /// </summary>
        /// <param name="Size">Row and column count.</param>
        /// <exception cref="ArgumentException"></exception>
        public SymmetricMatrix(int Size) : this(Size, 0)
        {
        }

        /// <summary>
        /// Create symmetric matrix. Row and column count equal  <paramref name="Size"/>. Elements of matrix are equal <paramref name="DefaultValue"/>.
        /// </summary>
        /// <param name="Size">Row and column count.</param>
        /// <param name="DefaultValue">Default value for elements of matrix.</param>
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
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
        /// Get or set value of element of matrix.
        /// </summary>
        /// <param name="RowIndex"></param>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        public double this[int RowIndex, int ColumnIndex]
        {
            // We storing only part matrix. 
            // Matrix. (* - it is element of matrix).
            //     0 1 2 ... n - 1
            // 0   *
            // 1   * *
            // 2   * * *
            //     ...
            // n-1 * * * *  *
            get
            {
                // If element above main diagonal then return equal symmetric element. 
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
        /// Transformation two - dimensional index to one - dimensional for determine position element in array.
        /// Matrix. (* - it is element of matrix).
        ///   0 1 2 3
        /// 0 *
        /// 1 * *
        /// 2 * * *
        /// 3 * * * *
        /// linearizion to 
        /// [*]  [* *] [* * *] [* * * *] 
        /// (0, 0) transform to  0
        /// (1, 0) transform to  1
        /// (1, 1) transform to  2
        /// (2, 2) transform to  5
        /// (i, j) transform to i * (i + 1) / 2 + j;
        /// Formula works for i >= j.
        /// We have mapped index element of matrix and index in array.
        /// </summary>
        /// <param name="RowIndex"></param>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        private int GetIndexInArray(int RowIndex, int ColumnIndex)
        {
            return RowIndex * (RowIndex + 1) / 2 + ColumnIndex;
        }

       /// <summary>
       /// Copy matrix to two - dimensional array.
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
