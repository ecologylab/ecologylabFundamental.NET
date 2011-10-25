using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Fundamental.Generic
{
    public static class IntExtensions
    {
        public static String HexString(this Int32 myInt)
        {
            char[] hex = new char[4];

            for (int i = 0; i < 4; i++)
            {
                int num = myInt % 16;

                if (num < 10)
                    hex[3 - i] = (char)('0' + num);
                else
                    hex[3 - i] = (char)('A' + (num - 10));

                myInt >>= 4;
            }

            return new string(hex);
        }
    }
}
