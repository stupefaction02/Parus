using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Common.Extensions
{
     public static class CryptoExtensions
     {
        /// <summary>
        /// Uses ASCII encoding
        /// </summary>
        /// <param name="md5"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CreateHash(this MD5 md5, string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            var retVal = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < retVal.Length; i++)
                sb.Append(retVal[i].ToString("x2"));

            return sb.ToString();
        }
    }
}