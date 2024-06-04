using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKGroupManagerRequests
{
    public static class StringExtensions
    {
        public static string[] Split(this string source, string value, StringSplitOptions options = StringSplitOptions.None)
        {
            return source?.Split(new[] { value }, options);
        }

        public static string[] Split(this string source, params string[] values)
        {
            return source?.Split(values, StringSplitOptions.None);
        }
    }
}
