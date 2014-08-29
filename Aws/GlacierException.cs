using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aws
{
    public class GlacierException : Exception
    {
        public GlacierException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
