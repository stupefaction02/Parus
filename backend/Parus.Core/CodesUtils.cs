using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Parus.Core.Utils
{
    public static class CodesUtils
    {
        private static readonly Random random = new Random();
		public static int RandomizeEmailVerificatioCode()
        {
            string a = random.Next(1, 10).ToString();
            string b = random.Next(0, 10).ToString();
            string c = random.Next(0, 10).ToString();
            string d = random.Next(0, 10).ToString();
            string f = random.Next(0, 10).ToString();
            return Int32.Parse(a + b + c + d + f);
        }
	}
}
