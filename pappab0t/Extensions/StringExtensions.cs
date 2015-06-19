using System;

namespace pappab0t.Extensions
{
    public static class StringExtensions
    {
        public static string With(this string s, params object[] p)
        {
            return String.Format(s, p);
        }
    }
}
