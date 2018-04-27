using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;

namespace CCC_Linz18Spring
{
    public class Image
    {
        public int timestamp { get; set; }
        public int rowcount { get; set; }
        public int colcount { get; set; }

        // list of rows (where a row has the elements)
        public List<List<int>> rows { get; set; }
        public Matrix<double> matrix => Utils.ArrayToMatrix(rows);

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

        public Asteroid GetAsteroid()
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

            Matrix<double> submatrix = matrix.SubMatrix(rowIndex, rowCount, colIndex, colCount);
            return new Asteroid(submatrix, this);
        }

        public bool EqualAsteroidShape(Image image)
        {
            if (!image.HasAsteroid() || !HasAsteroid())
            {
                // no asteroid
                return false;
            }

            var myAsteroid = GetAsteroid();
            var yourAsteroid = image.GetAsteroid();

            return myAsteroid.EqualShape(yourAsteroid);
        }
    }
}