using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace CCC_Linz18Spring
{
    public class Utils
    {
        public static Matrix<double> ArrayToMatrix(List<List<int>> arr)
        {
            Matrix<double> matrix = Matrix<double>.Build.Dense(arr.Count, arr[0].Count);
            for (var row = 0; row < arr.Count; row++)
            {
                for (var col = 0; col < arr[row].Count; col++)
                {
                    matrix[row, col] = arr[row][col];
                }
            }

            return matrix;
        }
    }
}