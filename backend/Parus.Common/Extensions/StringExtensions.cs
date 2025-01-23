using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Common.Extensions
{
    public static class StringExtensions
    {
        public static string GetStringInSpacedString(this string str, int number)
        {
            int l = str.Length;
            int i = 0;
            int skip = 0;
            List<char> temp = new List<char>();

            bool previousWasSpace = default;

            while (i <= l)
            {
                char c = str[i];

                if (c == ' ' && previousWasSpace)
                {
                    i++;
                    previousWasSpace = true;
                    continue;
                }

                if (c == ' ' && !previousWasSpace)
                {
                    if (skip == number)
                    {
                        return String.Concat(temp);
                    }

                    temp.Clear();

                    skip++;

                    previousWasSpace = true;
                }
                else
                {
                    previousWasSpace = false;
                    temp.Add(c);
                }

                i++;
            }

            return null;
        }
    }
}
