using System;
using System.Collections.Generic;
using System.Text;

namespace Parus.Core.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}
