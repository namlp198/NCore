using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class LinqExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            var result = new ObservableCollection<T>();

            foreach (var item in source)
            {
                result.Add(item);
            }
            return result;
        }

        public static bool IsEqualElements<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if ((a == null && b != null && b.Count() == 0) || (b == null && a != null && a.Count() == 0))
            {
                return true;
            }

            if (a.Count() == b.Count())
            {
                int hitCount = 0;
                foreach (var element in a)
                {
                    hitCount += b.Contains(element) == true ? 1 : 0;
                }
                if (hitCount == b.Count())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// [Extension] Check Null or Empty for IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
    }
}
