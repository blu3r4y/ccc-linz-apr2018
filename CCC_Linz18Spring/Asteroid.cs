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
            if (Matrix.ColumnCount != yourAsteroid.Matrix.ColumnCount
                || Matrix.RowCount != yourAsteroid.Matrix.RowCount)
            {
                // shape not equal
                return false;
            }

            for (var x = 0; x < Matrix.RowCount; x++)
            {
                for (var y = 0; y < Matrix.ColumnCount; y++)
                {
                    if (Matrix[x, y] > 0 ^ yourAsteroid.Matrix[x, y] > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public string GetShapeId()
        {
            var sb = new StringBuilder();
            sb.Append(Matrix.RowCount).Append("-")
                .Append(Matrix.ColumnCount).Append("-");

            for (var x = 0; x < Matrix.RowCount; x++)
            {
                for (var y = 0; y < Matrix.ColumnCount; y++)
                {
                    sb.Append(Matrix[x, y] > 0 ? "1" : "0");
                }
            }

            return sb.ToString();
        }
    }
}