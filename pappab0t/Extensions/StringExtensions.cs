using System;

namespace pappab0t.Extensions
{
    public static class StringExtensions
    {
        public static string With(this string s, params object[] p)
        {
            return String.Format(s, p);
        }

        public static string Fallback(this string s, string fallback)
        {
            return String.IsNullOrEmpty(s)
                    ? fallback
                    : s;
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
    }
}
