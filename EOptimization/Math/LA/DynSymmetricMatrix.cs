// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Math.LA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Symmetric matrix. 
    /// </summary>
    public class DynSymmetricMatrix
    {
        private const string InvalidSizeMessage = "Row or column count must be greater than 0.";

        private const string NotSymmetricMatrixMessage = "Matrix must be symmetric.";

        private const string SqrMatrixMessage = "Matrix must be square.";

        private List<double> _elements;
       
        private int _size;

        /// <summary>
        /// <para>
        ///  Transformation two-dimension index to one-dimension for determine position element in the array. 
        /// </para>
        /// <para>
        /// Matrix. (* - it is element of the matrix). 
        ///   0 1 2 3 (indices)
        /// 0 * 
        /// 1 * * 
        /// 2 * * * 
        /// 3 * * * *
        /// Linearization: [*] [* *] [* * *] [* * * *] (0, 0) transform to 0; (1, 0) transform to 1;
        /// (1, 1) transform to 2; (2, 2) transform to 5; (i, j) transform to i * (i + 1) / 2 + j.
        /// The formula works for i &gt;= j.
        /// </para>
        /// </summary>
        /// <param name="RowIndex">   </param>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        private int GetIndexInArray(int RowIndex, int ColumnIndex)
        {
            return RowIndex * (RowIndex + 1) / 2 + ColumnIndex;
        }

        /// <summary>
        /// Get column count of matrix. 
        /// </summary>
        public int ColumnCount
        {
            get => _size;

            set
            {
                if (value < 1)
                    throw new ArgumentException(InvalidSizeMessage);

                if(value < ColumnCount)
                {
                    int countToDel = ((value + 1 + ColumnCount) * (ColumnCount - value)) / 2;
                    _elements.RemoveRange(GetIndexInArray(value + 1, 0), countToDel);
                }
                else if(value > ColumnCount)
                {
                    int countToAdd = ((ColumnCount + 1 + value) * (value - ColumnCount)) / 2;
                    _elements.AddRange(Enumerable.Repeat(0.0, countToAdd));
                }

                _size = value;
            }
        }


        /// <summary>
        /// Get row count of matrix. 
        /// </summary>
        public int RowCount
        {
            get => _size;

            set
            {
                ColumnCount = value;
            }
        }

        /// <summary>
        /// Create a symmetric matrix. Row and column count are equal <paramref name="Size"/>.
        /// Elements of matrix are equal default value 0.
        /// </summary>
        /// <param name="Size"> Row and column count. </param>
        /// <exception cref="ArgumentException"> If <paramref name="Size"/> &lt; 1. </exception>
        public DynSymmetricMatrix(int Size) : this(Size, 0)
        {
        }

        /// <summary>
        /// Create symmetric matrix. Row and column count equal <paramref name="Size"/>. Elements of
        /// matrix are equal <paramref name="DefaultValue"/>.
        /// </summary>
        /// <param name="Size">         Row and column count. </param>
        /// <param name="DefaultValue"> A default value for elements of matrix. </param>
        /// <exception cref="ArgumentException"> If <paramref name="Size"/> &lt; 1. </exception>
        public DynSymmetricMatrix(int Size, double DefaultValue)
        {
            if (Size < 1)
                throw new ArgumentException(InvalidSizeMessage, nameof(Size));

            _size = Size;

            _elements = new List<double>((_size + 1) * _size / 2);

            _elements.AddRange(Enumerable.Repeat(DefaultValue, _elements.Capacity));
        }

        /// <summary>
        /// Create symmetric matrix from array <paramref name="Elements"/>. 
        /// </summary>
        /// <param name="Elements"></param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="Elements"/> is not squared matrix or symmetric matrix.
        /// </exception>
        /// <exception cref="ArgumentNullException"> If <paramref name="Elements"/> is null. </exception>
        public DynSymmetricMatrix(double[,] Elements)
        {
            if (Elements == null)
            {
                throw new ArgumentNullException(nameof(Elements));
            }
            if (Elements.GetLength(0) != Elements.GetLength(1))
                throw new ArgumentException(SqrMatrixMessage, nameof(Elements));

            _size = Elements.GetLength(0);

            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (Elements[i, j] != Elements[j, i])
                        throw new ArgumentException(NotSymmetricMatrixMessage, nameof(Elements));
                }
            }

            _elements = new List<double>((_size + 1) * _size / 2);

            _elements.AddRange(Enumerable.Repeat(0.0, _elements.Capacity));

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
        /// <param name="RowIndex">   </param>
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
                    return _elements[GetIndexInArray(ColumnIndex, RowIndex)];
                else
                    return _elements[GetIndexInArray(RowIndex, ColumnIndex)];
            }
            set
            {
                if (RowIndex < ColumnIndex)
                    _elements[GetIndexInArray(ColumnIndex, RowIndex)] = value;
                else
                    _elements[GetIndexInArray(RowIndex, ColumnIndex)] = value;
            }
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