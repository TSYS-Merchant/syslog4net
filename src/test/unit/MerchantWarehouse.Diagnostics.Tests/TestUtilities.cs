using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantWarehouse.Diagnostics.Tests
{
    public static class TestUtilities
    {
        public static string GetStringFromStream(Stream stream)
        {
            stream.Position = 0;
            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
