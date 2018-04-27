using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace CCC_Linz18Spring
{
    public class Asteroid
    {
        public Image Image;
        public Matrix<double> Matrix;

        public Asteroid(Matrix<double> matrix, Image image)
        {
            Matrix = matrix;
            Image = image;
        }

        public Asteroid FromImage(Image img)
        {
            return img.GetAsteroid();
        }

        public bool EqualShape(Asteroid yourAsteroid)
        {
            return EqualShapeMatrix(Matrix, yourAsteroid.Matrix);
        }

        public static bool EqualShapeMatrix(Matrix<double> first, Matrix<double> second)
        {
            if (first.ColumnCount != second.ColumnCount
                || first.RowCount != second.RowCount)
            {
                // shape not equal
                return false;
            }

            for (var x = 0; x < first.RowCount; x++)
            {
                for (var y = 0; y < first.ColumnCount; y++)
                {
                    if (first[x, y] > 0 ^ second[x, y] > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Matrix<double> Rotate90(int k = 1)
        {
            return k == 0 ? Matrix : Rotate90(Matrix, k);
        }

        public static Matrix<double> Rotate90(Matrix<double> input, int k = 1)
        {
            if (k == 0)
                return input;

            Matrix<double> tmp = input;
            for (int i = 0; i < k; i++)
            {
                tmp = Rotate90Once(tmp);
            }

            return tmp;
        }

        private static Matrix<double> Rotate90Once(Matrix<double> input)
        {
            int n = input.RowCount;
            int m = input.ColumnCount;
            Matrix<double> output = Matrix<double>.Build.Dense(m, n);

            for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                output[j, n - 1 - i] = input[i, j];
            return output;
        }

        public string GetShapeId()
        {
            return AsteroidIdentifier.GetIdentifier(this);
        }

        public static string GetUnrotatedShapeId(Matrix<double> matrix)
        {
            var sb = new StringBuilder();
            //sb.Append(matrix.RowCount).Append("-")
            //    .Append(matrix.ColumnCount).Append("-");

            for (var x = 0; x < matrix.RowCount; x++)
            {
                for (var y = 0; y < matrix.ColumnCount; y++)
                {
                    sb.Append(matrix[x, y] > 0 ? "1" : "0");
                }
            }

            return sb.ToString();
        }
    }
}