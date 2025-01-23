using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Common.Utils
{
    public static class DateTimeUtils
    {
        public static int ToUnixTimeSeconds(DateTime date)
        {
            DateTime point = new DateTime(1970, 1, 1);
            TimeSpan time = date.Subtract(point);

            return (int)time.TotalSeconds;
        }

        public static int ToUnixTimeSeconds()
        {
            return ToUnixTimeSeconds(DateTime.UtcNow);
        }
    }
}
