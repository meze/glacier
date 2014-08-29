using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Aws
{
    public class Checksum
    {
        public static string Calculate(Stream stream)
        {
            return Amazon.Glacier.TreeHashGenerator.CalculateTreeHash(stream);
        }
    }
}
