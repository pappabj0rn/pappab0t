using System;
using System.Collections.Generic;
using System.Linq;

namespace pappab0t.Extensions
{
    public static class EnumerableExtensions
    {
        [Obsolete("Use <[injected]Random>.SelectOne(options).")]
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            var enumerable1 = enumerable as T[] ?? enumerable.ToArray();

            return enumerable1.ElementAt(new Random().Next(enumerable1.Count()));
        }
    }
}
