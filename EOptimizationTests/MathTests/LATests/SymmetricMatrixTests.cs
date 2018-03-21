namespace EOpt.Math.LA.Tests
{
    using System;

    using EOpt.Math.LA;

    using Xunit;

    public class SymmetricMatrixTests
    {
        [Fact]
        public void SymmetricMatrixTestConstr()
        {
            DynSymmetricMatrix matrix = new DynSymmetricMatrix(4);

            Assert.True(matrix.RowCount == matrix.ColumnCount && matrix.RowCount == 4);
        }

        [Fact]
        public void SymmetricMatrixTestConstr1()
        {
            DynSymmetricMatrix matrix = new DynSymmetricMatrix(3, 5);

            bool error = false;

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (matrix[i, j] != 5)
                        error = true;
                }
            }

            Assert.False(error);
        }

        [Fact]
        public void SymmetricMatrixTestConstr2()
        {
            double[,] array = { { 1, 2, 4 }, { 2, 7, 8 }, { 4, 8, 9 } };

            DynSymmetricMatrix matrix = new DynSymmetricMatrix(array);

            bool error = false;

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (array[i, j] != matrix[i, j])
                        error = true;
                }
            }

            Assert.False(error);
        }

        [Fact]
        public void SymmetricMatrixTestIndexator()
        {
            DynSymmetricMatrix matrix = new DynSymmetricMatrix(5, 9);

            matrix[3, 4] = 10;

            Assert.True(matrix[4, 3] == matrix[3, 4]);
        }

        [Fact]
        public void SymmetricMatrixTestIndexator1()
        {
            DynSymmetricMatrix matrix = new DynSymmetricMatrix(5, 0);

            matrix[1, 1] = 2;

            Assert.True(matrix[1, 1] == 2);
        }

        [Fact]
        public void SymmetricMatrixTestToArray()
        {
            double[,] array = { { 1, 2 }, { 2, 8 } };

            DynSymmetricMatrix martix = new DynSymmetricMatrix(array);

            double[,] arrayRes = martix.ToArray();

            bool error = false;

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j] != arrayRes[i, j])
                        error = true;
                }
            }

            Assert.False(error);
        }

        [Fact]
        public void SymmetricMatrixTestWrongParamConstr()
        {
            // Not symmetrix matrix.
            double[,] array = { { 1, 2, 4 }, { 89, 7, 8 }, { 4, 8, 9 } };

            Assert.Throws<ArgumentException>(() => new DynSymmetricMatrix(array));
        }
    }
}