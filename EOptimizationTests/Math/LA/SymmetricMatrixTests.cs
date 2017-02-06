namespace EOpt.Math.LA.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EOpt.Math.LA;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass()]
    public class SymmetricMatrixTests
    {
        [TestMethod()]
        public void SymmetricMatrixTestConstr()
        {
            SymmetricMatrix matrix = new SymmetricMatrix(4);

            Assert.IsTrue(matrix.RowCount == matrix.ColumnCount && matrix.RowCount == 4);
        }

        [TestMethod()]
        public void SymmetricMatrixTestConstr1()
        {
            SymmetricMatrix matrix = new SymmetricMatrix(3, 5);

            bool error = false;

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (matrix[i, j] != 5)
                        error = true;
                }
            }

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void SymmetricMatrixTestConstr2()
        {
            double[,] array = { { 1, 2, 4 }, { 2, 7, 8 }, { 4, 8, 9 } };

            SymmetricMatrix matrix = new SymmetricMatrix(array);

            bool error = false;

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (array[i, j] != matrix[i, j])
                        error = true;
                }
            }

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void SymmetricMatrixTestToArray()
        {
            double[,] array = { { 1, 2 }, { 2, 8 } };

            SymmetricMatrix martix = new SymmetricMatrix(array);

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

            Assert.IsFalse(error);

        }

        [TestMethod()]
        public void SymmetricMatrixTestWrongParamConstr()
        {
            // Not symmetrix matrix.
            double[,] array = { { 1, 2, 4 }, { 89, 7, 8 }, { 4, 8, 9 } };

            bool error = true;
            try
            {
                SymmetricMatrix matrix = new SymmetricMatrix(array);

            }
            catch(ArgumentException exc)
            {
                error = false;
            }

            Assert.IsFalse(error);
        }

        [TestMethod()]
        public void SymmetricMatrixTestIndexator()
        {
            SymmetricMatrix matrix = new SymmetricMatrix(5, 9);

            matrix[3, 4] = 10;

            Assert.IsTrue(matrix[4, 3] == matrix[3, 4]);
        }

        [TestMethod()]
        public void SymmetricMatrixTestIndexator1()
        {
            SymmetricMatrix matrix = new SymmetricMatrix(5, 0);

            matrix[1, 1] = 2;

            Assert.IsTrue(matrix[1, 1] == 2);
        }
    }
}