using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NpcCore.Wpf.Helpers
{
    public static class ArrayHelper
    {
        public static T[,] ToRectangular<T>(T[] flatArray, int width)
        {
            int height = flatArray.Length / width;
            T[,] result = new T[height, width];

            int index = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    result[row, col] = flatArray[index++];
                }
            }

            return result;
        }

        /// <summary>
        /// Convert byte array to short array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static float[] ConvertToFloatArray(byte[] data)
        {
            if (data == null)
                return null;

            int count = data.Length / sizeof(float);
            float[] floatArr = new float[count];
            Buffer.BlockCopy(data, 0, floatArr, 0, data.Length);
            
            return floatArr;
        }

        public static T[] Slice<T>(this T[] source, int fromIdx, int toIdx)
        {
            T[] ret = new T[toIdx - fromIdx + 1];
            for (int srcIdx = fromIdx, dstIdx = 0; srcIdx <= toIdx; srcIdx++)
            {
                ret[dstIdx++] = source[srcIdx];
            }
            return ret;
        }

        [SuppressMessage("Blocker Code Smell", "S2368:Public methods should not have multidimensional array parameters", Justification = "<Pending>")]
        public static T[,] Slice<T>(this T[,] source, int x1, int y1, int x2, int y2)
        {
            T[,] result = new T[x2 - x1 + 1, y2 - y1 + 1];

            for (var i = x1; i <= x2; i++)
            {
                for (var j = y1; j <= y2; j++)
                {
                    result[i - x1, j - y1] = source[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            float length = (float)array.Length / size;
            for (var i = 0; i < length; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}
