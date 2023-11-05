using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Common.Logging
{
    public class NaturisticLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new NaturisticLogger();
        }

        public void Dispose()
        {
            
        }
    }
}
