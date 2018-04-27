using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;

namespace CCC_Linz18Spring
{
    public class Input
    {
        public int start { get; set; }
        public int end { get; set; }
        public int imagecount { get; set; }
        public List<Image> images { get; set; }
    }

    public class Image
    {
        public int timestamp { get; set; }
        public int rowcount { get; set; }
        public int colcount { get; set; }

        // list of rows (where a row has the elements)
        public List<List<int>> rows { get; set; }
        public int[][] rowsArray => rows.Select(a => a.ToArray()).ToArray();

        public bool HasAsteroid()
        {
            return rows.SelectMany(e => e).Any(e => e > 0);
        }

        public (Point2D upperLeft, Point2D lowerRight) GetBoundingBox()
        {
            int? leftRow = null, leftCol = null, rightRow = null, rightCol = null;

            for (var row = 0; row < rows.Count; row++)
            {
                var rowHasSpot = false;
                for (var col = 0; col < rows[row].Count; col++)
                {
                    int spot = rows[row][col];
                    if (spot > 0)
                    {
                        rowHasSpot = true;

                        // col dimension
                        if (leftCol == null || col < leftCol) leftCol = col;
                        if (rightCol == null || rightCol < col) rightCol = col;
                    }
                }

                // row dimension
                if (rowHasSpot)
                {
                    if (leftRow == null || row < leftRow) leftRow = row;
                    if (rightRow == null || rightRow < row) rightRow = row;
                }
            }

            if (leftRow == null || leftCol == null || rightRow == null || rightCol == null)
            {
                throw new Exception("bounding box could not be calculated");
            }

            return (new Point2D(leftRow.Value, leftCol.Value), new Point2D(rightRow.Value, rightCol.Value));
        }

        public Matrix<double> GetAsteroid()
        {
            if (!HasAsteroid())
            {
                throw new Exception("no asteroid to extract");
            }

            var boundingBox = GetBoundingBox();
            int rowIndex = (int) boundingBox.upperLeft.X;
            int colIndex = (int) boundingBox.upperLeft.Y;
            int rowCount = (int) (boundingBox.lowerRight.X - boundingBox.upperLeft.X) + 1;
            int colCount = (int) (boundingBox.lowerRight.Y - boundingBox.upperLeft.Y) + 1;

            Matrix<double> matrix = GetMatrix();
            Matrix<double> submatrix = matrix.SubMatrix(rowIndex, rowCount, colIndex, colCount);
            return submatrix;
        }

        public Matrix<double> GetMatrix()
        {
            var matrix = Matrix<double>.Build.Dense(rowcount, colcount);
            for (var row = 0; row < rows.Count; row++)
            {
                for (var col = 0; col < rows[row].Count; col++)
                {
                    matrix[row, col] = rows[row][col];
                }
            }

            return matrix;
        }

        public bool EqualAsteroid(Image image)
        {
            if (!image.HasAsteroid() || !HasAsteroid())
            {
                // no asteroid
                return false;
            }

            Matrix<double> myAsteroid = GetAsteroid();
            Matrix<double> yourAsteroid = image.GetAsteroid();

            return EqualAsteroid(myAsteroid, yourAsteroid);
        }

        public static bool EqualAsteroid(Matrix<double> myAsteroid, Matrix<double> yourAsteroid)
        {
            if (myAsteroid.ColumnCount != yourAsteroid.ColumnCount
                || myAsteroid.RowCount != yourAsteroid.RowCount)
            {
                // shape not equal
                return false;
            }

            for (var x = 0; x < myAsteroid.RowCount; x++)
            {
                for (var y = 0; y < myAsteroid.ColumnCount; y++)
                {
                    if (myAsteroid[x, y] > 0 ^ yourAsteroid[x, y] > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public class Shape
    {
    }
}