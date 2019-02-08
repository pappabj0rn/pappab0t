using System;
using System.Collections.Generic;
using System.Linq;

namespace pappab0t.Extensions
{
    public static class RandomExtensions
    {
        public static T SelectOne<T>(this Random r, IEnumerable<T> source)
        {
            var array = source as T[] 
                        ?? source.ToArray();

            return array.ElementAt(r.Next(array.Length));
        }
    }
}