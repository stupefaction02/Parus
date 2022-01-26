using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;

namespace Naturistic.Core.Extensions
{
    public static class StreamExtension
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public async static Task<byte[]> ReadAllBytesAsync(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
