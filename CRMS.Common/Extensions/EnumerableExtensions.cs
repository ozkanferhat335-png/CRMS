using System;
using System.Collections.Generic;
using System.Linq;

namespace CRMS.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> enumerable, bool condition, Func<T, bool> predicate)
        {
            return condition ? enumerable.Where(predicate) : enumerable;
        }

        public static IEnumerable<T> DistinctBy<T, K>(this IEnumerable<T> enumerable, Func<T, K> selector)
        {
            var seen = new HashSet<K>();
            foreach (var item in enumerable)
            {
                var key = selector(item);
                if (seen.Add(key))
                {
                    yield return item;
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
    }
}