using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Fundamental.Generic
{
    public static class StringExtensions
    {
        public static string Reverse(this string s)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = s.Length - 1; i >= 0; i--)
            {
                builder.Append(s.Substring(i, 1));
            }
            return builder.ToString();
        }

        public static int GetTlvId(this string input)
        {
            int h = 0;
            int len = input.Length;
            for (int i = 0; i < len; i++)
            {
                h = 31 * h + input[i];
            }
            return h;
        }
    }
}
